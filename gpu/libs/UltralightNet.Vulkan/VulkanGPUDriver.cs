using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using UltralightNet.GPUCommon;
using UltralightNet.Platform;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace UltralightNet.GPU.Vulkan;

public unsafe sealed class VulkanGPUDriver : IGPUDriver, IDisposable
{
	readonly Vk vk;
	readonly PhysicalDevice physicalDevice;
	readonly Device device;
	readonly ExtDebugUtils? debugUtils;

	uint FramesInFlight { get; }
	readonly bool UMA = false;
	SampleCountFlags SampleCount { get; }
	bool MSAA => SampleCount != SampleCountFlags.Count1Bit;

	readonly ResourceList<int> textures = new();
	readonly ResourceList<int> renderBuffers = new();
	readonly ResourceList<GeometryEntry> geometries = new();

	readonly DestroyQueue destroyQueue = new();
	readonly Allocator allocator;

	readonly Sampler sampler;

	readonly DescriptorSetLayout uniformBufferDescriptorSetLayout;
	readonly DescriptorSetAllocator uniformBufferDescriptorSetAllocator;

	readonly DescriptorSetLayout textureDescriptorSetLayout;
	readonly DescriptorSetAllocator textureDescriptorSetAllocator;

	readonly PipelineLayout fillPipelineLayout;
	readonly PipelineLayout pathPipelineLayout;
	readonly RenderPass renderPass;
	readonly Pipeline fillPipeline;
	readonly Pipeline pathPipeline;


	public uint CurrentFrame { get; set; }

	CommandBuffer commandBuffer;

