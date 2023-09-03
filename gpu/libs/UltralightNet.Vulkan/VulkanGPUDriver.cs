using System;
using Silk.NET.Vulkan;
using UltralightNet.Platform;

namespace UltralightNet.GPU.Vulkan;

public unsafe sealed class VulkanGPUDriver : IGPUDriver
{
	readonly Vk vk;
	readonly PhysicalDevice physicalDevice;
	readonly Device device;

	readonly int framesInFlight = -1;
	readonly bool UMA = false;

	readonly ResourceList<int> textures = new();
	readonly ResourceList<int> renderBuffers = new();
	readonly ResourceList<GeometryEntry> geometries = new();

	readonly Allocator allocator;
	readonly DescriptorAllocator textureDescriptorAllocator;

	public uint FrameId { get; set; }

	CommandBuffer commandBuffer;

	public VulkanGPUDriver(Vk vk, PhysicalDevice physicalDevice, Device device)
	{
		this.vk = vk;
		this.physicalDevice = physicalDevice;
		this.device = device;

		var descriptorPoolSize = new DescriptorPoolSize(DescriptorType.CombinedImageSampler, 1);
		var descriptorPoolCreateInfo = new DescriptorPoolCreateInfo(maxSets: 128, poolSizeCount: 1, pPoolSizes: &descriptorPoolSize);
		textureDescriptorAllocator = new(vk, physicalDevice, device, descriptorPoolCreateInfo);
	}

	// Call only AFTER all commands are completed.
	public void Dispose()
	{
		// free pools
	}

	void IGPUDriver.CreateRenderBuffer(uint renderBufferId, ULRenderBuffer renderBuffer)
	{
		throw new NotImplementedException();
	}

	void IGPUDriver.CreateTexture(uint textureId, ULBitmap bitmap)
	{
		throw new NotImplementedException();
	}

	void IGPUDriver.DestroyRenderBuffer(uint renderBufferId)
	{
		throw new NotImplementedException();
	}

	void IGPUDriver.DestroyTexture(uint textureId)
	{
		throw new NotImplementedException();
	}

	void IGPUDriver.UpdateCommandList(ULCommandList commandList)
	{
		throw new NotImplementedException();
	}

