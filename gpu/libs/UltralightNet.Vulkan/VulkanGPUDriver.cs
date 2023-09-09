using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using UltralightNet.GPUCommon;
using UltralightNet.Platform;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace UltralightNet.GPU.Vulkan;

public unsafe sealed class VulkanGPUDriver : IGPUDriver, IDisposable
{
	readonly Vk vk;
	readonly PhysicalDevice physicalDevice;
	readonly Device device;

	uint FramesInFlight { get; }
	readonly bool UMA = false;

	readonly ResourceList<int> textures = new();
	readonly ResourceList<int> renderBuffers = new();
	readonly ResourceList<GeometryEntry> geometries = new();

	readonly DestroyQueue destroyQueue = new();
	readonly Allocator allocator;

	readonly Sampler sampler;

	readonly DescriptorSetLayout uniformBufferDescriptorSetLayout;
	readonly DescriptorSetAllocator uniformBufferDescriptorSetAllocator;

	readonly DescriptorSetLayout textureDescriptorSetLayout;
	readonly DescriptorSetAllocator textureDescriptorSetAllocator;

	public uint CurrentFrame { get; set; }

	CommandBuffer commandBuffer;

	public VulkanGPUDriver(Vk vk, PhysicalDevice physicalDevice, Device device, uint framesInFlight)
	{
		this.vk = vk;
		this.physicalDevice = physicalDevice;
		this.device = device;
		FramesInFlight = framesInFlight;

		allocator = new(vk, device, vk.GetPhysicalDeviceMemoryProperty(physicalDevice));

		{ // Sampler
			var samplerCreateInfo = new SamplerCreateInfo(
				magFilter: Filter.Nearest,
				minFilter: Filter.Linear,
				mipmapMode: SamplerMipmapMode.Nearest,
				addressModeU: SamplerAddressMode.ClampToEdge,
				addressModeV: SamplerAddressMode.ClampToEdge
				// TODO: Anisotropy enable flag
				// TODO: mipmaps
				);
			vk.CreateSampler(device, &samplerCreateInfo, null, out sampler).Check();
		}

		{ // uniformBufferDescriptorSet___
			var descriptorSetLayoutBinding = new DescriptorSetLayoutBinding(0, DescriptorType.UniformBufferDynamic, 1, ShaderStageFlags.ShaderStageVertexBit | ShaderStageFlags.ShaderStageFragmentBit);
			var descriptorSetLayoutCreateInfo = new DescriptorSetLayoutCreateInfo(bindingCount: 1, pBindings: &descriptorSetLayoutBinding);
			vk.CreateDescriptorSetLayout(device, &descriptorSetLayoutCreateInfo, null, out uniformBufferDescriptorSetLayout).Check();

			uniformBufferDescriptorSetAllocator = new(vk, device, new[] { new DescriptorPoolSize(DescriptorType.UniformBufferDynamic, 1) }, uniformBufferDescriptorSetLayout);
		}
		{ // textureDescriptorSet___
			var descriptorSetLayoutBinding = new DescriptorSetLayoutBinding(0, DescriptorType.CombinedImageSampler, 1, ShaderStageFlags.ShaderStageFragmentBit);
			var descriptorSetLayoutCreateInfo = new DescriptorSetLayoutCreateInfo(bindingCount: 1, pBindings: &descriptorSetLayoutBinding);
			vk.CreateDescriptorSetLayout(device, &descriptorSetLayoutCreateInfo, null, out textureDescriptorSetLayout).Check();

			textureDescriptorSetAllocator = new(vk, device, new[] { new DescriptorPoolSize(DescriptorType.CombinedImageSampler, 1) }, textureDescriptorSetLayout);
		}

	}

	// Call only AFTER all commands are completed.
	public void Dispose()
	{
		textureDescriptorSetAllocator.Dispose();
		vk.DestroyDescriptorSetLayout(device, textureDescriptorSetLayout, null);

		uniformBufferDescriptorSetAllocator.Dispose();
		vk.DestroyDescriptorSetLayout(device, uniformBufferDescriptorSetLayout, null);

		vk.DestroySampler(device, sampler, null);

		allocator.Dispose();
		destroyQueue.Dispose();

		textures.Dispose();
		renderBuffers.Dispose();
		geometries.Dispose();
	}