	public VulkanGPUDriver(Vk vk, PhysicalDevice physicalDevice, Device device, uint framesInFlight, SampleCountFlags sampleCount = SampleCountFlags.Count4Bit)
	{
		this.vk = vk;
		this.physicalDevice = physicalDevice;
		this.device = device;
		vk.TryGetInstanceExtension(vk.CurrentInstance!.Value, out debugUtils);
		FramesInFlight = framesInFlight;
		SampleCount = sampleCount;

		allocator = new(vk, device, vk.GetPhysicalDeviceMemoryProperty(physicalDevice));

		{ // Sampler
			var samplerCreateInfo = new SamplerCreateInfo(
				magFilter: Filter.Nearest,
				minFilter: Filter.Linear,
				mipmapMode: SamplerMipmapMode.Nearest,
				addressModeU: SamplerAddressMode.ClampToEdge,
				addressModeV: SamplerAddressMode.ClampToEdge
				// TODO: Anisotropy enable flag
				// TODO: mipmaps
				);
			vk.CreateSampler(device, &samplerCreateInfo, null, out sampler).Check();
		}
		{
			Sampler* immutableSamplers = stackalloc Sampler[2] { sampler, default };

			{ // uniformBufferDescriptorSet___
				var descriptorSetLayoutBinding = new DescriptorSetLayoutBinding(0, DescriptorType.UniformBufferDynamic, 1, ShaderStageFlags.ShaderStageVertexBit | ShaderStageFlags.ShaderStageFragmentBit, immutableSamplers);
				var descriptorSetLayoutCreateInfo = new DescriptorSetLayoutCreateInfo(bindingCount: 1, pBindings: &descriptorSetLayoutBinding);
				vk.CreateDescriptorSetLayout(device, &descriptorSetLayoutCreateInfo, null, out uniformBufferDescriptorSetLayout).Check();

				uniformBufferDescriptorSetAllocator = new(vk, device, new[] { new DescriptorPoolSize(DescriptorType.UniformBufferDynamic, 1) }, uniformBufferDescriptorSetLayout);
			}
			{ // textureDescriptorSet___
				var descriptorSetLayoutBinding = new DescriptorSetLayoutBinding(0, DescriptorType.CombinedImageSampler, 1, ShaderStageFlags.ShaderStageFragmentBit, immutableSamplers);
				var descriptorSetLayoutCreateInfo = new DescriptorSetLayoutCreateInfo(bindingCount: 1, pBindings: &descriptorSetLayoutBinding);
				vk.CreateDescriptorSetLayout(device, &descriptorSetLayoutCreateInfo, null, out textureDescriptorSetLayout).Check();

				textureDescriptorSetAllocator = new(vk, device, new[] { new DescriptorPoolSize(DescriptorType.CombinedImageSampler, 1) }, textureDescriptorSetLayout);
			}
		}
		{ // PipelineLayout
			var descriptorSetLayouts = stackalloc DescriptorSetLayout[3] { uniformBufferDescriptorSetLayout, textureDescriptorSetLayout, textureDescriptorSetLayout };
			var pipelineLayoutCreateInfo = new PipelineLayoutCreateInfo(setLayoutCount: 3, pSetLayouts: descriptorSetLayouts);

			vk.CreatePipelineLayout(device, &pipelineLayoutCreateInfo, null, out fillPipelineLayout).Check();
			pipelineLayoutCreateInfo.SetLayoutCount = 1;
			vk.CreatePipelineLayout(device, &pipelineLayoutCreateInfo, null, out pathPipelineLayout).Check();
		}
		{ // RenderPass
			AttachmentDescription colorAttachmentDescription;
			AttachmentDescription resolveAttachmentDescription = default;

			SubpassDependency* subpassDependencies = stackalloc SubpassDependency[2];

			if (MSAA)
			{
				colorAttachmentDescription = new(
					format: Format.B8G8R8A8Unorm,
					samples: SampleCount,
					loadOp: AttachmentLoadOp.Load,
					storeOp: AttachmentStoreOp.Store,
					stencilLoadOp: AttachmentLoadOp.DontCare,
					stencilStoreOp: AttachmentStoreOp.DontCare,
					initialLayout: ImageLayout.ColorAttachmentOptimal,
					finalLayout: ImageLayout.ColorAttachmentOptimal
				);
				resolveAttachmentDescription = new(
					format: Format.B8G8R8A8Unorm,
					samples: SampleCountFlags.Count1Bit,
					loadOp: AttachmentLoadOp.DontCare,
					storeOp: AttachmentStoreOp.Store,
					stencilLoadOp: AttachmentLoadOp.DontCare,
					stencilStoreOp: AttachmentStoreOp.DontCare,
					initialLayout: ImageLayout.ShaderReadOnlyOptimal,
					finalLayout: ImageLayout.ShaderReadOnlyOptimal
				);

				subpassDependencies[0] = new SubpassDependency(
					Vk.SubpassExternal, 0,
					PipelineStageFlags.ColorAttachmentOutputBit, PipelineStageFlags.ColorAttachmentOutputBit,
					AccessFlags.ColorAttachmentReadBit | AccessFlags.ColorAttachmentWriteBit, AccessFlags.ColorAttachmentReadBit | AccessFlags.ColorAttachmentWriteBit);
			}
			else
			{
				colorAttachmentDescription = new(
					format: Format.B8G8R8A8Unorm,
					samples: SampleCountFlags.Count1Bit,
					loadOp: AttachmentLoadOp.Load,
					storeOp: AttachmentStoreOp.Store,
					stencilLoadOp: AttachmentLoadOp.DontCare,
					stencilStoreOp: AttachmentStoreOp.DontCare,
					initialLayout: ImageLayout.ShaderReadOnlyOptimal,
					finalLayout: ImageLayout.ShaderReadOnlyOptimal
				);

				subpassDependencies[0] = new SubpassDependency(
					Vk.SubpassExternal, 0,
					PipelineStageFlags.FragmentShaderBit, PipelineStageFlags.ColorAttachmentOutputBit,
					AccessFlags.ShaderReadBit, AccessFlags.ColorAttachmentReadBit | AccessFlags.ColorAttachmentWriteBit);
				subpassDependencies[1] = new SubpassDependency(
					0, Vk.SubpassExternal,
					PipelineStageFlags.ColorAttachmentOutputBit, PipelineStageFlags.FragmentShaderBit,
					AccessFlags.ColorAttachmentReadBit | AccessFlags.ColorAttachmentWriteBit, AccessFlags.ShaderReadBit);
			}

			AttachmentDescription* attachments = stackalloc[] { colorAttachmentDescription, resolveAttachmentDescription };

			var colorAttachmentReference = new AttachmentReference(0, ImageLayout.ColorAttachmentOptimal);
			var resolveAttachmentReference = new AttachmentReference(1, ImageLayout.ColorAttachmentOptimal);

			var subpassDescription = new SubpassDescription(
				pipelineBindPoint: PipelineBindPoint.Graphics,
				inputAttachmentCount: 0, pInputAttachments: null,
				colorAttachmentCount: 1, pColorAttachments: &colorAttachmentReference,
				pResolveAttachments: MSAA ? &resolveAttachmentReference : null
			);

			var renderPassCreateInfo = new RenderPassCreateInfo(
				attachmentCount: 2, pAttachments: attachments,
				subpassCount: 1, pSubpasses: &subpassDescription,
				dependencyCount: MSAA ? 1U : 2U, pDependencies: subpassDependencies
			);
			vk.CreateRenderPass(device, &renderPassCreateInfo, null, out renderPass).Check();
		}
		{ // Pipelines
			StackDisposable<ShaderModule> CreateShaderModule(UnmanagedMemoryStream stream)
			{
				var shaderModuleCreateInfo = new ShaderModuleCreateInfo(codeSize: (nuint)stream.Length, pCode: (uint*)stream.PositionPointer);
				ShaderModule shaderModule;
				vk.CreateShaderModule(device, &shaderModuleCreateInfo, null, &shaderModule).Check();
				return new(shaderModule, (shaderModule) => vk.DestroyShaderModule(device, shaderModule, null));
			}

			// Fill
			using var fillVertexShader = CreateShaderModule(typeof(VulkanGPUDriver).Assembly.GetManifestResourceStream("UltralightNet.Vulkan.shader_fill.vert.spv") as UnmanagedMemoryStream ?? throw new Exception("Shaders not found."));
			using var fillFragmentShader = CreateShaderModule(typeof(VulkanGPUDriver).Assembly.GetManifestResourceStream("UltralightNet.Vulkan.shader_fill.frag.spv") as UnmanagedMemoryStream ?? throw new Exception("Shaders not found."));

			debugUtils?.SetDebugUtilsObjectName(device, new DebugUtilsObjectNameInfoEXT(objectType: ObjectType.ShaderModule, objectHandle: fillVertexShader.Value.Handle, pObjectName: "Ultralight Fill Vertex Shader"u8.AsPointer()));
			debugUtils?.SetDebugUtilsObjectName(device, new DebugUtilsObjectNameInfoEXT(objectType: ObjectType.ShaderModule, objectHandle: fillFragmentShader.Value.Handle, pObjectName: "Ultralight Fill Fragment Shader"u8.AsPointer()));


			var pipelineShaderStages = stackalloc PipelineShaderStageCreateInfo[2]{
				new(stage: ShaderStageFlags.VertexBit, module: fillVertexShader.Value, pName: "main"u8.AsPointer()),
				new(stage: ShaderStageFlags.FragmentBit, module: fillFragmentShader.Value, pName: "main"u8.AsPointer()),
			};

			var vertexInputBindingDescription = new VertexInputBindingDescription(0, 140, VertexInputRate.Vertex);
			var vertexInputAttributeDescriptions = stackalloc VertexInputAttributeDescription[11]{
				new(0, 0, Format.R32G32Sfloat, 0),
				new(1, 0, Format.R8G8B8A8Unorm, 8),
				new(2, 0, Format.R32G32Sfloat, 12),
				new(3, 0, Format.R32G32Sfloat, 20),
				new(4, 0, Format.R32G32B32A32Sfloat, 28),
				new(5, 0, Format.R32G32B32A32Sfloat, 44),
				new(6, 0, Format.R32G32B32A32Sfloat, 60),
				new(7, 0, Format.R32G32B32A32Sfloat, 76),
				new(8, 0, Format.R32G32B32A32Sfloat, 92),
				new(9, 0, Format.R32G32B32A32Sfloat, 108),
				new(10, 0, Format.R32G32B32A32Sfloat, 124)
			};
			var pipelineVertexInputStateCreateInfo = new PipelineVertexInputStateCreateInfo(
				vertexBindingDescriptionCount: 1, pVertexBindingDescriptions: &vertexInputBindingDescription,
				vertexAttributeDescriptionCount: 11, pVertexAttributeDescriptions: vertexInputAttributeDescriptions);
			var pipelineInputAssemblyStateCreateInfo = new PipelineInputAssemblyStateCreateInfo(topology: PrimitiveTopology.TriangleList);
			var pipelineViewportStateCreateInfo = new PipelineViewportStateCreateInfo(viewportCount: 1, scissorCount: 1);
			var pipelineRasterizationStateCreateInfo = new PipelineRasterizationStateCreateInfo(polygonMode: PolygonMode.Fill, cullMode: CullModeFlags.BackBit, frontFace: FrontFace.CounterClockwise, lineWidth: 1.0f);
			var pipelineMultisampleStateCreateInfo = new PipelineMultisampleStateCreateInfo(rasterizationSamples: SampleCount);
			var pipelineDepthStencilStateCreateInfo = new PipelineDepthStencilStateCreateInfo(depthTestEnable: false);
			var pipelineColorBlendAttachmentState = new PipelineColorBlendAttachmentState(
				blendEnable: true,
				BlendFactor.One, BlendFactor.OneMinusSrcAlpha, // COLOR
				BlendOp.Add,
				BlendFactor.OneMinusSrcAlpha, BlendFactor.One, // ALPHA
				BlendOp.Add,
				colorWriteMask: ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit);
			var pipelineColorBlendStateCreateInfo = new PipelineColorBlendStateCreateInfo(attachmentCount: 1, pAttachments: &pipelineColorBlendAttachmentState);
			var dynamicStates = stackalloc DynamicState[] { DynamicState.Viewport, DynamicState.Scissor };
			var pipelineDynamicStateCreateInfo = new PipelineDynamicStateCreateInfo(dynamicStateCount: 2, pDynamicStates: dynamicStates);
			var graphicsPipelineCreateInfo = new GraphicsPipelineCreateInfo(
				stageCount: 2, pStages: pipelineShaderStages,
				pVertexInputState: &pipelineVertexInputStateCreateInfo,
				pInputAssemblyState: &pipelineInputAssemblyStateCreateInfo,
				pViewportState: &pipelineViewportStateCreateInfo,
				pRasterizationState: &pipelineRasterizationStateCreateInfo,
				pMultisampleState: &pipelineMultisampleStateCreateInfo,
				pDepthStencilState: &pipelineDepthStencilStateCreateInfo,
				pColorBlendState: &pipelineColorBlendStateCreateInfo,
				pDynamicState: &pipelineDynamicStateCreateInfo,
				layout: fillPipelineLayout, renderPass: renderPass);
			vk.CreateGraphicsPipelines(device, default /* TODO: PipelineCache */, 1, &graphicsPipelineCreateInfo, null, out fillPipeline).Check();

			debugUtils?.SetDebugUtilsObjectName(device, new DebugUtilsObjectNameInfoEXT(objectType: ObjectType.Pipeline, objectHandle: fillPipeline.Handle, pObjectName: "Ultralight Fill Pipeline"u8.AsPointer()));

			// Path
			using var pathVertexShader = CreateShaderModule(typeof(VulkanGPUDriver).Assembly.GetManifestResourceStream("UltralightNet.Vulkan.shader_fill_path.vert.spv") as UnmanagedMemoryStream ?? throw new Exception("Shaders not found."));
			using var pathFragmentShader = CreateShaderModule(typeof(VulkanGPUDriver).Assembly.GetManifestResourceStream("UltralightNet.Vulkan.shader_fill_path.frag.spv") as UnmanagedMemoryStream ?? throw new Exception("Shaders not found."));

			debugUtils?.SetDebugUtilsObjectName(device, new DebugUtilsObjectNameInfoEXT(objectType: ObjectType.ShaderModule, objectHandle: pathVertexShader.Value.Handle, pObjectName: "Ultralight Path Vertex Shader"u8.AsPointer()));
			debugUtils?.SetDebugUtilsObjectName(device, new DebugUtilsObjectNameInfoEXT(objectType: ObjectType.ShaderModule, objectHandle: pathFragmentShader.Value.Handle, pObjectName: "Ultralight Path Fragment Shader"u8.AsPointer()));

			pipelineShaderStages[0].Module = pathVertexShader.Value;
			pipelineShaderStages[1].Module = pathFragmentShader.Value;

			vertexInputBindingDescription.Stride = 20;
			pipelineVertexInputStateCreateInfo.VertexAttributeDescriptionCount = 3;
			vk.CreateGraphicsPipelines(device, default /* TODO: PipelineCache */, 1, &graphicsPipelineCreateInfo, null, out pathPipeline).Check();

			debugUtils?.SetDebugUtilsObjectName(device, new DebugUtilsObjectNameInfoEXT(objectType: ObjectType.Pipeline, objectHandle: pathPipeline.Handle, pObjectName: "Ultralight Path Pipeline"u8.AsPointer()));
		}
	}

