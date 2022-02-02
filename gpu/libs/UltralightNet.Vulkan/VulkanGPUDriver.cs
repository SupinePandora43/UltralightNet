namespace UltralightNet.Vulkan;

using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;
using UltralightNet;
using UltralightNet.GPUCommon;
using Buffer = Silk.NET.Vulkan.Buffer;

// https://github.com/SaschaWillems/Vulkan/blob/master/examples/offscreen/offscreen.cpp for reference

public class RenderBufferEntry
{
	public Framebuffer framebuffer;

	public TextureEntry textureEntry;
}

public class TextureEntry
{
	public uint width;
	public uint height;

	public Image image;
	public ImageView imageView;
	public DeviceMemory imageMemory;

	public DescriptorPool descriptorPool;
	public DescriptorSet descriptorSet;
}

internal class GeometryEntry
{
	public Buffer vertexStagingBuffer;
	public Buffer vertexBuffer;
	public Buffer indexStagingBuffer;
	public Buffer indexBuffer;

	public DeviceMemory vertexStagingMemory;
	public DeviceMemory vertexMemory;
	public DeviceMemory indexStagingMemory;
	public DeviceMemory indexMemory;
}

public unsafe partial class VulkanGPUDriver
{
	private const ulong UniformBufferSize = 768;

	private readonly Vk vk;
	private readonly PhysicalDevice physicalDevice; // TODO
	private readonly Device device;
	public CommandPool commandPool; // TODO

	public Queue graphicsQueue; // TODO
	private Queue transferQueue;

	private readonly Image pipelineImage;
	private readonly ImageView pipelineImageView;
	private readonly Framebuffer pipelineFramebuffer;
	private readonly RenderPass pipelineRenderPass;
	private readonly DescriptorSetLayout textureSetLayout;
	//private readonly DescriptorSetLayout uniformSetLayout;
	private readonly PipelineLayout fillPipelineLayout;
	private readonly Pipeline fillPipeline;
	private readonly Pipeline pathPipeline;
	private readonly DescriptorPool uniformDescriptorPool;
	private readonly DescriptorSet uniformSet;
	private readonly DeviceMemory uniformBufferMemory;
	private readonly Buffer uniformBuffer;
	private readonly Sampler textureSampler;
	// TODO: implement MSAA
	private const SampleCountFlags msaaSamples = SampleCountFlags.SampleCount4Bit;

	private readonly List<TextureEntry> textures = new();
	private readonly Queue<uint> texturesFreeIds = new();
	private readonly List<GeometryEntry> geometries = new();
	private readonly Queue<uint> geometriesFreeIds = new();
	private readonly List<RenderBufferEntry> renderBuffers = new();
	private readonly Queue<uint> renderBuffersFreeIds = new();

	public VulkanGPUDriver(Vk vk, PhysicalDevice physicalDevice, Device device)
	{
		this.vk = vk;
		this.physicalDevice = physicalDevice;
		this.device = device;

		geometries.Add(new()); // id => 1 workaround
		textures.Add(new()); // id => 1 workaround
		renderBuffers.Add(new()); // id => 1 workaround

		#region TextureSetLayout
		DescriptorSetLayoutBinding samplerLayoutBinding = new()
		{
			Binding = 0,
			DescriptorCount = 1,
			DescriptorType = DescriptorType.CombinedImageSampler,
			PImmutableSamplers = null,
			StageFlags = ShaderStageFlags.ShaderStageFragmentBit,
		};

		fixed (DescriptorSetLayout* textureSetLayoutPtr = &textureSetLayout)
		{
			DescriptorSetLayoutCreateInfo layoutInfo = new()
			{
				SType = StructureType.DescriptorSetLayoutCreateInfo,
				BindingCount = 1,
				PBindings = &samplerLayoutBinding,
			};

			if (vk.CreateDescriptorSetLayout(device, layoutInfo, null, textureSetLayoutPtr) != Result.Success)
			{
				throw new Exception("failed to create descriptor set layout!");
			}
		}
		#endregion TextureSetLayout
		#region TextureSampler
		SamplerCreateInfo samplerInfo = new()
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
			MipLodBias = 0,
		};

