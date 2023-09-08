using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using UltralightNet;
using Buffer = Silk.NET.Vulkan.Buffer;

internal unsafe static class Utils
{
	const bool Debug =
#if DEBUG
	true;
#else
	false;
#endif

	public static void Check(this Result result)
	{
		if (result is not Result.Success) throw new Exception($"Result is {result}");
	}
	public static void Assert(this bool condition)
	{
		if (!condition) throw new Exception("Assertion failed");
	}

	public static uint FindMemoryTypeIndex(this PhysicalDeviceMemoryProperties physicalDeviceMemoryProperties, uint memoryTypeBits, MemoryPropertyFlags memoryPropertyFlags)
	{
		for (int i = 0; i < physicalDeviceMemoryProperties.MemoryTypeCount; i++)
			if ((memoryTypeBits & (1 << i)) != 0 && physicalDeviceMemoryProperties.MemoryTypes[i].PropertyFlags.HasFlag(memoryPropertyFlags))
				return (uint)i;
		throw new Exception($"Memory not found: {memoryTypeBits}, {memoryPropertyFlags}");
	}
	public static ulong AlignTo(this ulong number, ulong alignment)
	{
		number -= 1;
		number |= alignment - 1;
		number += 1;
		return number;
	}

	public static void SetDebugUtilsObjectName<T>(this ExtDebugUtils debugUtils, Device device, T vulkanObject, ReadOnlySpan<char> name) where T : unmanaged
	{
		using ULString utf8Name = new(name);
		debugUtils.SetDebugUtilsObjectName(device, new DebugUtilsObjectNameInfoEXT(objectType:
			typeof(T) == typeof(DeviceMemory) ? ObjectType.DeviceMemory :
			typeof(T) == typeof(Buffer) ? ObjectType.Buffer :
			typeof(T) == typeof(Image) ? ObjectType.Image :
			typeof(T) == typeof(ImageView) ? ObjectType.ImageView :
			typeof(T) == typeof(DescriptorSet) ? ObjectType.DescriptorSet
			: ObjectType.Unknown,
			objectHandle: Unsafe.As<T, ulong>(ref vulkanObject), pObjectName: utf8Name.data)).Check();
	}
	public static byte* ToPointer(this ReadOnlySpan<byte> span) => (byte*)Unsafe.AsPointer(ref Unsafe.AsRef(in span[0]));

	public static void CreateInstance(Vk vk, string[] extensions, out Instance instance)
	{
		ApplicationInfo applicationInfo = new(
			pApplicationName: "VulkanExample"u8.ToPointer(),
			apiVersion: Vk.Version11
		);

		void* next = null;

		if (Debug)
		{
			DebugUtilsMessengerCreateInfoEXT debugUtilsMessengerCreateInfo = new(
				messageSeverity: DebugUtilsMessageSeverityFlagsEXT.DebugUtilsMessageSeverityWarningBitExt,
				messageType: DebugUtilsMessageTypeFlagsEXT.DebugUtilsMessageTypeValidationBitExt | DebugUtilsMessageTypeFlagsEXT.DebugUtilsMessageTypeGeneralBitExt | DebugUtilsMessageTypeFlagsEXT.DebugUtilsMessageTypePerformanceBitExt,
				pfnUserCallback: (delegate* unmanaged[Cdecl]<DebugUtilsMessageSeverityFlagsEXT, DebugUtilsMessageTypeFlagsEXT, DebugUtilsMessengerCallbackDataEXT*, void*, Bool32>)&DebugLogger
			);
			next = &debugUtilsMessengerCreateInfo;
		}
		byte** extensionsPtr = (byte**)SilkMarshal.StringArrayToPtr(extensions);
		try
		{
			vk.CreateInstance(new InstanceCreateInfo(
				pNext: next,
				pApplicationInfo: &applicationInfo,
				enabledLayerCount: 0u,
				ppEnabledLayerNames: null,
				enabledExtensionCount: (uint)extensions.Length,
				ppEnabledExtensionNames: extensionsPtr
			), null, out instance).Check();
		}
		finally
		{
			for (uint i = 0; i < extensions.Length; i++) SilkMarshal.FreeString((nint)extensionsPtr[i]);
			SilkMarshal.Free((nint)extensionsPtr);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
	private static Bool32 DebugLogger(DebugUtilsMessageSeverityFlagsEXT messageSeverity, DebugUtilsMessageTypeFlagsEXT messageTypes, DebugUtilsMessengerCallbackDataEXT* data, void* _)
	{
		Console.WriteLine(Marshal.PtrToStringUTF8((nint)data->PMessage));
		return Vk.False;
	}
}
