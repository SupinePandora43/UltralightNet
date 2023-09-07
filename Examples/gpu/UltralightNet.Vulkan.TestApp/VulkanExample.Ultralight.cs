using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using UltralightNet;
using UltralightNet.Platform;
using Buffer = Silk.NET.Vulkan.Buffer;

unsafe partial class Application : ISurfaceDefinition // this may cause problems in future...
{
	const bool SurfaceDefinition_SafeBufferization = true;

	readonly List<SurfaceEntry> surfaces = new(2) { new() };
	readonly Queue<int> freeSurfaceIds = new(2);

	readonly Queue<SurfaceEntry> destroyQueue = new();

	SurfaceEntry CreateSurface(nint id, uint width, uint height)
	{
		void CreateBuffer(ulong size, BufferUsageFlags bufferUsageFlags, MemoryPropertyFlags memoryPropertyFlags, out Buffer buffer, out DeviceMemory bufferMemory)
		{
			var queueFamilyIndices = stackalloc uint[] { graphicsQueueFamily };
			var bufferCreateInfo = new BufferCreateInfo(size: size, usage: bufferUsageFlags, sharingMode: SharingMode.Exclusive, queueFamilyIndexCount: 1, pQueueFamilyIndices: queueFamilyIndices);
			vk.CreateBuffer(device, &bufferCreateInfo, null, out buffer).Check();

			MemoryRequirements memoryRequirements;
			vk.GetBufferMemoryRequirements(device, buffer, &memoryRequirements);

			var memoryAllocateInfo = new MemoryAllocateInfo(allocationSize: memoryRequirements.Size, memoryTypeIndex: physicalDeviceMemoryProperties.FindMemoryTypeIndex(memoryRequirements.MemoryTypeBits, memoryPropertyFlags));
			vk.AllocateMemory(device, &memoryAllocateInfo, null, out bufferMemory).Check();
			vk.BindBufferMemory(device, buffer, bufferMemory, 0).Check();
		}
		CreateBuffer(width * height * 4 * MaxFramesInFlight, BufferUsageFlags.BufferUsageTransferSrcBit, MemoryPropertyFlags.MemoryPropertyHostVisibleBit | MemoryPropertyFlags.MemoryPropertyHostCoherentBit, out Buffer stagingBuffer, out DeviceMemory stagingMemory);

		byte* data;
		vk.MapMemory(device, stagingMemory, 0, width * height * 4 * MaxFramesInFlight, 0, (void**)&data).Check();

		var imageCreateInfo = new ImageCreateInfo(
			imageType: ImageType.ImageType2D,
			format: Format.B8G8R8A8Unorm,
			extent: new(width, height, 1),
			mipLevels: 1,
			arrayLayers: 1,
			samples: SampleCountFlags.SampleCount1Bit,
			tiling: ImageTiling.Optimal,
			usage: ImageUsageFlags.ImageUsageSampledBit | ImageUsageFlags.ImageUsageTransferDstBit);
		vk.CreateImage(device, &imageCreateInfo, null, out Image image).Check();

		vk.GetImageMemoryRequirements(device, image, out var imageMemoryRequirements);

		var memoryAllocateInfo = new MemoryAllocateInfo(allocationSize: imageMemoryRequirements.Size, memoryTypeIndex: physicalDeviceMemoryProperties.FindMemoryTypeIndex(imageMemoryRequirements.MemoryTypeBits, MemoryPropertyFlags.MemoryPropertyDeviceLocalBit));
		vk.AllocateMemory(device, &memoryAllocateInfo, null, out DeviceMemory imageMemory).Check();
		vk.BindImageMemory(device, image, imageMemory, 0).Check();

		debugUtils?.SetDebugUtilsObjectName(device, stagingBuffer, $"View {id} staging Buffer");
		debugUtils?.SetDebugUtilsObjectName(device, stagingMemory, $"View {id} staging Memory");
		debugUtils?.SetDebugUtilsObjectName(device, image, $"View {id} Image");
		debugUtils?.SetDebugUtilsObjectName(device, imageMemory, $"View {id} Image Memory");

		return new()
		{
			stagingBuffer = stagingBuffer,
			stagingMemory = stagingMemory,
			imageLayout = ImageLayout.Undefined,
			image = image,
			imageMemory = imageMemory,
			size = width * height * 4,
			mapped = data,
			width = width,
			height = height,
			latestFrame = (uint)CurrentFrame
		};
	}
	nint ISurfaceDefinition.Create(uint width, uint height)
	{
		if (!freeSurfaceIds.TryDequeue(out var id))
		{
			id = surfaces.Count;
			surfaces.Add(default);
		}
		surfaces[id] = CreateSurface(id, width, height);
		return id;
	}
	void ISurfaceDefinition.Destroy(nint id)
	{
		var entry = surfaces[(int)id];
		destroyQueue.Enqueue(entry with { latestFrame = unchecked((uint)CurrentFrame) });
		freeSurfaceIds.Enqueue((int)id);
	}

	uint ISurfaceDefinition.GetWidth(nint id) => surfaces[(int)id].width;
	uint ISurfaceDefinition.GetHeight(nint id) => surfaces[(int)id].height;
	uint ISurfaceDefinition.GetRowBytes(nint id) => surfaces[(int)id].width * 4;
	nuint ISurfaceDefinition.GetSize(nint id) => surfaces[(int)id].size;

