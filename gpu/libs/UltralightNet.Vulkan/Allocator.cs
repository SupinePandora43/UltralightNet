using System;
using Silk.NET.Vulkan;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace UltralightNet.GPU.Vulkan;


internal unsafe class Allocator : IDisposable
{
	public uint FramesInFlight = 2;
	public uint CurrentFrame = 0;

	readonly Vk vk;
	readonly Device device;

	readonly PhysicalDeviceMemoryProperties physicalDeviceMemoryProperties;

	public Allocator(Vk vk, Device device, PhysicalDeviceMemoryProperties physicalDeviceMemoryProperties){
		this.vk = vk;
		this.device = device;
		this.physicalDeviceMemoryProperties = physicalDeviceMemoryProperties;
	}

	public void CreateBuffer(ulong size, BufferUsageFlags bufferUsageFlags, MemoryPropertyFlags memoryPropertyFlags, out Buffer buffer, out DeviceMemory bufferMemory)
	{
		var bufferCreateInfo = new BufferCreateInfo(size: size, usage: bufferUsageFlags, sharingMode: SharingMode.Exclusive/*, queueFamilyIndexCount: 1*/);
		vk.CreateBuffer(device, &bufferCreateInfo, null, out buffer).Check();

		MemoryRequirements memoryRequirements;
		vk.GetBufferMemoryRequirements(device, buffer, &memoryRequirements);

		var memoryAllocateInfo = new MemoryAllocateInfo(allocationSize: memoryRequirements.Size, memoryTypeIndex: physicalDeviceMemoryProperties.FindMemoryTypeIndex(memoryRequirements.MemoryTypeBits, memoryPropertyFlags));
		vk.AllocateMemory(device, &memoryAllocateInfo, null, out bufferMemory).Check();
		vk.BindBufferMemory(device, buffer, bufferMemory, 0).Check();
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}
	~Allocator() => Dispose();
}
