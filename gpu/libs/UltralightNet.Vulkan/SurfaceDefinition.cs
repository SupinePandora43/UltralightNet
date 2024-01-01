using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using UltralightNet.GPUCommon;
using UltralightNet.Platform;
using Buffer = Silk.NET.Vulkan.Buffer;

namespace UltralightNet.GPU.Vulkan;

public unsafe sealed class SurfaceDefinition : ISurfaceDefinition, IDisposable
{
	readonly Vk vk;
	readonly Device device;
	readonly PhysicalDeviceMemoryProperties physicalDeviceMemoryProperties; // TODO: use Allocator
	readonly uint framesInFlight;
	readonly Bufferization bufferization;
	readonly Queue<(uint frame, SurfaceEntry entry)> destroyQueue = new(16); // TODO: internal, in set_CurrentFrame
	readonly bool resetCommandBuffers;
	readonly CommandBuffer[] commandBuffers;

	readonly ExtDebugUtils? debugUtils;

	readonly ResourceList<SurfaceEntry> surfaces = new();

	uint currentFrame;
	public uint CurrentFrame
	{
		get => currentFrame;
		set
		{
			if (currentFrame > framesInFlight) throw new ArgumentOutOfRangeException(nameof(value), $"{currentFrame} is greater than the number of frames in flight, equal to {framesInFlight}.");

			while (destroyQueue.TryPeek(out (uint frame, SurfaceEntry entry) result) && result.frame == value)
				FreeSurface(destroyQueue.Dequeue().entry);

			if (commandBufferBegun) throw new Exception($"Command Buffer recording started, but was not finished. ({nameof(TryGetCommandBufferForSubmission)} was not called.)");

			currentFrame = value;
		}
	}
	bool commandBufferBegun = false;

	/// <param name="framesInFlight">How many frames are rendered simultaneously. (Make sure resources don't get disposed while in use.)</param>
	/// <param name="bufferization">Make sure that the rendered frame is exactly what was rendered by Ultralight, or not.</param>
	/// <param name="resetCommandBuffers">Automatically call <see cref="Vk.ResetCommandBuffer(CommandBuffer, CommandBufferResetFlags)" /> before <see cref="Vk.BeginCommandBuffer(CommandBuffer, CommandBufferBeginInfo*)" /> or not.</param>
	public SurfaceDefinition(Vk vk, Device device, PhysicalDeviceMemoryProperties physicalDeviceMemoryProperties, uint framesInFlight, Bufferization bufferization, bool resetCommandBuffers, CommandBuffer[] commandBuffers)
	{
		this.vk = vk;
		this.device = device;
		this.physicalDeviceMemoryProperties = physicalDeviceMemoryProperties;

		if (framesInFlight < 1) throw new ArgumentOutOfRangeException(nameof(framesInFlight), framesInFlight, "Invalid count of frames in flight.");
		else this.framesInFlight = framesInFlight;

		if (framesInFlight is 1 && bufferization is not Bufferization.None) throw new ArgumentException("Bufferization is not supported with 1 frame in flight.", nameof(bufferization));
		else if (bufferization is not Bufferization.None && bufferization is not Bufferization.FrameWithCopy && bufferization is not Bufferization.FrameWithoutCopy) throw new ArgumentOutOfRangeException(nameof(bufferization), bufferization, "Unknown bufferization setting.");
		else this.bufferization = bufferization;

		this.resetCommandBuffers = resetCommandBuffers;
		if (commandBuffers.Length < framesInFlight) throw new ArgumentException($"Count of Command Buffers is less than frames in fight, equal to {framesInFlight}.", nameof(commandBuffers));
		else this.commandBuffers = commandBuffers;

		for (uint i = 0; i < 10000; i++)
		{
			if (vk.TryGetInstanceExtension(vk.CurrentInstance!.Value, out debugUtils)) debugUtils!.Dispose();
		}
		vk.TryGetInstanceExtension(vk.CurrentInstance!.Value, out debugUtils);
	}

	public bool IsDisposed { get; private set; }
	public void Dispose()
	{
		if (IsDisposed) return;

		while (destroyQueue.TryDequeue(out (uint frame, SurfaceEntry entry) result)) FreeSurface(result.entry);
		// TODO: assert no surfaces are left
		surfaces.Dispose();
		debugUtils?.Dispose();
		IsDisposed = true;
	}

