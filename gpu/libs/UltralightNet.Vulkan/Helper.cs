using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace UltralightNet.GPU.Vulkan;

internal static class Helper
{
	public static void Check(this Result result)
	{
		if (result is not Result.Success) throw new Exception($"Result is {result}");
	}

	public unsafe static T* AsPointer<T>(this ReadOnlySpan<T> span) where T : unmanaged => (T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));

	public static uint FindMemoryTypeIndex(this PhysicalDeviceMemoryProperties physicalDeviceMemoryProperties, uint memoryTypeBits, MemoryPropertyFlags memoryPropertyFlags)
	{
		for (int i = 0; i < physicalDeviceMemoryProperties.MemoryTypeCount; i++)
			if ((memoryTypeBits & (1 << i)) != 0 && physicalDeviceMemoryProperties.MemoryTypes[i].PropertyFlags.HasFlag(memoryPropertyFlags))
			{
				return (uint)i;
			}
		throw new Exception($"Memory not found: {memoryTypeBits}, {memoryPropertyFlags}");
	}
}
