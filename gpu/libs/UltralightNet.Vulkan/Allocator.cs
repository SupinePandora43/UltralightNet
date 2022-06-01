using System;
using Silk.NET.Vulkan;

namespace UltralightNet.Vulkan.Memory;

public struct GeometryCreateInfo {
	public int Id { readonly get; init; }
	public ulong Size { readonly get; init; }
	public BufferUsageFlags BufferFlags { readonly get; init; }
	public MemoryPropertyFlags MemoryFlags { readonly get; init; }
	public BufferResource BufferResource { get; set; }
}

public struct GeometryDestroyInfo
{
	public int Id { readonly get; init; }
	public BufferResource BufferResource { get; set; }
}

/*
public unsafe class Allocator
{
	public uint FramesInFlight = 2;
	public uint CurrentFrame = 0;

	private Vk vk;
	private AllocationCallbacks* allocationCallbacks = null;
	private Device device;

	private PhysicalDeviceMemoryProperties physicalDeviceMemoryProperties;

	public BufferAllocation AllocateBuffer(BufferCreateInfo createInfo, MemoryPropertyFlags memoryPropertyFlags)
	{
		Buffer buffer;
		vk.CreateBuffer(device, &createInfo, allocationCallbacks, &buffer);
		var memoryRequirements = GetMemoryRequirements(buffer);
		var memory = AllocateMemory(new(allocationSize: memoryRequirements.Size, memoryTypeIndex: FindMemoryType(memoryRequirements.MemoryTypeBits, memoryPropertyFlags)));
		return new() { buffer = buffer, offset = 0, size = createInfo.Size, memory = memory };
	}

	private MemoryRequirements GetMemoryRequirements(Buffer buffer)
	{
		MemoryRequirements requirements;
		vk.GetBufferMemoryRequirements(device, buffer, &requirements);
		return requirements;
	}

	private uint FindMemoryType(uint memoryTypeBits, MemoryPropertyFlags memoryPropertyFlags)
	{
		for (uint i = 0; i < physicalDeviceMemoryProperties.MemoryTypeCount; i++)
			if (physicalDeviceMemoryProperties.MemoryTypes[(int)i].PropertyFlags.HasFlag(memoryPropertyFlags))
				return i;

		throw new System.Exception("no such memory");
	}

	private DeviceMemory AllocateMemory(MemoryAllocateInfo memoryAllocateInfo)
	{
		DeviceMemory memory;
		vk.AllocateMemory(device, &memoryAllocateInfo, allocationCallbacks, &memory);
		return memory;
	}
}
*/
