using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace UltralightNet.GPU.Vulkan;

internal class DescriptorAllocator
{
	readonly Vk vk;
	readonly PhysicalDevice physicalDevice;
	readonly Device device;

	readonly List<Pool> pools = new(1);
	readonly Queue<int> freePools = new(128);

	public DescriptorAllocator(Vk vk, PhysicalDevice physicalDevice, Device device, DescriptorPoolCreateInfo descriptorPoolCreateInfo)
	{
		this.vk = vk;
		this.physicalDevice = physicalDevice;
		this.device = device;

		//pools.Add(CreatePool(descriptorPoolCreateInfo));
	}

	/*unsafe Pool CreatePool(DescriptorPoolCreateInfo descriptorPoolCreateInfo)
	{

		DescriptorPool pool;
		vk.CreateDescriptorPool(device, &descriptorPoolCreateInfo, null, &pool).Check();
		Pool poolS = new()
		for (uint i = 0; i < 128; i++) freePools.Enqueue(pools.Count - 1);
	}

	public unsafe void AllocateSet(DescriptorSetAllocateInfo descriptorSetAllocateInfo, out DescriptorSet set)
	{
		// Debug.Assert(descriptorSetAllocateInfo.DescriptorPool == default);
		descriptorSetAllocateInfo.DescriptorPool = pools[freePools.Dequeue()];
		vk.AllocateDescriptorSets(device, &descriptorSetAllocateInfo, out set).Check();
	}*/

	struct Pool
	{
		public DescriptorPool pool;
		public DescriptorSet[] sets;
		public Queue<int> freeIds;

		public bool TryGetSet(out DescriptorSet set)
		{
			if (freeIds.TryDequeue(out int id))
			{
				set = sets[id];
				return true;
			}
			set = default;
			return false;
		}
		public void Return(DescriptorSet set)
		{
			var id = MemoryMarshal.Cast<DescriptorSet, ulong>(sets).IndexOf(Unsafe.As<DescriptorSet, ulong>(ref set));
			Debug.Assert(id != -1 && id < sets.Length);
			freeIds.Enqueue(id);
		}
	}
}