	ref struct StackDisposable<T>(T value, Action<T> disposer)
	{
		public readonly T Value
		{
			get
			{
				if (IsDisposed) throw new ObjectDisposedException(nameof(StackDisposable<T>));
				return value;
			}
		}
		public bool IsDisposed { get; private set; }

		public void Dispose()
		{
			if (IsDisposed) return;
			disposer(Value);
			IsDisposed = true;
		}
	}

	public bool IsDisposed { get; private set; }
	// Call only AFTER all commands are completed.
	public void Dispose()
	{
		if (IsDisposed) return;

		vk.DestroyPipeline(device, pathPipeline, null);
		vk.DestroyPipeline(device, fillPipeline, null);

		vk.DestroyRenderPass(device, renderPass, null);

		vk.DestroyPipelineLayout(device, pathPipelineLayout, null);
		vk.DestroyPipelineLayout(device, fillPipelineLayout, null);

		textureDescriptorSetAllocator.Dispose();
		vk.DestroyDescriptorSetLayout(device, textureDescriptorSetLayout, null);

		uniformBufferDescriptorSetAllocator.Dispose();
		vk.DestroyDescriptorSetLayout(device, uniformBufferDescriptorSetLayout, null);

		vk.DestroySampler(device, sampler, null);

		allocator.Dispose();
		destroyQueue.Dispose();

		textures.Dispose();
		renderBuffers.Dispose();
		geometries.Dispose();

		debugUtils?.Dispose();

		IsDisposed = true;
		//GC.SuppressFinalize(this);
	}
	//~VulkanGPUDriver() => Dispose();