	void IGPUDriver.CreateGeometry(uint geometryId, ULVertexBuffer vertexBuffer, ULIndexBuffer indexBuffer)
	{
		ref var g = ref geometries[(int)geometryId];
		// allocate buffers
		if (!UMA)
		{
			allocator.CreateBuffer(vertexBuffer.size + indexBuffer.size, BufferUsageFlags.BufferUsageVertexBufferBit | BufferUsageFlags.BufferUsageIndexBufferBit | BufferUsageFlags.BufferUsageTransferDstBit, MemoryPropertyFlags.MemoryPropertyDeviceLocalBit, out var sharedDeviceBuffer, out var sharedDeviceMemory);
			allocator.CreateBuffer((vertexBuffer.size + indexBuffer.size) * (uint)framesInFlight, BufferUsageFlags.BufferUsageTransferSrcBit, MemoryPropertyFlags.MemoryPropertyHostVisibleBit | MemoryPropertyFlags.MemoryPropertyHostCoherentBit, out var sharedHostBuffer, out var sharedHostMemory);

			byte* mapped;
			vk.MapMemory(device, sharedHostMemory, 0, (vertexBuffer.size + indexBuffer.size) * (uint)framesInFlight, 0, (void**)&mapped).Check();

			var vertex = new BufferResource[1 + framesInFlight];
			vertex[0] = new() { Buffer = sharedDeviceBuffer, Offset = indexBuffer.size, Size = vertexBuffer.size };
			for (uint frame = 0; frame < framesInFlight; frame++)
			{
				vertex[frame + 1] = new() { Buffer = sharedHostBuffer, Offset = (indexBuffer.size * (uint)framesInFlight) + (vertexBuffer.size * frame), Size = vertexBuffer.size, Mapped = mapped + (indexBuffer.size * (uint)framesInFlight) + (vertexBuffer.size * frame) };
			}

			var index = new BufferResource[1 + framesInFlight];
			index[0] = new() { Buffer = sharedDeviceBuffer, Offset = 0, Size = indexBuffer.size };
			for (uint frame = 0; frame < framesInFlight; frame++)
			{
				index[frame + 1] = new() { Buffer = sharedHostBuffer, Offset = indexBuffer.size * frame, Size = indexBuffer.size, Mapped = mapped + indexBuffer.size * frame };
			}

			g = new()
			{
				Vertex = new(this, vertex),
				Index = new(this, index)
			};
		}
		else
		{
			allocator.CreateBuffer((vertexBuffer.size + indexBuffer.size) * (uint)framesInFlight, BufferUsageFlags.BufferUsageVertexBufferBit | BufferUsageFlags.BufferUsageIndexBufferBit, MemoryPropertyFlags.MemoryPropertyDeviceLocalBit | MemoryPropertyFlags.MemoryPropertyHostVisibleBit | MemoryPropertyFlags.MemoryPropertyHostCoherentBit, out var sharedBuffer, out var sharedMemory);
			throw new NotImplementedException("TODO");
		}
		(this as IGPUDriver).UpdateGeometry(geometryId, vertexBuffer, indexBuffer);
	}
	void IGPUDriver.UpdateGeometry(uint geometryId, ULVertexBuffer vertexBuffer, ULIndexBuffer indexBuffer)
	{
		ref var g = ref geometries[(int)geometryId];
		new Span<byte>(vertexBuffer.data, (int)vertexBuffer.size).CopyTo(g.Vertex.ToWrite.AsSpan());
		new Span<byte>(indexBuffer.data, (int)indexBuffer.size).CopyTo(g.Index.ToWrite.AsSpan());

		if (!UMA)
		{
			var bufferMemoryBarriers = stackalloc BufferMemoryBarrier[2] {
				new(
					srcAccessMask: AccessFlags.AccessVertexAttributeReadBit, dstAccessMask: AccessFlags.AccessTransferWriteBit,
					srcQueueFamilyIndex: Vk.QueueFamilyIgnored, dstQueueFamilyIndex: Vk.QueueFamilyIgnored,
					buffer: g.Vertex.ToUse.Buffer, offset: g.Vertex.ToUse.Offset, size: g.Vertex.ToUse.Size),
				new(
					srcAccessMask: AccessFlags.AccessIndexReadBit, dstAccessMask: AccessFlags.AccessTransferWriteBit,
					srcQueueFamilyIndex: Vk.QueueFamilyIgnored, dstQueueFamilyIndex: Vk.QueueFamilyIgnored,
					buffer: g.Index.ToUse.Buffer, offset: g.Index.ToUse.Offset, size: g.Index.ToUse.Size)
			};

			vk.CmdPipelineBarrier(commandBuffer,
				PipelineStageFlags.PipelineStageBottomOfPipeBit,
				PipelineStageFlags.PipelineStageTransferBit,
				0,
				0, null,
				4, bufferMemoryBarriers,
				0, null);

			var bufferCopy = new BufferCopy(g.Vertex.ToWrite.Offset, g.Vertex.ToUse.Offset, g.Vertex.ToUse.Size);
			vk.CmdCopyBuffer(commandBuffer, g.Vertex.ToWrite.Buffer, g.Vertex.ToUse.Buffer, 1, &bufferCopy);
			bufferCopy = new(g.Index.ToWrite.Offset, g.Index.ToUse.Offset, g.Index.ToUse.Size);
			vk.CmdCopyBuffer(commandBuffer, g.Index.ToWrite.Buffer, g.Index.ToUse.Buffer, 1, &bufferCopy);

			bufferMemoryBarriers[0].SrcAccessMask = AccessFlags.AccessTransferWriteBit;
			bufferMemoryBarriers[0].DstAccessMask = AccessFlags.AccessVertexAttributeReadBit;

			bufferMemoryBarriers[1].SrcAccessMask = AccessFlags.AccessTransferWriteBit;
			bufferMemoryBarriers[1].DstAccessMask = AccessFlags.AccessIndexReadBit;

			vk.CmdPipelineBarrier(commandBuffer,
				PipelineStageFlags.PipelineStageBottomOfPipeBit,
				PipelineStageFlags.PipelineStageTransferBit,
				0,
				0, null,
				4, bufferMemoryBarriers,
				0, null);
		}
	}
	void IGPUDriver.DestroyGeometry(uint geometryId)
	{
		throw new NotImplementedException();
	}

	void IGPUDriver.UpdateTexture(uint textureId, ULBitmap bitmap)
	{
		throw new NotImplementedException();
	}

	uint IGPUDriver.NextTextureId() => (uint)textures.GetNewId();
	uint IGPUDriver.NextRenderBufferId() => (uint)renderBuffers.GetNewId();
	uint IGPUDriver.NextGeometryId() => (uint)geometries.GetNewId();

	public void BeginSynchronize()
	{
		var commandBufferBeginInfo = new CommandBufferBeginInfo(flags: CommandBufferUsageFlags.CommandBufferUsageOneTimeSubmitBit);
		vk.BeginCommandBuffer(commandBuffer, &commandBufferBeginInfo).Check();
	}
	public void EndSynchronize()
	{
		vk.EndCommandBuffer(commandBuffer).Check();
	}

	unsafe readonly struct GeometryEntry
	{
		public AllocationTuple Vertex { readonly get; init; }
		public AllocationTuple Index { readonly get; init; }

		public struct AllocationTuple
		{
			/// <summary>
			/// Desktop <br />
			/// 0 - DEVICE_LOCAL bind TRANSFER_DST <br />
			/// 1 - STAGING frame_id TRANSFER_SRC <br />
			/// 2 - STAGING frame_id TRANSFER_SRC <br />
			/// Unified memory <br />
			/// 0 - UNIFIED frame_id <br />
			/// 1 - UNIFIED frame_id <br />
			/// </summary>
			/// <remarks>Single DEVICE_LOCAL because frames are executed sequentially and synchronized by a semaphore</remarks>
			readonly BufferResource[] buffers;
			readonly VulkanGPUDriver owner;
			int mostRecent = -1;

			public BufferResource ToWrite => owner.UMA ? buffers[mostRecent = (int)owner.FrameId + 1] : buffers[(int)owner.FrameId];
			public readonly BufferResource ToUse => owner.UMA ? buffers[mostRecent] : buffers[0];

			public AllocationTuple(VulkanGPUDriver owner, BufferResource[] buffers)
			{
				this.owner = owner;
				this.buffers = buffers;
			}
		}
	}
}
