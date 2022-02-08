using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using UltralightNet.GPUCommon;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace UltralightNet.Vulkan;

internal unsafe struct UniformBufferEntry
{
	public Uniforms data;
	public Uniforms* mappedMemory;
	public DeviceMemory memory;
	public Buffer buffer;
	public DescriptorPool descriptorPool;
	public DescriptorSet descriptorSet;
}

public unsafe partial class VulkanGPUDriver
{
	private const ulong UniformBufferSize = 768;

	private readonly Dictionary<ULGPUState, UniformBufferEntry> uniformBuffers = new();

	//[SkipLocalsInit]
	/*[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private DescriptorSet GetUniformBuffer(ULGPUState state)
	{
		if (uniformBuffers.TryGetValue(state, out UniformBufferEntry entry))
		{
			return entry.descriptorSet;
		}
		else
		{
			CreateBuffer(UniformBufferSize, BufferUsageFlags.BufferUsageUniformBufferBit, MemoryPropertyFlags.MemoryPropertyHostVisibleBit | MemoryPropertyFlags.MemoryPropertyHostCoherentBit, out Buffer buffer, out DeviceMemory memory);
			Uniforms* mapped;
			vk.MapMemory(device, memory, 0, UniformBufferSize, 0, (void**)&mapped);
			mapped->Transform = state.transform.ApplyProjection(state.viewport_width, state.viewport_height, true);
			mapped->ClipSize = state.clip_size;
			new ReadOnlySpan<Vector4>(&state.scalar_0, 10).CopyTo(new Span<Vector4>(&mapped->Scalar4_0.W, 10));
			new ReadOnlySpan<Matrix4x4>(&state.clip_0.M11, 8).CopyTo(new Span<Matrix4x4>(&mapped->Clip_0.M11, 8));
			vk.UnmapMemory(device, memory);

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
			DescriptorSet uniformSet;
			fixed (DescriptorSetLayout* u = &uniformSetLayout)
			{
				DescriptorSetAllocateInfo allocateInfo = new()
				{
					SType = StructureType.DescriptorSetAllocateInfo,
					DescriptorPool = uniformDescriptorPool,
					DescriptorSetCount = 1,
					PSetLayouts = u
				};
				if (vk.AllocateDescriptorSets(device, allocateInfo, &uniformSet) is not Result.Success)
				{
					throw new Exception("failed to allocate descriptor sets!");
				}
			}

			var bufferInfo = new DescriptorBufferInfo()
			{
				Buffer = buffer,
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

			uniformBuffers[state] = new()
			{
				buffer = buffer,
				memory = memory,
				descriptorPool = uniformDescriptorPool,
				descriptorSet = uniformSet
			};

			return uniformBuffers[state].descriptorSet;
		}
	}*/

}
