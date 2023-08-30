using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using UltralightNet.Platform;
using Buffer = Silk.NET.Vulkan.Buffer;

unsafe partial class Application : ISurfaceDefinition // this may cause problems in future...
{
	readonly List<SurfaceEntry> surfaces = new(2) { new() };
	readonly Queue<int> freeSurfaceIds = new(2);

	nint ISurfaceDefinition.Create(uint width, uint height)
	{
		void CreateBuffer(ulong size, BufferUsageFlags bufferUsageFlags, MemoryPropertyFlags memoryPropertyFlags, out Buffer buffer, out DeviceMemory bufferMemory)
		{
			uint FindMemoryTypeIndex(uint memoryTypeBits, MemoryPropertyFlags memoryPropertyFlags)
			{
				for (int i = 0; i < physicalDeviceMemoryProperties.MemoryTypeCount; i++)
					if ((memoryTypeBits & (1 << i)) != 0 && physicalDeviceMemoryProperties.MemoryTypes[i].PropertyFlags.HasFlag(memoryPropertyFlags))
						return (uint)i;
				throw new Exception($"Memory not found: {memoryTypeBits}, {memoryPropertyFlags}");
			}

			var queueFamilyIndices = stackalloc uint[] { graphicsQueueFamily };
			var bufferCreateInfo = new BufferCreateInfo(size: size, usage: bufferUsageFlags, sharingMode: SharingMode.Exclusive, queueFamilyIndexCount: 1, pQueueFamilyIndices: queueFamilyIndices);
			vk.CreateBuffer(device, &bufferCreateInfo, null, out buffer).Check();

			MemoryRequirements memoryRequirements;
			vk.GetBufferMemoryRequirements(device, buffer, &memoryRequirements);

			var memoryAllocateInfo = new MemoryAllocateInfo(allocationSize: memoryRequirements.Size, memoryTypeIndex: FindMemoryTypeIndex(memoryRequirements.MemoryTypeBits, memoryPropertyFlags));
			vk.AllocateMemory(device, &memoryAllocateInfo, null, out bufferMemory).Check();
			vk.BindBufferMemory(device, buffer, bufferMemory, 0).Check();
		}
		CreateBuffer(width * height * 4, BufferUsageFlags.BufferUsageTransferSrcBit, MemoryPropertyFlags.MemoryPropertyHostVisibleBit | MemoryPropertyFlags.MemoryPropertyHostCoherentBit, out Buffer buffer, out DeviceMemory memory);

		SurfaceEntry entry = new() { buffer = buffer, memory = memory, size = width * height * 4, width = width, height = height };
		if (freeSurfaceIds.TryDequeue(out var id))
		{
			surfaces[id] = entry;
			return id;
		}
		else surfaces.Add(entry);
		return surfaces.Count - 1;
	}
	void ISurfaceDefinition.Destroy(nint id)
	{
		var entry = surfaces[(int)id];
		vk.FreeMemory(device, entry.memory, null);
		vk.DestroyBuffer(device, entry.buffer, null);
		freeSurfaceIds.Enqueue((int)id);
	}

	uint ISurfaceDefinition.GetWidth(nint id) => surfaces[(int)id].width;
	uint ISurfaceDefinition.GetHeight(nint id) => surfaces[(int)id].height;
	uint ISurfaceDefinition.GetRowBytes(nint id) => surfaces[(int)id].width * 4;
	nuint ISurfaceDefinition.GetSize(nint id) => surfaces[(int)id].size;

	byte* ISurfaceDefinition.LockPixels(nint id)
	{
		byte* data;
		vk.MapMemory(device, surfaces[(int)id].memory, 0, surfaces[(int)id].size, 0, (void**)&data).Check();
		return data;
	}
	void ISurfaceDefinition.UnlockPixels(nint id){
		ref var entry = ref CollectionsMarshal.AsSpan(surfaces)[(int)id];
		vk.UnmapMemory(device, entry.memory);
		entry.dirty = true;
	}

	void ISurfaceDefinition.Resize(nint id, uint width, uint height)
	{
		throw new NotImplementedException();
	}

	private struct SurfaceEntry
	{
		public Buffer buffer;
		public DeviceMemory memory;
		public nuint size;
		public uint width;
		public uint height;
		public bool dirty;
	}
}
