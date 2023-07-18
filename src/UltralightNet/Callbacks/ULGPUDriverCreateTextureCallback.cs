using System.Runtime.InteropServices;

namespace UltralightNet;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void ULGPUDriverCreateTextureCallback__PInvoke__(
	uint textureId,
	void* bitmap
);
public delegate void ULGPUDriverCreateTextureCallback(
	uint textureId,
	ULBitmap bitmap
);
