using System.Runtime.InteropServices;

namespace UltralightNet;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void ULGPUDriverUpdateTextureCallback__PInvoke__(
	uint textureId,
	void* bitmap
);
public delegate void ULGPUDriverUpdateTextureCallback(
	uint textureId,
	ULBitmap bitmap
);
