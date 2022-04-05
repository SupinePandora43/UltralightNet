using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULGPUDriverCreateTextureCallback__PInvoke__(
		uint texture_id,
		void* bitmap
	);
	public delegate void ULGPUDriverCreateTextureCallback(
		uint texture_id,
		ULBitmap bitmap
	);
}
