namespace UltralightNet.Vulkan;

using System;
using System.IO;
using System.Text;
using System.Numerics;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.Vulkan.Extensions.KHR;
using UltralightNet;
using Buffer = Silk.NET.Vulkan.Buffer;

internal class TextureEntry
{

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

	public CommandBuffer stageCopy;
}

public unsafe partial class VulkanGPUDriver
{
	private readonly Vk vk;
	private readonly PhysicalDevice physicalDevice;
	private readonly Device device;
	private readonly CommandPool commandPool;

	private Queue graphicsQueue;
	private Queue transferQueue;

	// TODO: is it useful at all currently?
	private const uint mipLevels = 1;
	private const SampleCountFlags msaaSamples = SampleCountFlags.SampleCount4Bit;

	private readonly List<TextureEntry> textures = new();
	private readonly List<GeometryEntry> geometries = new();
	private readonly Queue<uint> geometriesFreeIds = new();

	public VulkanGPUDriver(Vk vk, Device device)
	{
		this.vk = vk;
		this.device = device;

		geometries.Add(new()); // id => 1 workaround
	}

	private uint NextGeometryId()
	{
		if (geometriesFreeIds.Count is not 0) return geometriesFreeIds.Dequeue();
		else
		{
			geometries.Add(new());
			return (uint)geometries.Count;
		}
	}

	private void CreateTexture(uint id, void* bitmapPtr)
	{
		ULBitmap bitmap = new((IntPtr)bitmapPtr);

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
			MipLevels = mipLevels,
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

		if (isRt)
		{

		}
		else
		{
			nuint bitmapSize = bitmap.Size;

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

		CommandBuffer stageCopy;

		CreateBuffer(vb.size, BufferUsageFlags.BufferUsageTransferSrcBit, MemoryPropertyFlags.MemoryPropertyHostVisibleBit | MemoryPropertyFlags.MemoryPropertyHostCoherentBit, out vertexStagingBuffer, out vertexStagingMemory);
		CreateBuffer(vb.size, BufferUsageFlags.BufferUsageTransferDstBit | BufferUsageFlags.BufferUsageVertexBufferBit, MemoryPropertyFlags.MemoryPropertyDeviceLocalBit, out vertexBuffer, out vertexMemory);

		CreateBuffer(ib.size, BufferUsageFlags.BufferUsageTransferSrcBit, MemoryPropertyFlags.MemoryPropertyHostVisibleBit | MemoryPropertyFlags.MemoryPropertyHostCoherentBit, out indexStagingBuffer, out indexStagingMemory);
		CreateBuffer(ib.size, BufferUsageFlags.BufferUsageTransferDstBit | BufferUsageFlags.BufferUsageIndexBufferBit, MemoryPropertyFlags.MemoryPropertyDeviceLocalBit, out indexBuffer, out indexMemory);

		void* vertexStaging;
		vk.MapMemory(device, vertexStagingMemory, 0, vb.size, 0, &vertexStaging);
		new ReadOnlySpan<byte>(vb.data, (int)vb.size).CopyTo(new Span<byte>(vertexStaging, (int)vb.size));
		vk.UnmapMemory(device, vertexStagingMemory);

		void* indexStaging;
		vk.MapMemory(device, indexStagingMemory, 0, ib.size, 0, &indexStaging);
		new ReadOnlySpan<byte>(ib.data, (int)ib.size).CopyTo(new Span<byte>(indexStaging, (int)ib.size));
		vk.UnmapMemory(device, indexStagingMemory);

		stageCopy = CreateCommandBuffer(CommandBufferLevel.Secondary);

		BeginCommandBuffer(stageCopy);
		CopyBuffer(stageCopy, vertexStagingBuffer, vertexBuffer, vb.size);
		CopyBuffer(stageCopy, indexStagingBuffer, indexBuffer, ib.size);
		EndCommandBuffer(stageCopy);

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

		fixed (CommandBuffer* stageCopy = &geometryEntry.stageCopy)
			vk.FreeCommandBuffers(device, commandPool, 1, stageCopy);
		geometriesFreeIds.Enqueue(id);

		geometries[(int)id] = null;
	}
}
