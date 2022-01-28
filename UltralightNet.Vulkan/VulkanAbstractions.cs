namespace UltralightNet.Vulkan;

using System.Runtime.CompilerServices;
using Silk.NET.Vulkan;

public unsafe partial class VulkanGPUDriver
{
	private static readonly System.Exception error = new();

	private uint FindMemoryType(uint typeFilter, MemoryPropertyFlags properties)
	{
		vk.GetPhysicalDeviceMemoryProperties(physicalDevice, out PhysicalDeviceMemoryProperties memProperties);
		for (int i = 0; i < memProperties.MemoryTypeCount; i++)
		{
			if ((typeFilter & (1 << i)) != 0 && (memProperties.MemoryTypes[i].PropertyFlags & properties) == properties)
			{
				return (uint)i;
			}
		}
		throw error;
	}

	private CommandBuffer CreateCommandBuffer(CommandBufferLevel level)
	{
		CommandBufferAllocateInfo allocateInfo = new()
		{
			SType = StructureType.CommandBufferAllocateInfo,
			Level = level,
			CommandPool = commandPool,
			CommandBufferCount = 1,
		};

		CommandBuffer commandBuffer = default;
		vk.AllocateCommandBuffers(device, &allocateInfo, &commandBuffer);

		return commandBuffer;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void BeginCommandBuffer(CommandBuffer commandBuffer, CommandBufferUsageFlags flags = 0)
	{
		CommandBufferBeginInfo beginInfo = new()
		{
			SType = StructureType.CommandBufferBeginInfo,
			Flags = flags,
		};

		vk.BeginCommandBuffer(commandBuffer, &beginInfo);
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void EndCommandBuffer(CommandBuffer commandBuffer)
	{
		vk.EndCommandBuffer(commandBuffer);
	}

	private CommandBuffer BeginSingleTimeCommands()
	{
		CommandBufferAllocateInfo allocateInfo = new()
		{
			SType = StructureType.CommandBufferAllocateInfo,
			Level = CommandBufferLevel.Primary,
			CommandPool = commandPool,
			CommandBufferCount = 1,
		};

		CommandBuffer commandBuffer;
		vk.AllocateCommandBuffers(device, allocateInfo, &commandBuffer);

		CommandBufferBeginInfo beginInfo = new()
		{
			SType = StructureType.CommandBufferBeginInfo,
			Flags = CommandBufferUsageFlags.CommandBufferUsageOneTimeSubmitBit,
		};

		vk.BeginCommandBuffer(commandBuffer, beginInfo);

		return commandBuffer;
	}

	private void EndSingleTimeCommands(CommandBuffer commandBuffer)
	{
		vk.EndCommandBuffer(commandBuffer);

		SubmitInfo submitInfo = new()
		{
			SType = StructureType.SubmitInfo,
			CommandBufferCount = 1,
			PCommandBuffers = &commandBuffer,
		};

		vk.QueueSubmit(graphicsQueue, 1, submitInfo, default);
		vk.QueueWaitIdle(graphicsQueue);

		vk.FreeCommandBuffers(device, commandPool, 1, commandBuffer);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CopyBuffer(CommandBuffer commandBuffer, Buffer from, Buffer to, ulong size)
	{
		BufferCopy copyRegion = new() { Size = size };
		vk.CmdCopyBuffer(commandBuffer, from, to, 1, &copyRegion);
	}

	private void CreateBuffer(ulong size, BufferUsageFlags usage, MemoryPropertyFlags properties, out Buffer buffer, out DeviceMemory bufferMemory)
	{
		BufferCreateInfo bufferInfo = new()
		{
			SType = StructureType.BufferCreateInfo,
			Size = size,
			Usage = usage,
			SharingMode = SharingMode.Exclusive,
		};

		Buffer refBuffer;

		if (vk.CreateBuffer(device, bufferInfo, null, &refBuffer) != Result.Success)
		{
			throw error;
		}

		MemoryRequirements memRequirements;
		vk.GetBufferMemoryRequirements(device, refBuffer, &memRequirements);

		MemoryAllocateInfo allocateInfo = new()
		{
			SType = StructureType.MemoryAllocateInfo,
			AllocationSize = memRequirements.Size,
			MemoryTypeIndex = FindMemoryType(memRequirements.MemoryTypeBits, properties),
		};

		DeviceMemory refBufferMemory;

		if (vk.AllocateMemory(device, allocateInfo, null, &refBufferMemory) != Result.Success)
		{
			throw error;
		}

		vk.BindBufferMemory(device, refBuffer, refBufferMemory, 0);

		buffer = refBuffer;
		bufferMemory = refBufferMemory;
	}
	private void TransitionImageLayout(Image image, ImageLayout oldLayout, ImageLayout newLayout)
	{
		CommandBuffer commandBuffer = BeginSingleTimeCommands();

		ImageMemoryBarrier barrier = new()
		{
			SType = StructureType.ImageMemoryBarrier,
			OldLayout = oldLayout,
			NewLayout = newLayout,
			SrcQueueFamilyIndex = Vk.QueueFamilyIgnored,
			DstQueueFamilyIndex = Vk.QueueFamilyIgnored,
			Image = image,
			SubresourceRange =
			{
				AspectMask = ImageAspectFlags.ImageAspectColorBit,
				BaseMipLevel = 0,
				LevelCount = mipLevels,
				BaseArrayLayer = 0,
				LayerCount = 1,
			}
		};

		PipelineStageFlags sourceStage;
		PipelineStageFlags destinationStage;

		if (oldLayout is ImageLayout.Undefined && newLayout is ImageLayout.TransferDstOptimal)
		{
			barrier.SrcAccessMask = 0;
			barrier.DstAccessMask = AccessFlags.AccessTransferWriteBit;

			sourceStage = PipelineStageFlags.PipelineStageTopOfPipeBit;
			destinationStage = PipelineStageFlags.PipelineStageTransferBit;
		}
		else if (oldLayout is ImageLayout.TransferDstOptimal && newLayout is ImageLayout.ShaderReadOnlyOptimal)
		{
			barrier.SrcAccessMask = AccessFlags.AccessTransferWriteBit;
			barrier.DstAccessMask = AccessFlags.AccessShaderReadBit;

			sourceStage = PipelineStageFlags.PipelineStageTransferBit;
			destinationStage = PipelineStageFlags.PipelineStageFragmentShaderBit;
		}
		else
		{
			throw error;
		}

		vk.CmdPipelineBarrier(commandBuffer, sourceStage, destinationStage, 0, 0, null, 0, null, 1, barrier);

		EndSingleTimeCommands(commandBuffer);
	}
}