	byte* ISurfaceDefinition.LockPixels(nint id)
	{
		ref var entry = ref CollectionsMarshal.AsSpan(surfaces)[(int)id];

		if (SurfaceDefinition_SafeBufferization && entry.latestFrame != CurrentFrame)
		{
			new ReadOnlySpan<byte>(entry.mapped + (entry.size * entry.latestFrame), checked((int)entry.size))
				.CopyTo(new Span<byte>(entry.mapped + (entry.size * unchecked((uint)CurrentFrame)), checked((int)entry.size)));
			entry.latestFrame = unchecked((uint)CurrentFrame);
		}
		return entry.mapped + (entry.size * unchecked((uint)CurrentFrame));
	}
	void ISurfaceDefinition.UnlockPixels(nint id)
	{
		ref var entry = ref CollectionsMarshal.AsSpan(surfaces)[(int)id];

		Debug.Assert(entry.latestFrame == CurrentFrame);

		var commandBuffer = GetSurfaceDefinitionCommandBuffer();
		if (debugUtils is not null)
		{
			using ULString label = new($"Update View {id} Image");
			debugUtils.CmdBeginDebugUtilsLabel(commandBuffer, new DebugUtilsLabelEXT(pLabelName: label.data));
		}

		var imageMemoryBarrier = new ImageMemoryBarrier(
			srcAccessMask: entry.imageLayout is ImageLayout.Undefined ? AccessFlags.AccessNoneKhr : AccessFlags.AccessShaderReadBit, dstAccessMask: AccessFlags.AccessTransferWriteBit,
			oldLayout: ImageLayout.Undefined, newLayout: ImageLayout.TransferDstOptimal,
			image: entry.image, subresourceRange: new ImageSubresourceRange(ImageAspectFlags.ImageAspectColorBit, 0, 1, 0, 1));
		vk.CmdPipelineBarrier(commandBuffer,
			PipelineStageFlags.PipelineStageFragmentShaderBit, PipelineStageFlags.PipelineStageTransferBit, 0,
			0, null,
			0, null,
			1, &imageMemoryBarrier);

		var bufferImageCopy = new BufferImageCopy(entry.size * unchecked((uint)CurrentFrame), entry.width, entry.height, new ImageSubresourceLayers(ImageAspectFlags.ImageAspectColorBit, 0, 0, 1), imageExtent: new Extent3D(entry.width, entry.height, 1));
		vk.CmdCopyBufferToImage(commandBuffer, entry.stagingBuffer, entry.image, ImageLayout.TransferDstOptimal, 1, &bufferImageCopy);

		imageMemoryBarrier = imageMemoryBarrier with
		{
			SrcAccessMask = AccessFlags.AccessTransferWriteBit,
			DstAccessMask = AccessFlags.AccessShaderReadBit,
			OldLayout = ImageLayout.TransferDstOptimal,
			NewLayout = ImageLayout.ShaderReadOnlyOptimal
		};
		vk.CmdPipelineBarrier(commandBuffer,
			PipelineStageFlags.PipelineStageTransferBit, PipelineStageFlags.PipelineStageFragmentShaderBit, 0,
			0, null,
			0, null,
			1, &imageMemoryBarrier);

		entry.imageLayout = ImageLayout.ShaderReadOnlyOptimal;

		debugUtils?.CmdEndDebugUtilsLabel(commandBuffer);
	}

	void ISurfaceDefinition.Resize(nint id, uint width, uint height)
	{
		ref var entry = ref CollectionsMarshal.AsSpan(surfaces)[(int)id];
		destroyQueue.Enqueue(entry);

		// var commandBuffer = GetSurfaceDefinitionCommandBuffer(); // if we were actually resizing it...

		entry = CreateSurface(id, width, height);
	}

	CommandBuffer GetSurfaceDefinitionCommandBuffer()
	{
		var commandBuffer = ultralightCommandBuffers[CurrentFrame];
		if (!ultralightCommandBufferBegun[CurrentFrame])
		{
			vk.ResetCommandBuffer(commandBuffer, 0).Check();
			var commandBufferInheritanceInfo = new CommandBufferInheritanceInfo(pNext: null);
			var commandBufferBeginInfo = new CommandBufferBeginInfo(flags: CommandBufferUsageFlags.CommandBufferUsageOneTimeSubmitBit, pInheritanceInfo: &commandBufferInheritanceInfo);
			vk.BeginCommandBuffer(commandBuffer, &commandBufferBeginInfo).Check();

			ultralightCommandBufferBegun[CurrentFrame] = true;
		};
		return commandBuffer;
	}
	bool TryGetSurfaceDefinitionCommandBufferToSubmit(out CommandBuffer commandBuffer)
	{
		if (!ultralightCommandBufferBegun[CurrentFrame]) { commandBuffer = default; return false; }
		else ultralightCommandBufferBegun[CurrentFrame] = false;

		commandBuffer = ultralightCommandBuffers[CurrentFrame];
		vk.EndCommandBuffer(commandBuffer).Check();
		return true;
	}

	void ExecuteSurfaceDefinitionDestroyByFrameQueue()
	{
		while (destroyQueue.TryPeek(out var entry) && entry.latestFrame == CurrentFrame) FreeSurface(destroyQueue.Dequeue());
	}
	void ExecuteSurfaceDefinitionDestroyQueue()
	{
		while (destroyQueue.TryDequeue(out var entry)) FreeSurface(entry);
	}
	void FreeSurface(SurfaceEntry entry)
	{
		vk.DestroyImage(device, entry.image, null);
		vk.FreeMemory(device, entry.imageMemory, null);
		vk.DestroyBuffer(device, entry.stagingBuffer, null);
		vk.UnmapMemory(device, entry.stagingMemory);
		vk.FreeMemory(device, entry.stagingMemory, null);
	}

	private struct SurfaceEntry
	{
		public Buffer stagingBuffer;
		public DeviceMemory stagingMemory;
		public ImageLayout imageLayout;
		public Image image;
		public DeviceMemory imageMemory;
		public nuint size;
		public byte* mapped;
		public uint width, height;
		public uint latestFrame;
	}
}