	void IGPUDriver.CreateTexture(uint textureId, ULBitmap bitmap)
	{
		throw new NotImplementedException();
	}
	void IGPUDriver.UpdateTexture(uint textureId, ULBitmap bitmap)
	{
		throw new NotImplementedException();
	}
	void IGPUDriver.DestroyTexture(uint textureId)
	{
		throw new NotImplementedException();
	}

	void IGPUDriver.CreateRenderBuffer(uint renderBufferId, ULRenderBuffer renderBuffer)
	{
		throw new NotImplementedException();
	}
	void IGPUDriver.DestroyRenderBuffer(uint renderBufferId)
	{
		throw new NotImplementedException();
	}

	void IGPUDriver.CreateGeometry(uint geometryId, ULVertexBuffer vertexBuffer, ULIndexBuffer indexBuffer)
	{
		ref var g = ref geometries[(int)geometryId];

		Debug.Assert(vertexBuffer.size % 256 == 0, "nonCoherentAtomSize");
		Debug.Assert(indexBuffer.size % 256 == 0, "nonCoherentAtomSize");

		Buffer sharedDeviceBuffer = default;
		DeviceMemory sharedDeviceMemory = default;

		if (!UMA)
			allocator.CreateBuffer(
				vertexBuffer.size + indexBuffer.size,
				BufferUsageFlags.VertexBufferBit | BufferUsageFlags.IndexBufferBit | BufferUsageFlags.TransferDstBit,
				MemoryPropertyFlags.DeviceLocalBit,
				out sharedDeviceBuffer, out sharedDeviceMemory);

		allocator.CreateBuffer(
			(vertexBuffer.size + indexBuffer.size) * FramesInFlight,
			!UMA ? BufferUsageFlags.TransferSrcBit : BufferUsageFlags.VertexBufferBit | BufferUsageFlags.IndexBufferBit,
			MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit,
			out var sharedHostBuffer, out var sharedHostMemory);

		byte* mapped;
		vk.MapMemory(device, sharedHostMemory, 0, (vertexBuffer.size + indexBuffer.size) * FramesInFlight, 0, (void**)&mapped).Check();

		g = new()
		{
			Buffer = !UMA ? sharedDeviceBuffer : sharedHostBuffer,
			HostBuffer = sharedHostBuffer,
			Index = (0UL, (ulong)indexBuffer.size),
			Vertex = (!UMA ? (ulong)indexBuffer.size : (ulong)indexBuffer.size * (ulong)FramesInFlight, (ulong)vertexBuffer.size),
			HostMemory = sharedHostMemory,
			DeviceMemory = sharedDeviceMemory,
			Mapped = mapped
		};
		(this as IGPUDriver).UpdateGeometry(geometryId, vertexBuffer, indexBuffer);
	}
	void IGPUDriver.UpdateGeometry(uint geometryId, ULVertexBuffer vertexBuffer, ULIndexBuffer indexBuffer)
	{
		ref var g = ref geometries[(int)geometryId];

		g.GetBuffersToWriteTo(CurrentFrame, out var index, out var vertex);

		new Span<byte>(vertexBuffer.data, (int)vertexBuffer.size).CopyTo(vertex);
		new Span<byte>(indexBuffer.data, (int)indexBuffer.size).CopyTo(index);

		if (!UMA)
		{
			var bufferMemoryBarriers = stackalloc BufferMemoryBarrier[2] {
				new(
					srcAccessMask: AccessFlags.VertexAttributeReadBit, dstAccessMask: AccessFlags.TransferWriteBit,
					srcQueueFamilyIndex: Vk.QueueFamilyIgnored, dstQueueFamilyIndex: Vk.QueueFamilyIgnored,
					buffer: g.Buffer, offset: g.Vertex.offset, size: g.Vertex.size),
				new(
					srcAccessMask: AccessFlags.IndexReadBit, dstAccessMask: AccessFlags.TransferWriteBit,
					srcQueueFamilyIndex: Vk.QueueFamilyIgnored, dstQueueFamilyIndex: Vk.QueueFamilyIgnored,
					buffer: g.Buffer, offset: g.Index.offset, size: g.Index.size)
			};

			vk.CmdPipelineBarrier(commandBuffer,
				PipelineStageFlags.VertexInputBit,
				PipelineStageFlags.TransferBit,
				0,
				0, null,
				2, bufferMemoryBarriers,
				0, null);

			var bufferCopy = stackalloc BufferCopy[] {
				new(g.Index.size * CurrentFrame, 0, g.Index.size),
				new(g.Vertex.offset + (g.Vertex.size * CurrentFrame), g.Vertex.size)
			};
			vk.CmdCopyBuffer(commandBuffer, g.HostBuffer, g.Buffer, 2, bufferCopy);

			bufferMemoryBarriers[0].SrcAccessMask = AccessFlags.TransferWriteBit;
			bufferMemoryBarriers[0].DstAccessMask = AccessFlags.VertexAttributeReadBit;

			bufferMemoryBarriers[1].SrcAccessMask = AccessFlags.TransferWriteBit;
			bufferMemoryBarriers[1].DstAccessMask = AccessFlags.IndexReadBit;

			vk.CmdPipelineBarrier(commandBuffer,
				PipelineStageFlags.TransferBit,
				PipelineStageFlags.VertexInputBit,
				0,
				0, null,
				2, bufferMemoryBarriers,
				0, null);
		}
	}
	void IGPUDriver.DestroyGeometry(uint geometryId)
	{
		var geo = geometries[(int)geometryId];

		vk.UnmapMemory(device, geo.HostMemory);

		destroyQueue.Enqueue(CurrentFrame, () =>
		{
			vk.DestroyBuffer(device, geo.Buffer, null);
			vk.FreeMemory(device, geo.DeviceMemory, null);
			if (!UMA)
			{
				vk.DestroyBuffer(device, geo.HostBuffer, null);
				vk.FreeMemory(device, geo.HostMemory, null);
			}
		});

		geometries.Remove((int)geometryId);
	}

