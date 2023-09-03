//namespace VulkanExample;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Input;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;
using Silk.NET.Windowing;
using UltralightNet;
using UltralightNet.AppCore;
using UltralightNet.Platform;
using Buffer = Silk.NET.Vulkan.Buffer;

using Application app = new();
app.Run();

internal unsafe partial class Application : IDisposable
{
	readonly Stopwatch stopwatch = Stopwatch.StartNew();

	readonly IWindow window = Window.Create(WindowOptions.DefaultVulkan with { UpdatesPerSecond = 60, FramesPerSecond = 61 });
	readonly IInputContext input;
	readonly Vk vk = Vk.GetApi();
	readonly Instance instance;

	readonly KhrSurface khrSurface;
	readonly ExtDebugUtils? debugUtils;

	readonly SurfaceKHR surface;
	readonly PhysicalDevice physicalDevice;
	readonly PhysicalDeviceMemoryProperties physicalDeviceMemoryProperties;
	readonly uint graphicsQueueFamily = uint.MaxValue;
	readonly uint presentQueueFamily = uint.MaxValue;

	readonly Device device;
	readonly Queue graphicsQueue;
	readonly Queue presentQueue;
	readonly KhrSwapchain khrSwapchain;

	//readonly SampleCountFlags sampleCountFlags = SampleCountFlags.SampleCount4Bit;
	readonly SurfaceCapabilitiesKHR surfaceCapabilitiesKHR;
	readonly SurfaceFormatKHR surfaceFormat = new(Format.B8G8R8A8Srgb, ColorSpaceKHR.ColorSpaceSrgbNonlinearKhr);
	readonly PresentModeKHR presentMode = PresentModeKHR.PresentModeFifoKhr;

	//readonly Sampler sampler;
	readonly DescriptorSetLayout descriptorSetLayout;
	readonly PipelineLayout pipelineLayout;
	readonly RenderPass renderPass;
	readonly Pipeline pipeline;

	readonly CommandPool commandPool;

	readonly Renderer renderer;
	readonly View view;

