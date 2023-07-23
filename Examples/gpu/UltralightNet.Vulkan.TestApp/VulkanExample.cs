//namespace VulkanExample;

using System.Diagnostics;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;
using Silk.NET.Windowing;

using Application app = new();
app.Run();

internal unsafe partial class Application : IDisposable
{
	readonly Stopwatch stopwatch = Stopwatch.StartNew();

	readonly IWindow window = Window.Create(WindowOptions.DefaultVulkan);
	readonly Vk vk = Vk.GetApi();
	readonly Instance instance;

	readonly KhrSurface khrSurface;
	readonly ExtDebugUtils? debugUtils;

	readonly SurfaceKHR surface;
	readonly PhysicalDevice physicalDevice;

	readonly uint graphicsQueueFamily = uint.MaxValue;
	readonly uint presentQueueFamily = uint.MaxValue;

	readonly Device device;
	readonly Queue graphicsQueue;
	readonly Queue presentQueue;
	readonly KhrSwapchain khrSwapchain;

	readonly SampleCountFlags sampleCountFlags = SampleCountFlags.SampleCount4Bit;
	readonly SurfaceFormatKHR surfaceFormat = new(Format.B8G8R8A8Srgb, ColorSpaceKHR.ColorSpaceSrgbNonlinearKhr);
	readonly PresentModeKHR presentMode = PresentModeKHR.PresentModeFifoKhr;

	//readonly Sampler sampler;
	readonly DescriptorSetLayout descriptorSetLayout;
	readonly PipelineLayout pipelineLayout;
	readonly RenderPass renderPass;
	readonly Pipeline pipeline;

	public Application()
	{
		{ // Window
			window.Initialize();
			if (window.VkSurface is null) throw new PlatformNotSupportedException("Vulkan surface not found.");
		}
		{ // Instance
			var extensions = SilkMarshal.PtrToStringArray((nint)window.VkSurface.GetRequiredExtensions(out uint surfaceExtensionCount), (int)surfaceExtensionCount).ToList();
			extensions.Add(KhrSurface.ExtensionName);
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
				else vk.GetDeviceQueue(device, presentQueueFamily, 0, out presentQueue);

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
			khrSurface.GetPhysicalDeviceSurfaceCapabilities(physicalDevice, surface, out SurfaceCapabilitiesKHR surfaceCapabilitiesKHR);

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
				var pipelineLayoutCreateInfo = new PipelineLayoutCreateInfo(setLayoutCount: 1, pSetLayouts: pDescriptorSetLayout);
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
				vk.CreateGraphicsPipelines(device, default /* TODO: PipelineCache */, 1, &graphicsPipelineCreateInfo, null, out pipeline);
			}
			finally
			{
				vk.DestroyShaderModule(device, frag, null);
				vk.DestroyShaderModule(device, vert, null);
			}
		}
		Console.WriteLine($"Initialized Application in {stopwatch.Elapsed}");
	}



	public void Run()
	{

	}

	void IDisposable.Dispose()
	{
		vk.DestroyPipeline(device, pipeline, null);
		vk.DestroyRenderPass(device, renderPass, null);
		vk.DestroyPipelineLayout(device, pipelineLayout, null);
		vk.DestroyDescriptorSetLayout(device, descriptorSetLayout, null);
		//vk.DestroySampler(device, sampler, null);
		vk.DestroyDevice(device, null);
		khrSurface.DestroySurface(instance, surface, null);
		vk.DestroyInstance(instance, null);
		vk.Dispose();
		window.Dispose();
	}
}