		fixed (Sampler* textureSamplerPtr = &textureSampler)
		{
			if (vk.CreateSampler(device, samplerInfo, null, textureSamplerPtr) != Result.Success)
			{
				throw new Exception("failed to create texture sampler!");
			}
		}
		#endregion TextureSampler
		#region RenderPass
		{
			AttachmentDescription colorAttachment = new()
			{
				Format = Format.B8G8R8A8Unorm,
				Samples = SampleCountFlags.SampleCount1Bit,
				LoadOp = AttachmentLoadOp.Load,
				StoreOp = AttachmentStoreOp.Store,
				StencilLoadOp = AttachmentLoadOp.DontCare,
				StencilStoreOp = AttachmentStoreOp.DontCare,
				InitialLayout = ImageLayout.ShaderReadOnlyOptimal,
				FinalLayout = ImageLayout.ShaderReadOnlyOptimal,
			};

			AttachmentReference colorAttachmentRef = new()
			{
				Attachment = 0,
				Layout = ImageLayout.ColorAttachmentOptimal,
			};

			SubpassDescription subpass = new()
			{
				PipelineBindPoint = PipelineBindPoint.Graphics,
				ColorAttachmentCount = 1,
				PColorAttachments = &colorAttachmentRef,
				//PResolveAttachments = &colorAttachmentRef
			};

			SubpassDependency dependency = new()
			{
				SrcSubpass = Vk.SubpassExternal,
				DstSubpass = 0,
				SrcStageMask = PipelineStageFlags.PipelineStageFragmentShaderBit,
				SrcAccessMask = AccessFlags.AccessShaderReadBit,
				DstStageMask = PipelineStageFlags.PipelineStageColorAttachmentOutputBit,
				DstAccessMask = AccessFlags.AccessColorAttachmentWriteBit
			};

			RenderPassCreateInfo renderPassInfo = new()
			{
				SType = StructureType.RenderPassCreateInfo,
				AttachmentCount = 1,
				PAttachments = &colorAttachment,
				SubpassCount = 1,
				PSubpasses = &subpass,
				DependencyCount = 1,
				PDependencies = &dependency
			};

			if (vk.CreateRenderPass(device, renderPassInfo, null, out pipelineRenderPass) is not Result.Success)
			{
				throw new Exception("failed to create render pass!");
			}
		}
		#endregion RenderPass
		DescriptorSetLayout uniformSetLayout;
		#region UnfiromSetLayout
		{
			DescriptorSetLayoutBinding uniformLayoutBinding = new()
			{
				Binding = 0,
				DescriptorCount = 1,
				DescriptorType = DescriptorType.UniformBuffer,
				PImmutableSamplers = null,
				StageFlags = ShaderStageFlags.ShaderStageVertexBit | ShaderStageFlags.ShaderStageFragmentBit
			};


			DescriptorSetLayoutCreateInfo layoutInfo = new()
			{
				SType = StructureType.DescriptorSetLayoutCreateInfo,
				BindingCount = 1,
				PBindings = &uniformLayoutBinding,
			};
			if (vk.CreateDescriptorSetLayout(device, layoutInfo, null, &uniformSetLayout) != Result.Success)
			{
				throw new Exception("failed to create descriptor set layout!");
			}
			//this.uniformSetLayout = uniformSetLayout;
		}
		#endregion UniformSetLayout
		#region FillPipeline
		{
			VertexInputBindingDescription vertexInputBindingDescription = new()
			{
				Binding = 0,
				Stride = 140,
				InputRate = VertexInputRate.Vertex,
			};
			VertexInputAttributeDescription[] vertexInputAttributeDescriptions = new VertexInputAttributeDescription[]{
				new (){
					Binding = 0,
					Location = 0,
					Format = Format.R32G32Sfloat,
					Offset = 0
				}, new(){
					Binding = 0,
					Location = 1,
					Format = Format.R8G8B8A8Unorm,
					Offset = 8
				}, new(){
					Binding = 0,
					Location = 2,
					Format = Format.R32G32Sfloat,
					Offset = 12
				}, new(){ // in_ObjCoord
					Binding = 0,
					Location = 3,
					Format = Format.R32G32Sfloat,
					Offset = 20
				}, new(){ // in_Data0
					Binding = 0,
					Location = 4,
					Format = Format.R32G32B32A32Sfloat,
					Offset = 28
				}, new(){ // in_Data1
					Binding = 0,
					Location = 5,
					Format = Format.R32G32B32A32Sfloat,
					Offset = 44
				}, new(){ // in_Data2
					Binding = 0,
					Location = 6,
					Format = Format.R32G32B32A32Sfloat,
					Offset = 60
				}, new(){ // in_Data3
					Binding = 0,
					Location = 7,
					Format = Format.R32G32B32A32Sfloat,
					Offset = 76
				}, new(){ // in_Data4
					Binding = 0,
					Location = 8,
					Format = Format.R32G32B32A32Sfloat,
					Offset = 92
				}, new(){ // in_Data5
					Binding = 0,
					Location = 9,
					Format = Format.R32G32B32A32Sfloat,
					Offset = 108
				}, new(){ // in_Data6
					Binding = 0,
					Location = 10,
					Format = Format.R32G32B32A32Sfloat,
					Offset = 124
				}
			};

			fixed (VertexInputAttributeDescription* vertexInputAttributeDescriptionsPtr = vertexInputAttributeDescriptions)
			{
				var pipelineVertexInputStateCreateInfo = new PipelineVertexInputStateCreateInfo()
				{
					SType = StructureType.PipelineVertexInputStateCreateInfo,
					VertexBindingDescriptionCount = 1,
					PVertexBindingDescriptions = &vertexInputBindingDescription,
					PVertexAttributeDescriptions = vertexInputAttributeDescriptionsPtr,
					VertexAttributeDescriptionCount = (uint)vertexInputAttributeDescriptions.Length
				};

				var pipelineInputAssemblyStateCreateInfo = new PipelineInputAssemblyStateCreateInfo()
				{
					SType = StructureType.PipelineInputAssemblyStateCreateInfo,
					Topology = PrimitiveTopology.TriangleList,
					PrimitiveRestartEnable = false
				};

				var viewport = new Viewport()
				{
					X = 0,
					Y = 0,
					Width = 512,
					Height = 512,
					MinDepth = 0,
					MaxDepth = 1,
				};

				var scissor = new Rect2D()
				{
					Offset = { X = 0, Y = 0 },
					Extent = new(512, 512),
				};
				var pipelineViewportStateCreateInfo = new PipelineViewportStateCreateInfo()
				{
					SType = StructureType.PipelineViewportStateCreateInfo,
					ViewportCount = 1,
					PViewports = &viewport,
					ScissorCount = 1,
					PScissors = &scissor,
				};
				PipelineRasterizationStateCreateInfo rasterizer = new()
				{
					SType = StructureType.PipelineRasterizationStateCreateInfo,
					DepthClampEnable = false,
					RasterizerDiscardEnable = false,
					PolygonMode = PolygonMode.Fill,
					LineWidth = 1,
					CullMode = CullModeFlags.CullModeNone, // TODO
					FrontFace = FrontFace.CounterClockwise, // TODO
					DepthBiasEnable = false,
				};

				PipelineMultisampleStateCreateInfo multisampling = new()
				{
					SType = StructureType.PipelineMultisampleStateCreateInfo,
					SampleShadingEnable = false,
					RasterizationSamples = SampleCountFlags.SampleCount1Bit,
				};

				PipelineDepthStencilStateCreateInfo depthStencil = new()
				{
					SType = StructureType.PipelineDepthStencilStateCreateInfo,
					DepthTestEnable = true,
					DepthWriteEnable = true,
					DepthCompareOp = CompareOp.Less,
					DepthBoundsTestEnable = false,
					StencilTestEnable = false,
				};

				PipelineColorBlendAttachmentState colorBlendAttachment = new()
				{
					ColorWriteMask = ColorComponentFlags.ColorComponentRBit | ColorComponentFlags.ColorComponentGBit | ColorComponentFlags.ColorComponentBBit | ColorComponentFlags.ColorComponentABit,
					BlendEnable = true,
					ColorBlendOp = BlendOp.Add,
					AlphaBlendOp = BlendOp.Add,
					SrcAlphaBlendFactor = BlendFactor.One,
					DstAlphaBlendFactor = BlendFactor.OneMinusSrcAlpha,
					SrcColorBlendFactor = BlendFactor.One,
					DstColorBlendFactor = BlendFactor.OneMinusSrcAlpha
				};

				PipelineColorBlendStateCreateInfo colorBlending = new()
				{
					SType = StructureType.PipelineColorBlendStateCreateInfo,
					LogicOpEnable = false,
					LogicOp = LogicOp.Copy,
					AttachmentCount = 1,
					PAttachments = &colorBlendAttachment,
				};

				colorBlending.BlendConstants[0] = 0;
				colorBlending.BlendConstants[1] = 0;
				colorBlending.BlendConstants[2] = 0;
				colorBlending.BlendConstants[3] = 0;
				DescriptorSetLayout* sets = stackalloc DescriptorSetLayout[] { uniformSetLayout, textureSetLayout, textureSetLayout };
				PipelineLayoutCreateInfo pipelineLayoutInfo = new()
				{
					SType = StructureType.PipelineLayoutCreateInfo,
					PushConstantRangeCount = 0,
					SetLayoutCount = 3,
					PSetLayouts = sets
				};

				if (vk.CreatePipelineLayout(device, pipelineLayoutInfo, null, out fillPipelineLayout) is not Result.Success)
				{
					throw new Exception("failed to create pipeline layout!");
				}

				_shader_main = (byte*)SilkMarshal.StringToPtr("main");

				var shaderStages = stackalloc[] {
					LoadShader("UltralightNet.Vulkan.shader_fill.vert.spv", ShaderStageFlags.ShaderStageVertexBit),
					LoadShader("UltralightNet.Vulkan.shader_fill.frag.spv", ShaderStageFlags.ShaderStageFragmentBit)
				};
				var dynamicStateScissor = DynamicState.Scissor;
				var dynamicState = new PipelineDynamicStateCreateInfo(dynamicStateCount: 1, pDynamicStates: &dynamicStateScissor);

				GraphicsPipelineCreateInfo pipelineInfo = new()
				{
					SType = StructureType.GraphicsPipelineCreateInfo,
					StageCount = 2,
					PStages = shaderStages,
					PVertexInputState = &pipelineVertexInputStateCreateInfo,
					PInputAssemblyState = &pipelineInputAssemblyStateCreateInfo,
					PViewportState = &pipelineViewportStateCreateInfo,
					PRasterizationState = &rasterizer,
					PMultisampleState = &multisampling,
					PDepthStencilState = &depthStencil,
					PColorBlendState = &colorBlending,
					Layout = fillPipelineLayout,
					RenderPass = pipelineRenderPass,
					Subpass = 0,
					BasePipelineHandle = default,
					PDynamicState = &dynamicState
				};

				if (vk.CreateGraphicsPipelines(device, default, 1, pipelineInfo, null, out fillPipeline) is not Result.Success)
				{
					throw new Exception("failed to create graphics pipeline!");
				}

				SilkMarshal.Free((nint)_shader_main);
			}
		}
		#endregion FillPipeline
		#region UniformSet
		#region DescriptorPool
		var poolSize = new DescriptorPoolSize()
		{
			Type = DescriptorType.UniformBuffer,
			DescriptorCount = 1,
		};
		DescriptorPool uniformDescriptorPool;

