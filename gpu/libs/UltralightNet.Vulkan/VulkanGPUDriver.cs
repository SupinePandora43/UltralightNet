using System;
using Silk.NET.Vulkan;

namespace UltralightNet.GPU.Vulkan;

public sealed class VulkanGPUDriver : IDisposable
{
	readonly Vk vk;
	readonly PhysicalDevice physicalDevice;
	readonly Device device;


	public VulkanGPUDriver(Vk vk, PhysicalDevice physicalDevice, Device device)
	{
		this.vk = vk;
		this.physicalDevice = physicalDevice;
		this.device = device;


	}
	public void Dispose()
	{

	}

	// public void BeginSynchronize() {}
	// public void EndSynchronize() {}
}
