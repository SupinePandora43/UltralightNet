using System.Runtime.InteropServices;
using UltralightNet.LowStuff;

namespace UltralightNet;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void ULGPUDriverUpdateTextureCallback__PInvoke__(
	uint textureId,
	Handle<ULBitmap> bitmap
);
public delegate void ULGPUDriverUpdateTextureCallback(
	uint textureId,
	ULBitmap bitmap
);