	public Application()
	{
		{ // Window
			window.Initialize();
			if (window.VkSurface is null) throw new PlatformNotSupportedException("Vulkan surface not found.");

			window.FramebufferResize += (_) => resized = true;
			input = window.CreateInput();
		}
		{ // Instance
			var extensions = SilkMarshal.PtrToStringArray((nint)window.VkSurface.GetRequiredExtensions(out uint surfaceExtensionCount), (int)surfaceExtensionCount).ToList();
			if (vk.IsInstanceExtensionPresent(ExtDebugUtils.ExtensionName)) extensions.Add(ExtDebugUtils.ExtensionName);

			Utils.CreateInstance(vk, extensions.ToArray(), out instance);
		}
		{ // Instance extensions
			if (!vk.TryGetInstanceExtension(instance, out khrSurface)) throw new Exception($"{KhrSurface.ExtensionName} extension not found.");
			surface = window.VkSurface.Create<AllocationCallbacks>(instance.ToHandle(), null).ToSurface();

			vk.TryGetInstanceExtension(instance, out debugUtils);
		}
		{ // Physical Device
			uint deviceCount = 0;
			vk.EnumeratePhysicalDevices(instance, ref deviceCount, null).Check();
			if (deviceCount is 0) throw new Exception("Couldn't find physical vulkan device.");
			var devices = stackalloc PhysicalDevice[(int)deviceCount];
			vk.EnumeratePhysicalDevices(instance, ref deviceCount, devices).Check();
			physicalDevice = devices[0]; // idc
			if (!vk.IsDeviceExtensionPresent(physicalDevice, KhrSwapchain.ExtensionName)) throw new Exception($"Physical device doesn't support ${KhrSwapchain.ExtensionName}");

			uint queueFamilityCount = 0;
			vk.GetPhysicalDeviceQueueFamilyProperties(physicalDevice, ref queueFamilityCount, null);
			var queueFamilyProperties = stackalloc QueueFamilyProperties[(int)queueFamilityCount];
			vk.GetPhysicalDeviceQueueFamilyProperties(physicalDevice, ref queueFamilityCount, queueFamilyProperties);

			for (uint i = 0; i < queueFamilityCount; i++)
			{
				if (graphicsQueueFamily is uint.MaxValue && queueFamilyProperties[i].QueueFlags.HasFlag(QueueFlags.QueueGraphicsBit)) graphicsQueueFamily = i;
				if (presentQueueFamily is uint.MaxValue)
				{
					khrSurface.GetPhysicalDeviceSurfaceSupport(physicalDevice, i, surface, out Bool32 supported).Check();
					if ((bool)supported is true) presentQueueFamily = i;
				}
				if ((graphicsQueueFamily | presentQueueFamily) is not uint.MaxValue) break;
			}
			if ((graphicsQueueFamily | presentQueueFamily) is uint.MaxValue) throw new Exception("Suitable queue families not found.");

			vk.GetPhysicalDeviceMemoryProperties(physicalDevice, out physicalDeviceMemoryProperties);
		}
		{ // Device + Queues
			float priority = 1.0f;
			var queueCreateInfos = stackalloc DeviceQueueCreateInfo[2];
			queueCreateInfos[0] = new(queueFamilyIndex: graphicsQueueFamily, queueCount: 1, pQueuePriorities: &priority);
			queueCreateInfos[1] = new(queueFamilyIndex: presentQueueFamily, queueCount: 1, pQueuePriorities: &priority);

			PhysicalDeviceFeatures physicalDeviceFeatures = new() { SamplerAnisotropy = true };

			var extensions = new string[] { KhrSwapchain.ExtensionName };
			byte** extensionsPtr = (byte**)SilkMarshal.StringArrayToPtr(extensions);

			try
			{
				DeviceCreateInfo deviceCreateInfo = new(
					queueCreateInfoCount: graphicsQueueFamily == presentQueueFamily ? 1u : 2u, pQueueCreateInfos: queueCreateInfos,
					enabledExtensionCount: (uint)extensions.Length, ppEnabledExtensionNames: extensionsPtr,
					pEnabledFeatures: &physicalDeviceFeatures);

				vk.CreateDevice(physicalDevice, &deviceCreateInfo, null, out device).Check();

				vk.GetDeviceQueue(device, graphicsQueueFamily, 0, out graphicsQueue);
				if (graphicsQueueFamily == presentQueueFamily) presentQueue = graphicsQueue;
				else
				{
					vk.GetDeviceQueue(device, presentQueueFamily, 0, out presentQueue);
					throw new NotSupportedException("Separate graphics and present queues are not supported.");
				}

#if DEBUG
				debugUtils?.SetDebugUtilsObjectName(device, new DebugUtilsObjectNameInfoEXT(objectType: ObjectType.Queue, objectHandle: (ulong)graphicsQueue.Handle, pObjectName: "Graphics Queue"u8.ToPointer()));
#endif

				if (!vk.TryGetDeviceExtension(instance, device, out khrSwapchain)) throw new Exception($"{KhrSwapchain.ExtensionName} extension not found.");
			}
			finally
			{
				for (uint i = 0; i < extensions.Length; i++) SilkMarshal.FreeString((nint)extensionsPtr[i]);
				SilkMarshal.Free((nint)extensionsPtr);
			}
		}

		/*{ // Sampler
			var samplerCreateInfo = new SamplerCreateInfo()
			{
				SType = StructureType.SamplerCreateInfo,
				MagFilter = Filter.Linear,
				MinFilter = Filter.Linear,
				AddressModeU = SamplerAddressMode.ClampToEdge,
				AddressModeV = SamplerAddressMode.ClampToEdge,
				AddressModeW = SamplerAddressMode.Repeat,
				AnisotropyEnable = false,
				MaxAnisotropy = 1,
				BorderColor = BorderColor.IntOpaqueBlack,
				UnnormalizedCoordinates = false,
				CompareEnable = false,
				CompareOp = CompareOp.Never,
				MipmapMode = SamplerMipmapMode.Linear,
				MinLod = 0,
				MaxLod = 1,
				MipLodBias = 0
			};
			vk.CreateSampler(device, &samplerCreateInfo, null, out sampler);
		}*/
		{ // Surface support
			khrSurface.GetPhysicalDeviceSurfaceCapabilities(physicalDevice, surface, out surfaceCapabilitiesKHR).Check();

			uint surfaceFormatCount = 0;
			khrSurface.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, &surfaceFormatCount, null).Check();
			if (surfaceFormatCount is 0) throw new Exception("No surface formats found.");
			Span<SurfaceFormatKHR> surfaceFormats = stackalloc SurfaceFormatKHR[(int)surfaceFormatCount];
			khrSurface.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, &surfaceFormatCount, surfaceFormats).Check();