	void IGPUDriver.CreateTexture(uint textureId, ULBitmap bitmap)
	{
		throw new NotImplementedException();
	}
	void IGPUDriver.UpdateTexture(uint textureId, ULBitmap bitmap)
	{
		throw new NotImplementedException();
	}
	void IGPUDriver.DestroyTexture(uint textureId)
	{
		throw new NotImplementedException();
	}

	void IGPUDriver.CreateRenderBuffer(uint renderBufferId, ULRenderBuffer renderBuffer)
	{
		throw new NotImplementedException();
	}
	void IGPUDriver.DestroyRenderBuffer(uint renderBufferId)
	{
		throw new NotImplementedException();
	}

	void IGPUDriver.CreateGeometry(uint geometryId, ULVertexBuffer vertexBuffer, ULIndexBuffer indexBuffer)
	{
		ref var g = ref geometries[(int)geometryId];

		Debug.Assert(vertexBuffer.size % 256 == 0, "nonCoherentAtomSize");
		Debug.Assert(indexBuffer.size % 256 == 0, "nonCoherentAtomSize");

		Buffer sharedDeviceBuffer = default;
		DeviceMemory sharedDeviceMemory = default;

		if (!UMA)
			allocator.CreateBuffer(
				vertexBuffer.size + indexBuffer.size,
				BufferUsageFlags.BufferUsageVertexBufferBit | BufferUsageFlags.BufferUsageIndexBufferBit | BufferUsageFlags.BufferUsageTransferDstBit,
				MemoryPropertyFlags.MemoryPropertyDeviceLocalBit,
				out sharedDeviceBuffer, out sharedDeviceMemory);

		allocator.CreateBuffer(
			(vertexBuffer.size + indexBuffer.size) * FramesInFlight,
			!UMA ? BufferUsageFlags.BufferUsageTransferSrcBit : BufferUsageFlags.BufferUsageVertexBufferBit | BufferUsageFlags.BufferUsageIndexBufferBit,
			MemoryPropertyFlags.MemoryPropertyHostVisibleBit | MemoryPropertyFlags.MemoryPropertyHostCoherentBit,
			out var sharedHostBuffer, out var sharedHostMemory);

		byte* mapped;
		vk.MapMemory(device, sharedHostMemory, 0, (vertexBuffer.size + indexBuffer.size) * FramesInFlight, 0, (void**)&mapped).Check();

		g = new()
		{
			Buffer = !UMA ? sharedDeviceBuffer : sharedHostBuffer,
			HostBuffer = sharedHostBuffer,
			Index = (0UL, (ulong)indexBuffer.size),
			Vertex = (!UMA ? (ulong)indexBuffer.size : (ulong)indexBuffer.size * (ulong)FramesInFlight, (ulong)vertexBuffer.size),
			HostMemory = sharedHostMemory,
			DeviceMemory = sharedDeviceMemory,
			Mapped = mapped
		};
		(this as IGPUDriver).UpdateGeometry(geometryId, vertexBuffer, indexBuffer);
	}
	void IGPUDriver.UpdateGeometry(uint geometryId, ULVertexBuffer vertexBuffer, ULIndexBuffer indexBuffer)
	{
		ref var g = ref geometries[(int)geometryId];

		g.GetBuffersToWriteTo(CurrentFrame, out var index, out var vertex);

		new Span<byte>(vertexBuffer.data, (int)vertexBuffer.size).CopyTo(vertex);
		new Span<byte>(indexBuffer.data, (int)indexBuffer.size).CopyTo(index);

		if (!UMA)
		{
			var bufferMemoryBarriers = stackalloc BufferMemoryBarrier[2] {
				new(
					srcAccessMask: AccessFlags.AccessVertexAttributeReadBit, dstAccessMask: AccessFlags.AccessTransferWriteBit,
					srcQueueFamilyIndex: Vk.QueueFamilyIgnored, dstQueueFamilyIndex: Vk.QueueFamilyIgnored,
					buffer: g.Buffer, offset: g.Vertex.offset, size: g.Vertex.size),
				new(
					srcAccessMask: AccessFlags.AccessIndexReadBit, dstAccessMask: AccessFlags.AccessTransferWriteBit,
					srcQueueFamilyIndex: Vk.QueueFamilyIgnored, dstQueueFamilyIndex: Vk.QueueFamilyIgnored,
					buffer: g.Buffer, offset: g.Index.offset, size: g.Index.size)
			};

			vk.CmdPipelineBarrier(commandBuffer,
				PipelineStageFlags.PipelineStageVertexInputBit,
				PipelineStageFlags.PipelineStageTransferBit,
				0,
				0, null,
				2, bufferMemoryBarriers,
				0, null);

			var bufferCopy = stackalloc BufferCopy[] {
				new(g.Index.size * CurrentFrame, 0, g.Index.size),
				new(g.Vertex.offset + (g.Vertex.size * CurrentFrame), g.Vertex.size)
			};
			vk.CmdCopyBuffer(commandBuffer, g.HostBuffer, g.Buffer, 2, bufferCopy);

			bufferMemoryBarriers[0].SrcAccessMask = AccessFlags.AccessTransferWriteBit;
			bufferMemoryBarriers[0].DstAccessMask = AccessFlags.AccessVertexAttributeReadBit;

			bufferMemoryBarriers[1].SrcAccessMask = AccessFlags.AccessTransferWriteBit;
			bufferMemoryBarriers[1].DstAccessMask = AccessFlags.AccessIndexReadBit;

			vk.CmdPipelineBarrier(commandBuffer,
				PipelineStageFlags.PipelineStageTransferBit,
				PipelineStageFlags.PipelineStageVertexInputBit,
				0,
				0, null,
				2, bufferMemoryBarriers,
				0, null);
		}
	}
	void IGPUDriver.DestroyGeometry(uint geometryId)
	{
		var geo = geometries[(int)geometryId];

		vk.UnmapMemory(device, geo.HostMemory);

		destroyQueue.Enqueue(CurrentFrame, () =>
		{
			vk.DestroyBuffer(device, geo.Buffer, null);
			vk.FreeMemory(device, geo.DeviceMemory, null);
			if (!UMA)
			{
				vk.DestroyBuffer(device, geo.HostBuffer, null);
				vk.FreeMemory(device, geo.HostMemory, null);
			}
		});

		geometries.Remove((int)geometryId);
	}