	SurfaceEntry CreateSurface(uint width, uint height)
	{
		static ulong AlignTo(ulong value, ulong alignment)
		{
			value -= 1ul;
			value |= alignment - 1ul;
			value += 1ul;
			return value;
		}

		var alignedRowSize = AlignTo(width * 4u, 16);
		var frameSize = alignedRowSize * height;

		Debug.Assert(alignedRowSize % 16 == 0);

		var bufferCreateInfo = new BufferCreateInfo(size: frameSize * (bufferization is not Bufferization.None ? framesInFlight : 1u), usage: BufferUsageFlags.TransferSrcBit);
		vk.CreateBuffer(device, &bufferCreateInfo, null, out var buffer).Check();

		MemoryRequirements memoryRequirements;
		vk.GetBufferMemoryRequirements(device, buffer, &memoryRequirements);

		var memoryAllocateInfo = new MemoryAllocateInfo(allocationSize: memoryRequirements.Size, memoryTypeIndex: physicalDeviceMemoryProperties.FindMemoryTypeIndex(memoryRequirements.MemoryTypeBits, MemoryPropertyFlags.HostVisibleBit | MemoryPropertyFlags.HostCoherentBit));
		vk.AllocateMemory(device, &memoryAllocateInfo, null, out var bufferMemory).Check();
		vk.BindBufferMemory(device, buffer, bufferMemory, 0).Check();

		byte* data;
		vk.MapMemory(device, bufferMemory, 0, bufferCreateInfo.Size, 0, (void**)&data).Check();

		var imageCreateInfo = new ImageCreateInfo(
			imageType: ImageType.Type2D,
			format: Format.B8G8R8A8Srgb,
			extent: new(width, height, 1u),
			mipLevels: 1,
			arrayLayers: 1,
			samples: SampleCountFlags.Count1Bit,
			usage: ImageUsageFlags.SampledBit | ImageUsageFlags.TransferDstBit);
		vk.CreateImage(device, &imageCreateInfo, null, out Image image).Check();

		vk.GetImageMemoryRequirements(device, image, out var imageMemoryRequirements);

		memoryAllocateInfo = memoryAllocateInfo with
		{
			AllocationSize = imageMemoryRequirements.Size,
			MemoryTypeIndex = physicalDeviceMemoryProperties.FindMemoryTypeIndex(imageMemoryRequirements.MemoryTypeBits, MemoryPropertyFlags.DeviceLocalBit)
		};
		vk.AllocateMemory(device, &memoryAllocateInfo, null, out DeviceMemory imageMemory).Check();
		vk.BindImageMemory(device, image, imageMemory, 0).Check();

		return new()
		{
			stagingBuffer = buffer,
			stagingMemory = bufferMemory,
			// imageLayout = ImageLayout.Undefined,
			image = image,
			imageMemory = imageMemory,
			size = (nuint)frameSize,
			mapped = data,
			width = width,
			height = height,
			rowBytes = (uint)alignedRowSize,
			latestFrame = CurrentFrame
		};
	}
	void NameResources(nint id, in SurfaceEntry entry)
	{
		if (debugUtils is null) return;

		using ULString stagingBufferName = new($"View {id} staging Buffer");
		using ULString stagingMemoryName = new($"View {id} staging Memory");
		using ULString imageName = new($"View {id} Image");
		using ULString imageMemoryName = new($"View {id} Image Memory");

		DebugUtilsObjectNameInfoEXT objectNameInfoEXT = new(pNext: null);
		debugUtils.SetDebugUtilsObjectName(device, objectNameInfoEXT with { ObjectType = ObjectType.Buffer, ObjectHandle = entry.stagingBuffer.Handle, PObjectName = stagingBufferName.data });
		debugUtils.SetDebugUtilsObjectName(device, objectNameInfoEXT with { ObjectType = ObjectType.DeviceMemory, ObjectHandle = entry.stagingMemory.Handle, PObjectName = stagingMemoryName.data });
		debugUtils.SetDebugUtilsObjectName(device, objectNameInfoEXT with { ObjectType = ObjectType.Image, ObjectHandle = entry.image.Handle, PObjectName = imageName.data });
		debugUtils.SetDebugUtilsObjectName(device, objectNameInfoEXT with { ObjectType = ObjectType.DeviceMemory, ObjectHandle = entry.imageMemory.Handle, PObjectName = imageMemoryName.data });
	}

	nint ISurfaceDefinition.Create(uint width, uint height)
	{
		var id = surfaces.GetNewId();
		surfaces[id] = CreateSurface(width, height);
		NameResources(id, surfaces[id]);
		return id;
	}
	void ISurfaceDefinition.Destroy(nint id)
	{
		var entry = surfaces[(int)id] with { latestFrame = CurrentFrame };
		destroyQueue.Enqueue((CurrentFrame, entry));
		surfaces.Remove((int)id);
	}

	void FreeSurface(SurfaceEntry entry)
	{
		vk.DestroyImage(device, entry.image, null);
		vk.FreeMemory(device, entry.imageMemory, null);
		vk.DestroyBuffer(device, entry.stagingBuffer, null);
		vk.UnmapMemory(device, entry.stagingMemory);
		vk.FreeMemory(device, entry.stagingMemory, null);
	}

	uint ISurfaceDefinition.GetWidth(nint id) => surfaces[(int)id].width;
	uint ISurfaceDefinition.GetHeight(nint id) => surfaces[(int)id].height;
	uint ISurfaceDefinition.GetRowBytes(nint id) => surfaces[(int)id].rowBytes;
	nuint ISurfaceDefinition.GetSize(nint id) => surfaces[(int)id].size;

