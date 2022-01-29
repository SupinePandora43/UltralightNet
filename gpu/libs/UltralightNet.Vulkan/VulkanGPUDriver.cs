namespace UltralightNet.Vulkan;

using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;
using UltralightNet;
using Buffer = Silk.NET.Vulkan.Buffer;

// https://github.com/SaschaWillems/Vulkan/blob/master/examples/offscreen/offscreen.cpp for reference

internal class RenderBufferEntry
{
	public Framebuffer framebuffer;
	public RenderPass renderPass;

	public TextureEntry textureEntry;
}

internal class TextureEntry
{
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
	private readonly Vk vk;
	private readonly PhysicalDevice physicalDevice; // TODO
	private readonly Device device;
	public CommandPool commandPool; // TODO

	public Queue graphicsQueue; // TODO
	private Queue transferQueue;

	private readonly DeviceMemory uniformBufferMemory;
	private readonly Image pipelineImage;
	private readonly ImageView pipelineImageView;
	private readonly Framebuffer pipelineFramebuffer;
	private readonly DescriptorSetLayout textureSetLayout;
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
			AddressModeU = SamplerAddressMode.Repeat,
			AddressModeV = SamplerAddressMode.Repeat,
			AddressModeW = SamplerAddressMode.Repeat,
			AnisotropyEnable = false,
			MaxAnisotropy = 1,
			BorderColor = BorderColor.IntOpaqueBlack,
			UnnormalizedCoordinates = false,
			CompareEnable = false,
			CompareOp = CompareOp.Always,
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
		#endregion
		_gpuDriver = new()
		{
			NextTextureId = NextTextureId,
			CreateTexture = CreateTexture,
			UpdateTexture = UpdateTexture,
			DestroyTexture = DestroyTexture,
			NextRenderBufferId = NextRenderBufferId,
			CreateRenderBuffer = CreateRenderBuffer
		};
	}

	private ULGPUDriver _gpuDriver;
	public ULGPUDriver GPUDriver => _gpuDriver;

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

	private void CreateRenderBuffer(uint id, ULRenderBuffer renderBuffer)
	{
		RenderBufferEntry renderBufferEntry = renderBuffers[(int)id];


	}

	[SkipLocalsInit]
	private void CreateTexture(uint id, ULBitmap bitmap)
	{
		TextureEntry textureEntry = textures[(int)id];

		uint width = bitmap.Width;
		uint height = bitmap.Height;

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
			Format = isBgra ? Format.B8G8R8A8Srgb : Format.R8Srgb,
			Tiling = ImageTiling.Optimal,
			InitialLayout = ImageLayout.Undefined,
			Usage = isRt ? ImageUsageFlags.ImageUsageTransientAttachmentBit | ImageUsageFlags.ImageUsageColorAttachmentBit : ImageUsageFlags.ImageUsageTransferSrcBit | ImageUsageFlags.ImageUsageTransferDstBit | ImageUsageFlags.ImageUsageSampledBit,
			Samples = isRt ? msaaSamples : SampleCountFlags.SampleCount1Bit,
			SharingMode = SharingMode.Exclusive,
		};

		Image image;
		if (vk.CreateImage(device, imageInfo, null, &image) is not Result.Success)
		{
			throw new Exception("failed to create image!");
		}
		textureEntry.image = image;
		ImageView imageView = CreateImageView(image, imageInfo.Format);
		textureEntry.imageView = imageView;

		if (isRt)
		{

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

			// TODO: reuse staging buffers
			vk.DestroyBuffer(device, stagingBuffer, null);
			vk.FreeMemory(device, stagingBufferMemory, null);
		}

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

		if (vk!.CreateDescriptorPool(device, poolInfo, null, &descriptorPool) != Result.Success)
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
			DstBinding = 1,
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

		geometries[(int)id] = null;
	}

	[SkipLocalsInit]
	private void UpdateUniformBuffer()
	{

	}
}