			uint presentModeCount = 0;
			khrSurface.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, &presentModeCount, null).Check();
			if (presentModeCount is 0) throw new Exception("No present modes found.");
			Span<PresentModeKHR> presentModes = stackalloc PresentModeKHR[(int)presentModeCount];
			khrSurface.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, &presentModeCount, presentModes).Check();

			for (int i = 0; i < surfaceFormats.Length; i++)
			{
				SurfaceFormatKHR surfaceFormat = surfaceFormats[i];
				if (surfaceFormat.Format == this.surfaceFormat.Format && surfaceFormat.ColorSpace == this.surfaceFormat.ColorSpace) break;
				else if (i == surfaceFormats.Length - 1) throw new Exception("No supported surface format found.");
			}

			for (int i = 0; i < presentModes.Length; i++)
			{
				PresentModeKHR presentMode = presentModes[i];
				if (presentMode == this.presentMode) break;
				else if (i == presentModes.Length - 1) throw new Exception("No supported present mode found.");
			}
		}
		{ // DescriptorSetLayout
			var descriptorSetLayoutBinding = new DescriptorSetLayoutBinding(binding: 0, descriptorType: DescriptorType.CombinedImageSampler, descriptorCount: 1, stageFlags: ShaderStageFlags.ShaderStageFragmentBit);
			var descriptorSetLayoutCreateInfo = new DescriptorSetLayoutCreateInfo(bindingCount: 1, pBindings: &descriptorSetLayoutBinding);
			vk.CreateDescriptorSetLayout(device, &descriptorSetLayoutCreateInfo, null, out descriptorSetLayout).Check();
		}
		{ // PipelineLayout
			fixed (DescriptorSetLayout* pDescriptorSetLayout = &descriptorSetLayout)
			{
				var pipelineLayoutCreateInfo = new PipelineLayoutCreateInfo(/*setLayoutCount: 1,*/ pSetLayouts: pDescriptorSetLayout);
				vk.CreatePipelineLayout(device, &pipelineLayoutCreateInfo, null, out pipelineLayout).Check();
			}
		}
		{ // RenderPass
			var colorAttachment = new AttachmentDescription(
				format: surfaceFormat.Format, samples: SampleCountFlags.SampleCount1Bit,
				loadOp: AttachmentLoadOp.DontCare, storeOp: AttachmentStoreOp.Store,
				stencilLoadOp: AttachmentLoadOp.DontCare, stencilStoreOp: AttachmentStoreOp.DontCare,
				initialLayout: ImageLayout.Undefined, finalLayout: ImageLayout.PresentSrcKhr);
			var colorAttachmentRef = new AttachmentReference(0, ImageLayout.ColorAttachmentOptimal);
			var subpass = new SubpassDescription(pipelineBindPoint: PipelineBindPoint.Graphics, colorAttachmentCount: 1, pColorAttachments: &colorAttachmentRef);
			var subpassDependency = new SubpassDependency(
				Vk.SubpassExternal, 0,
				PipelineStageFlags.PipelineStageColorAttachmentOutputBit | PipelineStageFlags.PipelineStageFragmentShaderBit, PipelineStageFlags.PipelineStageColorAttachmentOutputBit | PipelineStageFlags.PipelineStageFragmentShaderBit,
				0, AccessFlags.AccessColorAttachmentWriteBit);
			var renderPassCreateInfo = new RenderPassCreateInfo(
				attachmentCount: 1, pAttachments: &colorAttachment,
				subpassCount: 1, pSubpasses: &subpass,
				dependencyCount: 1, pDependencies: &subpassDependency);
			vk.CreateRenderPass(device, &renderPassCreateInfo, null, out renderPass).Check();
		}
		{ // Pipeline
			ShaderModule CreateShaderModule(byte* pointer, nuint length)
			{
				var shaderModuleCreateInfo = new ShaderModuleCreateInfo(codeSize: length, pCode: (uint*)pointer);
				ShaderModule module;
				vk.CreateShaderModule(device, &shaderModuleCreateInfo, null, &module).Check();
				return module;
			}

			var vertStream = typeof(Application).Assembly.GetManifestResourceStream("UltralightNet.Vulkan.TestApp.shaders.spirv.quad.vert.spv") as UnmanagedMemoryStream ?? throw new Exception("Shaders not found.");
			var fragStream = typeof(Application).Assembly.GetManifestResourceStream("UltralightNet.Vulkan.TestApp.shaders.spirv.quad.frag.spv") as UnmanagedMemoryStream ?? throw new Exception("Shaders not found.");

			var vert = CreateShaderModule(vertStream.PositionPointer, (nuint)vertStream.Length);
			var frag = CreateShaderModule(fragStream.PositionPointer, (nuint)fragStream.Length);

#if DEBUG
			debugUtils?.SetDebugUtilsObjectName(device, new DebugUtilsObjectNameInfoEXT(objectType: ObjectType.ShaderModule, objectHandle: vert.Handle, pObjectName: "Quad Vertex Shader"u8.ToPointer()));
			debugUtils?.SetDebugUtilsObjectName(device, new DebugUtilsObjectNameInfoEXT(objectType: ObjectType.ShaderModule, objectHandle: frag.Handle, pObjectName: "Quad Fragment Shader"u8.ToPointer()));
#endif

			try
			{
				var pipelineStages = stackalloc PipelineShaderStageCreateInfo[2] {
					new(stage: ShaderStageFlags.ShaderStageVertexBit, module: vert, pName: "main"u8.ToPointer()),
					new(stage: ShaderStageFlags.ShaderStageFragmentBit, module: frag, pName: "main"u8.ToPointer())
				};
				var pipelineVertexInputStateCreateInfo = new PipelineVertexInputStateCreateInfo(flags: 0);
				var pipelineInputAssemblyStateCreateInfo = new PipelineInputAssemblyStateCreateInfo(topology: PrimitiveTopology.TriangleList);
				var pipelineViewportStateCreateInfo = new PipelineViewportStateCreateInfo(viewportCount: 1, scissorCount: 1);
				var pipelineRasterizationStateCreateInfo = new PipelineRasterizationStateCreateInfo(polygonMode: PolygonMode.Fill, cullMode: CullModeFlags.CullModeFrontBit, frontFace: FrontFace.CounterClockwise, lineWidth: 1.0f);
				var pipelineMultisampleStateCreateInfo = new PipelineMultisampleStateCreateInfo(rasterizationSamples: SampleCountFlags.SampleCount1Bit);
				var pipelineDepthStencilStateCreateInfo = new PipelineDepthStencilStateCreateInfo(depthTestEnable: false);
				var pipelineColorBlendAttachmentState = new PipelineColorBlendAttachmentState(blendEnable: false, colorWriteMask: ColorComponentFlags.ColorComponentRBit | ColorComponentFlags.ColorComponentGBit | ColorComponentFlags.ColorComponentBBit | ColorComponentFlags.ColorComponentABit);
				var pipelineColorBlendStateCreateInfo = new PipelineColorBlendStateCreateInfo(attachmentCount: 1, pAttachments: &pipelineColorBlendAttachmentState);
				var dynamicStates = stackalloc DynamicState[] { DynamicState.Viewport, DynamicState.Scissor };
				var pipelineDynamicStateCreateInfo = new PipelineDynamicStateCreateInfo(dynamicStateCount: 2, pDynamicStates: dynamicStates);
				var graphicsPipelineCreateInfo = new GraphicsPipelineCreateInfo(
					stageCount: 2, pStages: pipelineStages,
					pVertexInputState: &pipelineVertexInputStateCreateInfo,
					pInputAssemblyState: &pipelineInputAssemblyStateCreateInfo,
					pViewportState: &pipelineViewportStateCreateInfo,
					pRasterizationState: &pipelineRasterizationStateCreateInfo,
					pMultisampleState: &pipelineMultisampleStateCreateInfo,
					pDepthStencilState: &pipelineDepthStencilStateCreateInfo,
					pColorBlendState: &pipelineColorBlendStateCreateInfo,
					pDynamicState: &pipelineDynamicStateCreateInfo,
					layout: pipelineLayout, renderPass: renderPass);
				vk.CreateGraphicsPipelines(device, default /* TODO: PipelineCache */, 1, &graphicsPipelineCreateInfo, null, out pipeline).Check();

#if DEBUG
				debugUtils?.SetDebugUtilsObjectName(device, new DebugUtilsObjectNameInfoEXT(objectType: ObjectType.Pipeline, objectHandle: pipeline.Handle, pObjectName: "Quad Pipeline"u8.ToPointer()));
#endif
			}
			finally
			{
				vk.DestroyShaderModule(device, frag, null);
				vk.DestroyShaderModule(device, vert, null);
			}
		}

		{ // CommandPool
			var commandPoolCreateInfo = new CommandPoolCreateInfo(flags: CommandPoolCreateFlags.CommandPoolCreateResetCommandBufferBit);
			vk.CreateCommandPool(device, &commandPoolCreateInfo, null, out commandPool).Check();
		}

		{ // Ultralight
			AppCoreMethods.SetPlatformFontLoader();

			ULPlatform.SurfaceDefinition = this;

			renderer = ULPlatform.CreateRenderer();

			window.Update += (delta) => renderer.Update();

			view = renderer.CreateView((uint)window.FramebufferSize.X, (uint)window.FramebufferSize.Y);
			view.URL = "https://youtube.com";
		}
		{ // Input controls
			void SetupInput(IInputDevice device)
			{
				if (device is IMouse mouse)
				{
					mouse.MouseDown += (m, button) => view.FireMouseEvent(new() { Type = ULMouseEventType.MouseDown, X = (int)m.Position.X, Y = (int)m.Position.Y, Button = button switch { MouseButton.Left => ULMouseEventButton.Left, MouseButton.Right => ULMouseEventButton.Right, MouseButton.Middle => ULMouseEventButton.Middle, _ => ULMouseEventButton.None } });
				}
			}

			input.ConnectionChanged += (device, _) => SetupInput(device); // this doesn't work, at least, on mouse
			foreach (var mouse in input.Mice) SetupInput(mouse);
		}

		Console.WriteLine($"Initialized Application in {stopwatch.Elapsed}");
	}

	const int MaxFramesInFlight = 3;

	bool resized = false;

	SwapchainKHR swapchain;
	Extent2D extent;
	int framesInFlight;
	Image[]? swapchainImages;
	ImageView[]? swapchainImageViews;
	Framebuffer[]? framebuffers;
	CommandBuffer[]? commandBuffers;

	Semaphore[]? imageAvailableSemaphores;
	Semaphore[]? renderFinishedSemaphores;
	Fence[]? renderFinishedFences;

	int CurrentFrame;

	void CreateSwapchain()
	{
		Extent2D GetExtent()
		{
			if ((surfaceCapabilitiesKHR.CurrentExtent.Width | surfaceCapabilitiesKHR.CurrentExtent.Height) is not uint.MaxValue) return surfaceCapabilitiesKHR.CurrentExtent;
			else
			{
				var framebufferSize = (Silk.NET.Maths.Vector2D<uint>)window.FramebufferSize;
				return new Extent2D(
					Math.Clamp(framebufferSize.X, surfaceCapabilitiesKHR.MinImageExtent.Width, surfaceCapabilitiesKHR.MaxImageExtent.Width),
					Math.Clamp(framebufferSize.Y, surfaceCapabilitiesKHR.MinImageExtent.Height, surfaceCapabilitiesKHR.MaxImageExtent.Height)
				);
			}
		}
		var imageCount = Math.Min(surfaceCapabilitiesKHR.MinImageCount + 1, surfaceCapabilitiesKHR.MaxImageCount);
		extent = GetExtent();
		var queueFamilyIndices = stackalloc uint[] { graphicsQueueFamily, presentQueueFamily };
		var swapchainCreateInfoKHR = new SwapchainCreateInfoKHR(
			surface: surface,
			minImageCount: imageCount,
			imageFormat: surfaceFormat.Format,
			imageColorSpace: surfaceFormat.ColorSpace,
			imageExtent: extent,
			imageArrayLayers: 1,
			imageUsage: ImageUsageFlags.ImageUsageColorAttachmentBit,
			imageSharingMode: graphicsQueueFamily == presentQueueFamily ? SharingMode.Exclusive : SharingMode.Concurrent,
			queueFamilyIndexCount: graphicsQueueFamily == presentQueueFamily ? 1u : 2u,
			pQueueFamilyIndices: queueFamilyIndices,
			preTransform: surfaceCapabilitiesKHR.CurrentTransform,
			compositeAlpha: CompositeAlphaFlagsKHR.CompositeAlphaOpaqueBitKhr,
			presentMode: presentMode,
			clipped: true,
			oldSwapchain: swapchain);

		khrSwapchain.CreateSwapchain(device, &swapchainCreateInfoKHR, null, out swapchain).Check();

#if DEBUG
		using ULString swapchainName = new($"{window.Title} Swapchain");
		debugUtils?.SetDebugUtilsObjectName(device, new DebugUtilsObjectNameInfoEXT(objectType: ObjectType.SwapchainKhr, objectHandle: swapchain.Handle, pObjectName: swapchainName.data));
#endif

		khrSwapchain.GetSwapchainImages(device, swapchain, &imageCount, null).Check();
		swapchainImages = new Image[framesInFlight = (int)imageCount];
		khrSwapchain.GetSwapchainImages(device, swapchain, &imageCount, swapchainImages.AsSpan()).Check();

		swapchainImageViews = new ImageView[imageCount];
		for (int i = 0; i < swapchainImageViews.Length; i++)
		{
			var imageViewCreateInfo = new ImageViewCreateInfo(image: swapchainImages[i], viewType: ImageViewType.ImageViewType2D, format: surfaceFormat.Format, subresourceRange: new ImageSubresourceRange(ImageAspectFlags.ImageAspectColorBit, 0, 1, 0, 1));
			vk.CreateImageView(device, &imageViewCreateInfo, null, out swapchainImageViews[i]).Check();
		}

		framebuffers = new Framebuffer[imageCount];
		for (int i = 0; i < framebuffers.Length; i++)
		{
			fixed (ImageView* imageView = &swapchainImageViews[i])
			{
				var framebufferCreateInfo = new FramebufferCreateInfo(renderPass: renderPass, attachmentCount: 1, pAttachments: imageView, width: extent.Width, height: extent.Height, layers: 1);
				vk.CreateFramebuffer(device, &framebufferCreateInfo, null, out framebuffers[i]).Check();
			}

#if DEBUG
			using ULString swapchainImageName = new($"Swapchain Image#{i}");
			using ULString swapchainImageViewName = new($"Swapchain ImageView#{i}");
			using ULString framebufferName = new($"Swapchain Framebuffer#{i}");
			debugUtils?.SetDebugUtilsObjectName(device, new DebugUtilsObjectNameInfoEXT(objectType: ObjectType.Image, objectHandle: swapchainImages[i].Handle, pObjectName: swapchainImageName.data));
			debugUtils?.SetDebugUtilsObjectName(device, new DebugUtilsObjectNameInfoEXT(objectType: ObjectType.ImageView, objectHandle: swapchainImageViews[i].Handle, pObjectName: swapchainImageViewName.data));
			debugUtils?.SetDebugUtilsObjectName(device, new DebugUtilsObjectNameInfoEXT(objectType: ObjectType.Framebuffer, objectHandle: framebuffers[i].Handle, pObjectName: framebufferName.data));
#endif
		}

		commandBuffers = new CommandBuffer[imageCount];
		var commandBufferAllocateInfo = new CommandBufferAllocateInfo(commandPool: commandPool, level: CommandBufferLevel.Primary, commandBufferCount: imageCount);
		vk.AllocateCommandBuffers(device, &commandBufferAllocateInfo, commandBuffers.AsSpan()).Check();

		imageAvailableSemaphores = new Semaphore[imageCount];
		renderFinishedSemaphores = new Semaphore[imageCount];
		renderFinishedFences = new Fence[imageCount];

		for (int i = 0; i < imageCount; i++)
		{
			var semaphoreCreateInfo = new SemaphoreCreateInfo(flags: 0);
			var fenceCreateInfo = new FenceCreateInfo(flags: FenceCreateFlags.FenceCreateSignaledBit);

			vk.CreateSemaphore(device, &semaphoreCreateInfo, null, out imageAvailableSemaphores[i]).Check();
			vk.CreateSemaphore(device, &semaphoreCreateInfo, null, out renderFinishedSemaphores[i]).Check();
			vk.CreateFence(device, &fenceCreateInfo, null, out renderFinishedFences[i]).Check();

#if DEBUG
			using ULString imageAvailableSemaphoreName = new($"Image Available Semaphore#{i}");
			using ULString renderFinishedSemaphoreName = new($"Render Finished Semaphore#{i}");
			using ULString renderFinishedFenceName = new($"Render Finished Fence#{i}");
			debugUtils?.SetDebugUtilsObjectName(device, new DebugUtilsObjectNameInfoEXT(objectType: ObjectType.Semaphore, objectHandle: imageAvailableSemaphores[i].Handle, pObjectName: imageAvailableSemaphoreName.data));
			debugUtils?.SetDebugUtilsObjectName(device, new DebugUtilsObjectNameInfoEXT(objectType: ObjectType.Semaphore, objectHandle: renderFinishedSemaphores[i].Handle, pObjectName: renderFinishedSemaphoreName.data));
			debugUtils?.SetDebugUtilsObjectName(device, new DebugUtilsObjectNameInfoEXT(objectType: ObjectType.Fence, objectHandle: renderFinishedFences[i].Handle, pObjectName: renderFinishedFenceName.data));
#endif
		}
	}

	void RecordCommandBuffers()
	{
		for (int i = 0; i < commandBuffers!.Length; i++)
		{
			var commandBufferBeginInfo = new CommandBufferBeginInfo(pNext: null);
			vk.BeginCommandBuffer(commandBuffers[i], &commandBufferBeginInfo).Check();
			var renderPassBeginInfo = new RenderPassBeginInfo(renderPass: renderPass, framebuffer: framebuffers![i], renderArea: new Rect2D(extent: extent));
			vk.CmdBeginRenderPass(commandBuffers[i], &renderPassBeginInfo, SubpassContents.Inline);
			vk.CmdBindPipeline(commandBuffers[i], PipelineBindPoint.Graphics, pipeline);
			var viewport = new Viewport(width: extent.Width, height: extent.Height, minDepth: 0, maxDepth: 1);
			var scissor = new Rect2D(extent: extent);
			vk.CmdSetViewport(commandBuffers[i], 0, 1, &viewport);
			vk.CmdSetScissor(commandBuffers[i], 0, 1, &scissor);
			// TODO: bind texture
			vk.CmdDraw(commandBuffers[i], 3, 1, 0, 0);
			vk.CmdEndRenderPass(commandBuffers[i]);
			vk.EndCommandBuffer(commandBuffers[i]).Check();
		}
	}

	void CleanupSwapchainResources()
	{
		vk.WaitForFences(device, renderFinishedFences, true, ulong.MaxValue).Check();

		for (int i = 0; i < framesInFlight; i++)
		{
			vk.DestroySemaphore(device, imageAvailableSemaphores![i], null);
			vk.DestroySemaphore(device, renderFinishedSemaphores![i], null);
			vk.DestroyFence(device, renderFinishedFences![i], null);
		}
		imageAvailableSemaphores = renderFinishedSemaphores = null;
		renderFinishedFences = null;

		vk.FreeCommandBuffers(device, commandPool, (uint)commandBuffers!.Length, commandBuffers);
		for (int i = 0; i < framebuffers?.Length; i++)
			vk.DestroyFramebuffer(device, framebuffers[i], null);
		framebuffers = null;
		for (int i = 0; i < swapchainImageViews?.Length; i++)
			vk.DestroyImageView(device, swapchainImageViews[i], null);
		swapchainImageViews = null;
		swapchainImages = null;
	}

	public void Run()
	{
		CreateSwapchain();
		RecordCommandBuffers();

		window.Render += (delta) =>
		{
			vk.WaitForFences(device, 1, renderFinishedFences![CurrentFrame], true, ulong.MaxValue).Check();

			if (resized)
			{
				view.Resize((uint)window.FramebufferSize.X, (uint)window.FramebufferSize.Y);
				resized = false;
			}
			renderer.Render();

			uint imageIndex = 0;

			var result = khrSwapchain.AcquireNextImage(device, swapchain, ulong.MaxValue, imageAvailableSemaphores![CurrentFrame], default, ref imageIndex);
			// check
			Debug.Assert(imageIndex == CurrentFrame);


			vk.ResetFences(device, 1, renderFinishedFences![CurrentFrame]).Check();

			var waitSemaphore = imageAvailableSemaphores![CurrentFrame];
			var waitStage = PipelineStageFlags.PipelineStageColorAttachmentOutputBit;
			var commandBuffer = commandBuffers![CurrentFrame];
			var signalSemaphore = renderFinishedSemaphores![CurrentFrame];
			var submitInfo = new SubmitInfo(
				waitSemaphoreCount: 1, pWaitSemaphores: &waitSemaphore, pWaitDstStageMask: &waitStage,
				commandBufferCount: 1, pCommandBuffers: &commandBuffer,
				signalSemaphoreCount: 1, pSignalSemaphores: &signalSemaphore);
			vk.QueueSubmit(graphicsQueue, 1, &submitInfo, renderFinishedFences![CurrentFrame]).Check();

			fixed (SwapchainKHR* swapchainPtr = &swapchain)
			{
				var presentInfo = new PresentInfoKHR(
					waitSemaphoreCount: 1, pWaitSemaphores: &signalSemaphore,
					swapchainCount: 1, pSwapchains: swapchainPtr,
					pImageIndices: &imageIndex);
				khrSwapchain.QueuePresent(presentQueue, &presentInfo).Check();
			}

			CurrentFrame = (CurrentFrame + 1) % MaxFramesInFlight;
		};

		window.Run();
		CleanupSwapchainResources();
	}

	void IDisposable.Dispose()
	{
		if (swapchain.Handle is not 0) khrSwapchain.DestroySwapchain(device, swapchain, null);
		view.Dispose();
		renderer.Dispose();
		vk.DestroyCommandPool(device, commandPool, null);
		vk.DestroyPipeline(device, pipeline, null);
		vk.DestroyRenderPass(device, renderPass, null);
		vk.DestroyPipelineLayout(device, pipelineLayout, null);
		vk.DestroyDescriptorSetLayout(device, descriptorSetLayout, null);
		//vk.DestroySampler(device, sampler, null);
		vk.DestroyDevice(device, null);
		khrSwapchain.Dispose();
		khrSurface.DestroySurface(instance, surface, null);
		khrSurface.Dispose();
		debugUtils?.Dispose();
		vk.DestroyInstance(instance, null);
		vk.Dispose();
		input.Dispose();
		window.Dispose();
	}
}
