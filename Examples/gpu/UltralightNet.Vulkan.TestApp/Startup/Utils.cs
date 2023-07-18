using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Silk.NET.Core;
using Silk.NET.Vulkan;

internal unsafe static class Utils
{
	public static void Check(this Result result)
	{
		if (result is not Result.Success) throw new Exception("Result is not OK");
	}
	private static byte* ToPointer(this ReadOnlySpan<byte> span) => (byte*)Unsafe.AsPointer(ref Unsafe.AsRef(in span[0]));

	private static bool HasInstanceLayer(this Vk vk, ReadOnlySpan<byte> layer)
	{
		uint propertyCount = 0;
		vk.EnumerateInstanceLayerProperties(ref propertyCount, null).Check();
		var properties = stackalloc LayerProperties[(int)propertyCount];
		vk.EnumerateInstanceLayerProperties(ref propertyCount, properties).Check();
		for (int i = 0; i < propertyCount; i++)
			if (layer.SequenceEqual(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(properties[i].LayerName))) return true;
		return false;
	}
	public static void CreateInstance(Vk vk, uint extensionCount, byte** extensions, out Instance instance)
	{
		ApplicationInfo applicationInfo = new(
			pApplicationName: "VulkanExample"u8.ToPointer(),
			apiVersion: Vk.Version11
		);

		ReadOnlySpan<byte> validationLayer = "VK_LAYER_KHRONOS_validation"u8;
		byte** layers = stackalloc byte*[1] { ToPointer(validationLayer) };

		delegate* unmanaged[Cdecl]<DebugUtilsMessageSeverityFlagsEXT, DebugUtilsMessageTypeFlagsEXT, DebugUtilsMessengerCallbackDataEXT*, void*, Bool32> a = &DebugLogger;
		DebugUtilsMessengerCreateInfoEXT debugUtilsMessengerCreateInfo = new(
			messageSeverity: DebugUtilsMessageSeverityFlagsEXT.DebugUtilsMessageSeverityWarningBitExt | DebugUtilsMessageSeverityFlagsEXT.DebugUtilsMessageSeverityWarningBitExt,
			messageType: DebugUtilsMessageTypeFlagsEXT.DebugUtilsMessageTypeValidationBitExt,
			pfnUserCallback: (delegate* unmanaged[Cdecl]<DebugUtilsMessageSeverityFlagsEXT, DebugUtilsMessageTypeFlagsEXT, DebugUtilsMessengerCallbackDataEXT*, void*, Bool32>)&DebugLogger
		);

		vk.CreateInstance(new InstanceCreateInfo(
			pNext: vk.HasInstanceLayer(validationLayer) ? &debugUtilsMessengerCreateInfo : null,
			pApplicationInfo: &applicationInfo,
			enabledLayerCount: vk.HasInstanceLayer(validationLayer) ? 1u : 0u,
			ppEnabledLayerNames: layers,
			enabledExtensionCount: 0,
			ppEnabledExtensionNames: extensions
		), null, out instance).Check();
	}

	[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
	private static Bool32 DebugLogger(DebugUtilsMessageSeverityFlagsEXT messageSeverity, DebugUtilsMessageTypeFlagsEXT messageTypes, DebugUtilsMessengerCallbackDataEXT* data, void* _)
	{
		Console.WriteLine(Marshal.PtrToStringUTF8((nint)data->PMessage));
		return Vk.False;
	}
}