	void IGPUDriver.UpdateCommandList(ULCommandList commandList)
	{
		foreach (var command in commandList.AsSpan())
		{
			if (command.CommandType is ULCommandType.ClearRenderBuffer)
			{
				return;
			}
			else
			{
				ref var geo = ref geometries[(int)command.GeometryId];
				var indexBuffer = geo.GetIndexBufferToUse(UMA);
				var vertexBuffer = geo.GetVertexBufferToUse(UMA);
				vk.CmdBindIndexBuffer(commandBuffer, indexBuffer.buffer, indexBuffer.offset, IndexType.Uint32);
				vk.CmdBindVertexBuffers(commandBuffer, 0, 1, vertexBuffer.buffer, vertexBuffer.offset);
				vk.CmdDrawIndexed(commandBuffer, command.IndicesCount, 1, command.IndicesOffset, 0, 0);
			}
		}
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

	[StructLayout(LayoutKind.Auto, Pack = 8)]
	unsafe struct GeometryEntry
	{
		public Buffer Buffer { readonly get; init; }
		public Buffer HostBuffer { readonly get; init; }

		public (ulong offset, ulong size) Index { readonly get; init; }
		public (ulong offset, ulong size) Vertex { readonly get; init; }

		public DeviceMemory HostMemory { readonly get; init; }
		public DeviceMemory DeviceMemory { readonly get; init; }

		public byte* Mapped { readonly get; init; }

		public uint latestFrame;

		public readonly (Buffer buffer, ulong offset) GetIndexBufferToUse(bool UMA) => (Buffer, !UMA ? 0 : Index.size * latestFrame);
		public readonly (Buffer buffer, ulong offset) GetVertexBufferToUse(bool UMA) => (Buffer, !UMA ? Index.size : Vertex.offset + (Vertex.size * latestFrame));

		public void GetBuffersToWriteTo(uint frame, out Span<byte> index, out Span<byte> vertex)
		{
			index = new Span<byte>(Mapped + (Index.size * (latestFrame = frame)), (int)Index.size);
			vertex = new Span<byte>(Mapped + (Vertex.offset + (Vertex.size * latestFrame)), (int)Vertex.size);
		}
	}
}