	void IGPUDriver.UpdateCommandList(ULCommandList commandList)
	{
		foreach (var command in commandList.AsSpan())
		{
			if (command.CommandType is ULCommandType.ClearRenderBuffer)
			{

			}
			else
			{
				ref var geo = ref geometries[(int)command.GeometryId];
				var indexBuffer = geo.GetIndexBufferToUse(UMA);
				var vertexBuffer = geo.GetVertexBufferToUse(UMA);
				vk.CmdBindIndexBuffer(commandBuffer, indexBuffer.buffer, indexBuffer.offset, IndexType.Uint32);
				vk.CmdBindVertexBuffers(commandBuffer, 0, 1, vertexBuffer.buffer, vertexBuffer.offset);
				vk.CmdDrawIndexed(commandBuffer, command.IndicesCount, 1, command.IndicesOffset, 0, 0);
			}
		}
	}

	uint IGPUDriver.NextTextureId() => (uint)textures.GetNewId();
	uint IGPUDriver.NextRenderBufferId() => (uint)renderBuffers.GetNewId();
	uint IGPUDriver.NextGeometryId() => (uint)geometries.GetNewId();

	public void BeginSynchronize()
	{
		var commandBufferBeginInfo = new CommandBufferBeginInfo(flags: CommandBufferUsageFlags.OneTimeSubmitBit);
		vk.BeginCommandBuffer(commandBuffer, &commandBufferBeginInfo).Check();
	}
	public void EndSynchronize()
	{
		vk.EndCommandBuffer(commandBuffer).Check();
	}