		DescriptorPoolCreateInfo poolInfo = new()
		{
			SType = StructureType.DescriptorPoolCreateInfo,
			PoolSizeCount = 1,
			PPoolSizes = &poolSize,
			MaxSets = 1
		};

		if (vk.CreateDescriptorPool(device, poolInfo, null, &uniformDescriptorPool) is not Result.Success)
		{
			throw new Exception("failed to create descriptor pool!");
		}
		this.uniformDescriptorPool = uniformDescriptorPool;
		#endregion DescriptorPool
		#region DescriptorSet
		DescriptorSet uniformSet;

		DescriptorSetAllocateInfo allocateInfo = new()
		{
			SType = StructureType.DescriptorSetAllocateInfo,
			DescriptorPool = uniformDescriptorPool,
			DescriptorSetCount = 1,
			PSetLayouts = &uniformSetLayout
		};
		if (vk.AllocateDescriptorSets(device, allocateInfo, &uniformSet) is not Result.Success)
		{
			throw new Exception("failed to allocate descriptor sets!");
		}
		this.uniformSet = uniformSet;
		#endregion DescriptorSet
		#region Buffer
		CreateBuffer(UniformBufferSize, BufferUsageFlags.BufferUsageUniformBufferBit, MemoryPropertyFlags.MemoryPropertyHostVisibleBit | MemoryPropertyFlags.MemoryPropertyHostCoherentBit, out uniformBuffer, out uniformBufferMemory);
		#endregion Buffer
		#region DescriptorWrite
		var bufferInfo = new DescriptorBufferInfo()
		{
			Buffer = uniformBuffer,
			Offset = 0,
			Range = UniformBufferSize
		};
		var uniformWriteDescriptorSet = new WriteDescriptorSet()
		{
			SType = StructureType.WriteDescriptorSet,
			DstSet = uniformSet,
			DstBinding = 0,
			DstArrayElement = 0,
			DescriptorType = DescriptorType.UniformBuffer,
			DescriptorCount = 1,
			PBufferInfo = &bufferInfo,
		};
		vk.UpdateDescriptorSets(device, 1, &uniformWriteDescriptorSet, 0, null);
		#endregion
		#endregion UniformSet
		_gpuDriver = new()
		{
			NextTextureId = NextTextureId,
			CreateTexture = CreateTexture,
			UpdateTexture = UpdateTexture,
			DestroyTexture = DestroyTexture,
			NextRenderBufferId = NextRenderBufferId,
			CreateRenderBuffer = CreateRenderBuffer,
			DestroyRenderBuffer = DestroyRenderBuffer,
			NextGeometryId = NextGeometryId,
			CreateGeometry = CreateGeometry,
			UpdateGeometry = UpdateGeometry,
			DestroyGeometry = DestroyGeometry,
			UpdateCommandList = UpdateCommandList
		};
	}

	private ULGPUDriver _gpuDriver;
	public ULGPUDriver GPUDriver => _gpuDriver;

	public RenderBufferEntry GetRenderBuffer(uint id)
	{
		return renderBuffers[(int)id];
	}
	public TextureEntry GetTexture(uint id)
	{
		return textures[(int)id];
	}

	#region ID
	private uint NextGeometryId()
	{
		if (geometriesFreeIds.Count is not 0) return geometriesFreeIds.Dequeue();
		else
		{
			geometries.Add(new());
			return (uint)geometries.Count - 1;
		}
	}
	private uint NextTextureId()
	{
		if (texturesFreeIds.Count is not 0) return texturesFreeIds.Dequeue();
		else
		{
			textures.Add(new());
			return (uint)textures.Count - 1;
		}
	}
	private uint NextRenderBufferId()
	{
		if (renderBuffersFreeIds.Count is not 0) return renderBuffersFreeIds.Dequeue();
		else
		{
			renderBuffers.Add(new());
			return (uint)renderBuffers.Count - 1;
		}
	}
	#endregion ID

	//[SkipLocalsInit]
	private void CreateRenderBuffer(uint id, ULRenderBuffer renderBuffer)
	{
		RenderBufferEntry renderBufferEntry = renderBuffers[(int)id];
		TextureEntry textureEntry = textures[(int)renderBuffer.texture_id];
		renderBufferEntry.textureEntry = textureEntry;

		fixed (ImageView* attachmentsPtr = &textureEntry.imageView)
		{
			FramebufferCreateInfo framebufferInfo = new()
			{
				SType = StructureType.FramebufferCreateInfo,
				RenderPass = pipelineRenderPass,
				AttachmentCount = 1,
				PAttachments = attachmentsPtr,
				Width = textureEntry.width,
				Height = textureEntry.height,
				Layers = 1,
			};
			if (vk.CreateFramebuffer(device, framebufferInfo, null, out renderBufferEntry.framebuffer) is not Result.Success)
			{
				throw new Exception("failed to create framebuffer!");
			}
		}
	}
	[SkipLocalsInit]
	private void DestroyRenderBuffer(uint id)
	{
		RenderBufferEntry renderBufferEntry = renderBuffers[(int)id];
		vk.DestroyFramebuffer(device, renderBufferEntry.framebuffer, null);
		renderBuffersFreeIds.Enqueue(id);
	}

	//[SkipLocalsInit]
	private void CreateTexture(uint id, ULBitmap bitmap)
	{
		TextureEntry textureEntry = textures[(int)id];

		uint width = textureEntry.width = bitmap.Width;
		uint height = textureEntry.height = bitmap.Height;

		bool isBgra = bitmap.Format is ULBitmapFormat.BGRA8_UNORM_SRGB;
		bool isRt = bitmap.IsEmpty;

		ImageCreateInfo imageInfo = new()
		{
			SType = StructureType.ImageCreateInfo,
			ImageType = ImageType.ImageType2D,
			Extent =
			{
				Width = width,
				Height = height,
				Depth = 1,
			},
			MipLevels = 1,
			ArrayLayers = 1,
			Format = isBgra ? Format.B8G8R8A8Unorm : Format.R8Unorm,
			Tiling = ImageTiling.Optimal,
			InitialLayout = ImageLayout.Undefined,
			Usage = isRt ? ImageUsageFlags.ImageUsageColorAttachmentBit | ImageUsageFlags.ImageUsageSampledBit : ImageUsageFlags.ImageUsageTransferDstBit | ImageUsageFlags.ImageUsageSampledBit,
			Samples = isRt ? SampleCountFlags.SampleCount1Bit : SampleCountFlags.SampleCount1Bit,
			SharingMode = SharingMode.Exclusive,
		};

		Image image;
		if (vk.CreateImage(device, imageInfo, null, &image) is not Result.Success)
		{
			throw new Exception("failed to create image!");
		}
		textureEntry.image = image;

		#region Allocate DeviceMemory
		MemoryRequirements memoryRequirements;
		vk.GetImageMemoryRequirements(device, image, &memoryRequirements);

		MemoryAllocateInfo allocInfo = new()
		{
			SType = StructureType.MemoryAllocateInfo,
			AllocationSize = memoryRequirements.Size,
			MemoryTypeIndex = FindMemoryType(memoryRequirements.MemoryTypeBits, MemoryPropertyFlags.MemoryPropertyDeviceLocalBit),
		};

		DeviceMemory imageMemory;
		if (vk.AllocateMemory(device, allocInfo, null, &imageMemory) is not Result.Success)
		{
			throw new Exception("failed to allocate image memory!");
		}

		vk.BindImageMemory(device, image, imageMemory, 0);
		textureEntry.imageMemory = imageMemory;
		#endregion Allocate DeviceMemory

		if (isRt)
		{
			TransitionImageLayout(image, ImageLayout.Undefined, ImageLayout.ShaderReadOnlyOptimal);
		}
		else
		{
			nuint bitmapSize = bitmap.Size;

			// TODO: nuint.MaxValue > int.MaxValue
			if (bitmapSize >= int.MaxValue) throw error;

			CreateBuffer(bitmapSize, BufferUsageFlags.BufferUsageTransferSrcBit, MemoryPropertyFlags.MemoryPropertyHostVisibleBit | MemoryPropertyFlags.MemoryPropertyHostCoherentBit, out Buffer stagingBuffer, out DeviceMemory stagingBufferMemory);

			void* mappedStagingBufferMemory;
			vk.MapMemory(device, stagingBufferMemory, 0, bitmapSize, 0, &mappedStagingBufferMemory);
			new ReadOnlySpan<byte>((void*)bitmap.LockPixels(), (int)bitmapSize).CopyTo(new Span<byte>(mappedStagingBufferMemory, (int)bitmapSize));
			bitmap.UnlockPixels();
			vk.UnmapMemory(device, stagingBufferMemory);

			TransitionImageLayout(image, ImageLayout.Undefined, ImageLayout.TransferDstOptimal);

			CommandBuffer commandBuffer = BeginSingleTimeCommands();

			BufferImageCopy region = new()
			{
				BufferOffset = 0,
				BufferRowLength = bitmap.RowBytes,
				BufferImageHeight = 0,
				ImageSubresource =
				{
					AspectMask = ImageAspectFlags.ImageAspectColorBit,
					MipLevel = 0,
					BaseArrayLayer = 0,
					LayerCount = 1,
				},
				ImageOffset = new Offset3D(0, 0, 0),
				ImageExtent = new Extent3D(width, height, 1),
			};

			vk.CmdCopyBufferToImage(commandBuffer, stagingBuffer, image, ImageLayout.TransferDstOptimal, 1, &region);

			EndSingleTimeCommands(commandBuffer);

			TransitionImageLayout(image, ImageLayout.TransferDstOptimal, ImageLayout.ShaderReadOnlyOptimal);

			// TODO: reuse staging buffers
			vk.DestroyBuffer(device, stagingBuffer, null);
			vk.FreeMemory(device, stagingBufferMemory, null);
		}
		ImageView imageView = CreateImageView(image, imageInfo.Format);
		textureEntry.imageView = imageView;
		#region DescriptorPool
		var poolSize = new DescriptorPoolSize()
		{
			Type = DescriptorType.CombinedImageSampler,
			DescriptorCount = 1,
		};
		DescriptorPool descriptorPool;

		DescriptorPoolCreateInfo poolInfo = new()
		{
			SType = StructureType.DescriptorPoolCreateInfo,
			PoolSizeCount = 1,
			PPoolSizes = &poolSize,
			MaxSets = 2,
		};

		if (vk.CreateDescriptorPool(device, poolInfo, null, &descriptorPool) is not Result.Success)
		{
			throw new Exception("failed to create descriptor pool!");
		}
		textureEntry.descriptorPool = descriptorPool;
		#endregion DescriptorPool
		#region DescriptorSet
		DescriptorSet descriptorSet;

		fixed (DescriptorSetLayout* textureSetLayoutPtr = &textureSetLayout)
		{
			DescriptorSetAllocateInfo allocateInfo = new()
			{
				SType = StructureType.DescriptorSetAllocateInfo,
				DescriptorPool = descriptorPool,
				DescriptorSetCount = 1,
				PSetLayouts = textureSetLayoutPtr
			};
			if (vk.AllocateDescriptorSets(device, allocateInfo, &descriptorSet) != Result.Success)
			{
				throw new Exception("failed to allocate descriptor sets!");
			}
		}

		textureEntry.descriptorSet = descriptorSet;
		#endregion DescriptorSet
		#region DescriptorSetUpdate
		DescriptorImageInfo descriptorImageInfo = new()
		{
			ImageLayout = ImageLayout.ShaderReadOnlyOptimal,
			ImageView = imageView,
			Sampler = textureSampler,
		};
		WriteDescriptorSet descriptorWrite = new()
		{
			SType = StructureType.WriteDescriptorSet,
			DstSet = descriptorSet,
			DstBinding = 0,
			DstArrayElement = 0,
			DescriptorType = DescriptorType.CombinedImageSampler,
			DescriptorCount = 1,
			PImageInfo = &descriptorImageInfo
		};
		vk.UpdateDescriptorSets(device, 1, &descriptorWrite, 0, null);
		#endregion DescriptorSetUpdate
	}

	private void UpdateTexture(uint id, ULBitmap bitmap)
	{
		// TODO
	}
	private void DestroyTexture(uint id)
	{
		// TODO
	}

	[SkipLocalsInit]
	private void CreateGeometry(uint id, ULVertexBuffer vb, ULIndexBuffer ib)
	{
		Buffer vertexStagingBuffer;
		Buffer vertexBuffer;
		Buffer indexStagingBuffer;
		Buffer indexBuffer;

		DeviceMemory vertexStagingMemory;
		DeviceMemory vertexMemory;
		DeviceMemory indexStagingMemory;
		DeviceMemory indexMemory;

		CreateBuffer(vb.size, BufferUsageFlags.BufferUsageTransferSrcBit, MemoryPropertyFlags.MemoryPropertyHostVisibleBit | MemoryPropertyFlags.MemoryPropertyHostCoherentBit, out vertexStagingBuffer, out vertexStagingMemory);
		CreateBuffer(vb.size, BufferUsageFlags.BufferUsageTransferDstBit | BufferUsageFlags.BufferUsageVertexBufferBit, MemoryPropertyFlags.MemoryPropertyDeviceLocalBit, out vertexBuffer, out vertexMemory);

		CreateBuffer(ib.size, BufferUsageFlags.BufferUsageTransferSrcBit, MemoryPropertyFlags.MemoryPropertyHostVisibleBit | MemoryPropertyFlags.MemoryPropertyHostCoherentBit, out indexStagingBuffer, out indexStagingMemory);
		CreateBuffer(ib.size, BufferUsageFlags.BufferUsageTransferDstBit | BufferUsageFlags.BufferUsageIndexBufferBit, MemoryPropertyFlags.MemoryPropertyDeviceLocalBit, out indexBuffer, out indexMemory);

		// TODO: nuint.MaxValue > int.MaxValue
		if (Math.Max(vb.size, ib.size) >= int.MaxValue) throw error;

		void* vertexStaging;
		vk.MapMemory(device, vertexStagingMemory, 0, vb.size, 0, &vertexStaging);
		new ReadOnlySpan<byte>(vb.data, (int)vb.size).CopyTo(new Span<byte>(vertexStaging, (int)vb.size));
		vk.UnmapMemory(device, vertexStagingMemory);

		void* indexStaging;
		vk.MapMemory(device, indexStagingMemory, 0, ib.size, 0, &indexStaging);
		new ReadOnlySpan<byte>(ib.data, (int)ib.size).CopyTo(new Span<byte>(indexStaging, (int)ib.size));
		vk.UnmapMemory(device, indexStagingMemory);

		CommandBuffer stageCopy = BeginSingleTimeCommands();
		CopyBuffer(stageCopy, vertexStagingBuffer, vertexBuffer, vb.size);
		CopyBuffer(stageCopy, indexStagingBuffer, indexBuffer, ib.size);
		EndSingleTimeCommands(stageCopy);

		geometries[(int)id] = new GeometryEntry()
		{
			vertexStagingBuffer = vertexStagingBuffer,
			vertexBuffer = vertexBuffer,
			indexStagingBuffer = indexStagingBuffer,
			indexBuffer = indexBuffer,
			vertexStagingMemory = vertexStagingMemory,
			vertexMemory = vertexMemory,
			indexStagingMemory = indexStagingMemory,
			indexMemory = indexMemory
		};
	}
	[SkipLocalsInit]
	private void UpdateGeometry(uint id, ULVertexBuffer vb, ULIndexBuffer ib)
	{
		GeometryEntry geometryEntry = geometries[(int)id];

		// TODO: nuint.MaxValue > int.MaxValue
		if (Math.Max(vb.size, ib.size) >= int.MaxValue) throw error;

		void* vertexStaging;
		vk.MapMemory(device, geometryEntry.vertexStagingMemory, 0, vb.size, 0, &vertexStaging);
		new ReadOnlySpan<byte>(vb.data, (int)vb.size).CopyTo(new Span<byte>(vertexStaging, (int)vb.size));
		vk.UnmapMemory(device, geometryEntry.vertexStagingMemory);

		void* indexStaging;
		vk.MapMemory(device, geometryEntry.indexStagingMemory, 0, ib.size, 0, &indexStaging);
		new ReadOnlySpan<byte>(ib.data, (int)ib.size).CopyTo(new Span<byte>(indexStaging, (int)ib.size));
		vk.UnmapMemory(device, geometryEntry.indexStagingMemory);

		CommandBuffer stageCopy = BeginSingleTimeCommands();
		CopyBuffer(stageCopy, geometryEntry.vertexStagingBuffer, geometryEntry.vertexBuffer, vb.size);
		CopyBuffer(stageCopy, geometryEntry.indexStagingBuffer, geometryEntry.indexBuffer, ib.size);
		EndSingleTimeCommands(stageCopy);
	}
	[SkipLocalsInit]
	private void DestroyGeometry(uint id)
	{
		GeometryEntry geometryEntry = geometries[(int)id];

		vk.DestroyBuffer(device, geometryEntry.vertexStagingBuffer, null);
		vk.DestroyBuffer(device, geometryEntry.vertexBuffer, null);
		vk.DestroyBuffer(device, geometryEntry.indexStagingBuffer, null);
		vk.DestroyBuffer(device, geometryEntry.indexBuffer, null);

		vk.FreeMemory(device, geometryEntry.vertexStagingMemory, null);
		vk.FreeMemory(device, geometryEntry.vertexMemory, null);
		vk.FreeMemory(device, geometryEntry.indexStagingMemory, null);
		vk.FreeMemory(device, geometryEntry.indexMemory, null);

		geometriesFreeIds.Enqueue(id);
	}

	#region Rendering
	//[SkipLocalsInit]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void UpdateUniformBuffer(ULGPUState state)
	{
		// TODO: device-wise update, not per command like
		Uniforms* uniformBufferMappedMemory;
		vk.MapMemory(device, uniformBufferMemory, 0, UniformBufferSize, 0, (void**)&uniformBufferMappedMemory);
		// State is useless
		uniformBufferMappedMemory->Transform = state.transform.ApplyProjection(state.viewport_width, state.viewport_height, true); // TODO managed transformation
		uniformBufferMappedMemory->ClipSize = state.clip_size;
		new ReadOnlySpan<Vector4>((Vector4*)&state.scalar_0, 10).CopyTo(new Span<Vector4>(&uniformBufferMappedMemory->Scalar4_0, 10));
		new ReadOnlySpan<Matrix4x4>(&state.clip_0, 8).CopyTo(new Span<Matrix4x4>(&uniformBufferMappedMemory->Clip_0, 8));
		vk.UnmapMemory(device, uniformBufferMemory);
	}
	//[SkipLocalsInit]
	private void UpdateCommandList(ULCommandList list)
	{
		CommandBuffer commandBuffer = BeginSingleTimeCommands(); // TODO LMAO
		var s = list.ToSpan();
		foreach (var command in s)
		{
			ULGPUState state = command.gpu_state;
			RenderBufferEntry renderBufferEntry = renderBuffers[(int)command.gpu_state.render_buffer_id];

			RenderPassBeginInfo renderPassBeginInfo = new()
			{
				SType = StructureType.RenderPassBeginInfo,
				RenderPass = pipelineRenderPass,
				Framebuffer = renderBufferEntry.framebuffer,
				RenderArea =
				{
					Offset = { X = 0, Y = 0 },
					Extent = new(state.viewport_width, state.viewport_height),
				}
			};

			if (command.command_type is ULCommandType.DrawGeometry) // TODO: clearrenderbuffer
			{
				vk.CmdBeginRenderPass(commandBuffer, &renderPassBeginInfo, SubpassContents.Inline);
				if (state.shader_type is ULShaderType.Fill) vk!.CmdBindPipeline(commandBuffer, PipelineBindPoint.Graphics, fillPipeline);
				else goto asd;

				vk.CmdBindPipeline(commandBuffer, PipelineBindPoint.Graphics, fillPipeline);

				UpdateUniformBuffer(state);

				TextureEntry textureEntry1 = textures[(int)state.texture_1_id];
				TextureEntry textureEntry2 = textures[(int)state.texture_2_id is 0 ? (int)state.texture_1_id : (int)state.texture_2_id];

				var descriptorSets = stackalloc[] { uniformSet, textureEntry1.descriptorSet, textureEntry2.descriptorSet };

				vk.CmdBindDescriptorSets(commandBuffer, PipelineBindPoint.Graphics, fillPipelineLayout, 0, 3, descriptorSets, 0, 0);

				GeometryEntry geometryEntry = geometries[(int)command.geometry_id];

				vk.CmdBindVertexBuffers(commandBuffer, 0, 1, geometryEntry.vertexBuffer, 0);
				vk.CmdBindIndexBuffer(commandBuffer, geometryEntry.indexBuffer, 0, IndexType.Uint32);
				vk.CmdSetScissor(commandBuffer, 0, 1, state.enable_scissor ? (Rect2D*)&state.scissor_rect : &renderPassBeginInfo.RenderArea);
				vk.CmdDrawIndexed(commandBuffer, command.indices_count, 1, command.indices_offset, 0, 0);
			asd:
				vk.CmdEndRenderPass(commandBuffer);
			}
		}
		EndSingleTimeCommands(commandBuffer);
	}
	#endregion Rendering
}