	byte* ISurfaceDefinition.LockPixels(nint id)
	{
		ref var entry = ref surfaces[(int)id];

		if (bufferization is not Bufferization.None)
		{
			if (bufferization is Bufferization.FrameWithCopy && entry.latestFrame != CurrentFrame)
			{
				new ReadOnlySpan<byte>(entry.mapped + (entry.size * entry.latestFrame), checked((int)entry.size)) // ping me in discord, if this causes problems for you.
					.CopyTo(new Span<byte>(entry.mapped + (entry.size * CurrentFrame), checked((int)entry.size)));
			}
			entry.latestFrame = CurrentFrame;
			return entry.mapped + (entry.size * CurrentFrame);
		}
		return entry.mapped;
	}
	void ISurfaceDefinition.UnlockPixels(nint id)
	{
		ref var entry = ref surfaces[(int)id];

		//Debug.Assert(entry.latestFrame == CurrentFrame);

		var commandBuffer = GetCommandBufferForRecording();
		if (debugUtils is not null)
		{
			using ULString label = new($"Update View {id} Image");
			debugUtils.CmdBeginDebugUtilsLabel(commandBuffer, new DebugUtilsLabelEXT(pLabelName: label.data));
		}

		var imageMemoryBarrier = new ImageMemoryBarrier(
			srcAccessMask: entry.imageLayout is ImageLayout.Undefined ? AccessFlags.NoneKhr : AccessFlags.ShaderReadBit, dstAccessMask: AccessFlags.TransferWriteBit,
			oldLayout: ImageLayout.Undefined, newLayout: ImageLayout.TransferDstOptimal,
			srcQueueFamilyIndex: Vk.QueueFamilyIgnored, dstQueueFamilyIndex: Vk.QueueFamilyIgnored,
			image: entry.image, subresourceRange: new ImageSubresourceRange(ImageAspectFlags.ColorBit, 0, 1, 0, 1));
		vk.CmdPipelineBarrier(commandBuffer,
			PipelineStageFlags.FragmentShaderBit, PipelineStageFlags.TransferBit, 0,
			0, null,
			0, null,
			1, &imageMemoryBarrier);

		var bufferImageCopy = new BufferImageCopy(entry.size * (bufferization is not Bufferization.None ? CurrentFrame : 1ul), entry.rowBytes / 4, entry.height, new ImageSubresourceLayers(ImageAspectFlags.ColorBit, 0, 0, 1), imageExtent: new Extent3D(entry.width, entry.height, 1));
		vk.CmdCopyBufferToImage(commandBuffer, entry.stagingBuffer, entry.image, ImageLayout.TransferDstOptimal, 1, &bufferImageCopy);

		imageMemoryBarrier = imageMemoryBarrier with
		{
			SrcAccessMask = AccessFlags.TransferWriteBit,
			DstAccessMask = AccessFlags.ShaderReadBit,
			OldLayout = ImageLayout.TransferDstOptimal,
			NewLayout = entry.imageLayout = ImageLayout.ShaderReadOnlyOptimal
		};
		vk.CmdPipelineBarrier(commandBuffer,
			PipelineStageFlags.TransferBit, PipelineStageFlags.FragmentShaderBit, 0,
			0, null,
			0, null,
			1, &imageMemoryBarrier);

		debugUtils?.CmdEndDebugUtilsLabel(commandBuffer);
	}

	void ISurfaceDefinition.Resize(nint id, uint width, uint height)
	{
		ref var entry = ref surfaces[(int)id];
		destroyQueue.Enqueue((CurrentFrame, entry));

		// var commandBuffer = GetCommandBufferForRecording(); // if we were actually resizing it...

		entry = CreateSurface(width, height);
		NameResources(id, entry);
	}

	CommandBuffer GetCommandBufferForRecording()
	{
		var commandBuffer = commandBuffers[CurrentFrame];
		if (!commandBufferBegun)
		{
			if (resetCommandBuffers) vk.ResetCommandBuffer(commandBuffer, 0).Check();
			var commandBufferInheritanceInfo = new CommandBufferInheritanceInfo(pNext: null);
			var commandBufferBeginInfo = new CommandBufferBeginInfo(flags: CommandBufferUsageFlags.OneTimeSubmitBit, pInheritanceInfo: &commandBufferInheritanceInfo);
			vk.BeginCommandBuffer(commandBuffer, &commandBufferBeginInfo).Check();

			commandBufferBegun = true;
		};
		return commandBuffer;
	}
	public bool TryGetCommandBufferForSubmission(out CommandBuffer commandBuffer)
	{
		if (!commandBufferBegun) { commandBuffer = default; return false; }
		else commandBufferBegun = false;

		commandBuffer = commandBuffers[CurrentFrame];
		vk.EndCommandBuffer(commandBuffer).Check();
		return true;
	}

	public Image GetImage(nint id) => surfaces[(int)id].image;

	[StructLayout(LayoutKind.Auto)]
	private struct SurfaceEntry
	{
		public Buffer stagingBuffer;
		public DeviceMemory stagingMemory;
		public ImageLayout imageLayout;
		public Image image;
		public DeviceMemory imageMemory;
		// no ImageView because we don't have enough information upfront about transparency (ComponentMapping controls that, and i don't want to enable it, if it's not used.)
		public nuint size;
		public byte* mapped;
		public uint width, height, rowBytes;
		public uint latestFrame;
	}
}