	[StructLayout(LayoutKind.Auto, Pack = 8)]
	unsafe struct GeometryEntry
	{
		public Buffer Buffer { readonly get; init; }
		public Buffer HostBuffer { readonly get; init; }

		public (ulong offset, ulong size) Index { readonly get; init; }
		public (ulong offset, ulong size) Vertex { readonly get; init; }

		public DeviceMemory HostMemory { readonly get; init; }
		public DeviceMemory DeviceMemory { readonly get; init; }

		public byte* Mapped { readonly get; init; }

		public uint latestFrame;

		public readonly (Buffer buffer, ulong offset) GetIndexBufferToUse(bool UMA) => (Buffer, !UMA ? 0 : Index.size * latestFrame);
		public readonly (Buffer buffer, ulong offset) GetVertexBufferToUse(bool UMA) => (Buffer, !UMA ? Index.size : Vertex.offset + (Vertex.size * latestFrame));

		public void GetBuffersToWriteTo(uint frame, out Span<byte> index, out Span<byte> vertex)
		{
			index = new Span<byte>(Mapped + (Index.size * (latestFrame = frame)), (int)Index.size);
			vertex = new Span<byte>(Mapped + (Vertex.offset + (Vertex.size * latestFrame)), (int)Vertex.size);
		}
	}
}
