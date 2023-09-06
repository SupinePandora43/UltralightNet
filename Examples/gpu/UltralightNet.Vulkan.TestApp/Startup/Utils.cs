using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;

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
