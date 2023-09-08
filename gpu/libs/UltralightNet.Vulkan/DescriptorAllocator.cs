using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace UltralightNet.GPU.Vulkan;

public class DescriptorSetAllocator : IDisposable
{
	const int Count = 128;

	readonly Vk vk;
	readonly Device device;
	readonly DescriptorPoolSize[] descriptorPoolSizes;
	readonly DescriptorSetLayout descriptorSetLayout;

	readonly List<Pool> pools = new(1);
	readonly Queue<int> freePoolQueue = new(Count);

	readonly Queue<(int frame, PooledSet pooledSet)> destroyQueue = new(Count / 8);

	public int CurrentFrame { get; set; }

	public unsafe DescriptorSetAllocator(Vk vk, Device device, DescriptorPoolSize[] descriptorPoolSizes, DescriptorSetLayout descriptorSetLayout)
	{
		this.vk = vk;
		this.device = device;
		this.descriptorPoolSizes = descriptorPoolSizes;
		this.descriptorSetLayout = descriptorSetLayout;

		CreateNewPool();
	}

	unsafe void CreateNewPool()
	{
		DescriptorPool descriptorPool;

		fixed (DescriptorPoolSize* descriptorPoolSizesPtr = descriptorPoolSizes)
		{
			var descriptorPoolCreateInfo = new DescriptorPoolCreateInfo(maxSets: Count, poolSizeCount: (uint)descriptorPoolSizes.Length, pPoolSizes: descriptorPoolSizesPtr);
			vk.CreateDescriptorPool(device, &descriptorPoolCreateInfo, null, &descriptorPool).Check();
		}

		Pool pool = new(descriptorPool);

		Span<DescriptorSetLayout> layouts = stackalloc DescriptorSetLayout[Count];
		layouts.Fill(descriptorSetLayout);

		var descriptorSetAllocateInfo = new DescriptorSetAllocateInfo(descriptorPool: descriptorPool, descriptorSetCount: Count, pSetLayouts: ((ReadOnlySpan<DescriptorSetLayout>)layouts).AsPointer());
		vk.AllocateDescriptorSets(device, &descriptorSetAllocateInfo, pool.sets).Check();

		pools.Add(pool);

		for (int i = 0; i < Count; i++)
		{
			pool.freeIds.Enqueue(i);
			freePoolQueue.Enqueue(pools.Count - 1);
		}
	}

	public PooledSet GetDescriptorSet()
	{
		if (freePoolQueue.TryDequeue(out var poolId))
		{
			if (pools[poolId].TryGetSet(out var idInPool, out var set))
			{
				return new() { Value = set, PoolId = poolId, IdInPool = idInPool };
			}
			else throw new Exception("Pool that was SUPPOSED to have a free set, doesn't have it.");
		}
		CreateNewPool();
		return GetDescriptorSet();
	}

	public void Destroy(PooledSet pooledSet) => destroyQueue.Enqueue((CurrentFrame, pooledSet));

	public readonly struct PooledSet
	{
		public DescriptorSet Value { get; init; }
		public int PoolId { get; init; }
		public int IdInPool { get; init; }
	}

	// TODO: call this in setter
	public void ExecuteCurrentFrameDestroyQueue()
	{
		while (destroyQueue.TryPeek(out (int frame, PooledSet pooledSet) pair) && pair.frame == CurrentFrame)
		{
			pair = destroyQueue.Dequeue();
			pools[pair.pooledSet.PoolId].freeIds.Enqueue(pair.pooledSet.IdInPool);
			freePoolQueue.Enqueue(pair.pooledSet.PoolId);
		}
	}

	public unsafe void Dispose()
	{
		for (int i = 0; i < pools.Count; i++)
		{
			vk.DestroyDescriptorPool(device, pools[i].pool, null);
		}
		pools.Clear();
		freePoolQueue.Clear();

		GC.SuppressFinalize(this);
	}
	~DescriptorSetAllocator() => Dispose();

	readonly struct Pool
	{
		public readonly DescriptorPool pool;
		public readonly DescriptorSet[] sets = new DescriptorSet[Count];
		public readonly Queue<int> freeIds = new(Count);

		public Pool(DescriptorPool pool)
		{
			this.pool = pool;
		}

		public readonly bool TryGetSet(out int id, out DescriptorSet set)
		{
			if (freeIds.TryDequeue(out id))
			{
				set = sets[id];
				return true;
			}
			id = -1;
			set = default;
			return false;
		}
		public readonly void Return(DescriptorSet set)
		{
			var id = MemoryMarshal.Cast<DescriptorSet, ulong>(sets).IndexOf(set.Handle);
			Debug.Assert(id != -1 && id < sets.Length);
			freeIds.Enqueue(id);
		}
	}
}
