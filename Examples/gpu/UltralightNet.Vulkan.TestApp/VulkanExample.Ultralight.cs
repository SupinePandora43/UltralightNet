using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using UltralightNet;
using UltralightNet.Platform;
using Buffer = Silk.NET.Vulkan.Buffer;

unsafe partial class Application : ISurfaceDefinition // this may cause problems in future...
{
	const bool UMA = false;

	readonly List<SurfaceEntry> surfaces = new(2) { new() };
	readonly Queue<int> freeSurfaceIds = new(2);

	nint ISurfaceDefinition.Create(uint width, uint height)
	{
		void CreateBuffer(ulong size, BufferUsageFlags bufferUsageFlags, MemoryPropertyFlags memoryPropertyFlags, out Buffer buffer, out DeviceMemory bufferMemory)
		{
			var queueFamilyIndices = stackalloc uint[] { graphicsQueueFamily };
			var bufferCreateInfo = new BufferCreateInfo(size: size * MaxFramesInFlight, usage: bufferUsageFlags, sharingMode: SharingMode.Exclusive, queueFamilyIndexCount: 1, pQueueFamilyIndices: queueFamilyIndices);
			vk.CreateBuffer(device, &bufferCreateInfo, null, out buffer).Check();

			MemoryRequirements memoryRequirements;
			vk.GetBufferMemoryRequirements(device, buffer, &memoryRequirements);

			var memoryAllocateInfo = new MemoryAllocateInfo(allocationSize: memoryRequirements.Size, memoryTypeIndex: physicalDeviceMemoryProperties.FindMemoryTypeIndex(memoryRequirements.MemoryTypeBits, memoryPropertyFlags));
			vk.AllocateMemory(device, &memoryAllocateInfo, null, out bufferMemory).Check();
			vk.BindBufferMemory(device, buffer, bufferMemory, 0).Check();
		}
		CreateBuffer(width * height * 4, BufferUsageFlags.BufferUsageTransferSrcBit, MemoryPropertyFlags.MemoryPropertyHostVisibleBit | MemoryPropertyFlags.MemoryPropertyHostCoherentBit, out Buffer stagingBuffer, out DeviceMemory stagingMemory);


		var imageCreateInfo = new ImageCreateInfo(
			imageType: ImageType.ImageType2D,
			format: Format.B8G8R8A8Unorm,
			extent: new(width, height, 1),
			mipLevels: 1,
			arrayLayers: 1,
			samples: SampleCountFlags.SampleCount1Bit,
			tiling: ImageTiling.Optimal,
			usage: ImageUsageFlags.ImageUsageSampledBit | ImageUsageFlags.ImageUsageTransferDstBit);
		vk.CreateImage(device, &imageCreateInfo, null, out Image image).Check();

		vk.GetImageMemoryRequirements(device, image, out var imageMemoryRequirements);

		(!UMA).Assert();
		var memoryAllocateInfo = new MemoryAllocateInfo(allocationSize: imageMemoryRequirements.Size, memoryTypeIndex: physicalDeviceMemoryProperties.FindMemoryTypeIndex(imageMemoryRequirements.MemoryTypeBits, MemoryPropertyFlags.MemoryPropertyDeviceLocalBit));
		vk.AllocateMemory(device, &memoryAllocateInfo, null, out DeviceMemory imageMemory).Check();
		vk.BindImageMemory(device, image, imageMemory, 0).Check();

		SurfaceEntry entry = new() { stagingBuffer = stagingBuffer, stagingMemory = stagingMemory, imageLayout = ImageLayout.Undefined, image = image, imageMemory = imageMemory, size = width * height * 4, width = width, height = height };
		if (freeSurfaceIds.TryDequeue(out var id))
		{
			surfaces[id] = entry;
		}
		else { surfaces.Add(entry); id = surfaces.Count - 1; }

#if DEBUG
		debugUtils?.SetDebugUtilsObjectName(device, stagingBuffer, $"View {id} staging Buffer");
		debugUtils?.SetDebugUtilsObjectName(device, stagingMemory, $"View {id} staging Memory");
		debugUtils?.SetDebugUtilsObjectName(device, image, $"View {id} Image");
		debugUtils?.SetDebugUtilsObjectName(device, imageMemory, $"View {id} Image Memory");
#endif

		return id;
	}
	void ISurfaceDefinition.Destroy(nint id)
	{
		var entry = surfaces[(int)id];
		vk.DestroyImage(device, entry.image, null);
		vk.FreeMemory(device, entry.imageMemory, null);
		vk.DestroyBuffer(device, entry.stagingBuffer, null);
		vk.FreeMemory(device, entry.stagingMemory, null);
		freeSurfaceIds.Enqueue((int)id);
	}

	uint ISurfaceDefinition.GetWidth(nint id) => surfaces[(int)id].width;
	uint ISurfaceDefinition.GetHeight(nint id) => surfaces[(int)id].height;
	uint ISurfaceDefinition.GetRowBytes(nint id) => surfaces[(int)id].width * 4;
	nuint ISurfaceDefinition.GetSize(nint id) => surfaces[(int)id].size;

	byte* ISurfaceDefinition.LockPixels(nint id)
	{
		byte* data;
		vk.MapMemory(device, surfaces[(int)id].stagingMemory, 0, surfaces[(int)id].size, 0, (void**)&data).Check();
		return data;
	}
	void ISurfaceDefinition.UnlockPixels(nint id)
	{
		ref var entry = ref CollectionsMarshal.AsSpan(surfaces)[(int)id];
		vk.UnmapMemory(device, entry.stagingMemory);
		entry.dirty = true;
	}

	void ISurfaceDefinition.Resize(nint id, uint width, uint height)
	{
		ref var entry = ref CollectionsMarshal.AsSpan(surfaces)[(int)id];
		entry.width = width;
		entry.height = height;
	}

	private struct SurfaceEntry
	{
		public Buffer stagingBuffer;
		public DeviceMemory stagingMemory;
		public ImageLayout imageLayout;
		public Image image;
		public DeviceMemory imageMemory;
		public nuint size;
		public uint width;
		public uint height;
		public bool dirty;
	}
}
