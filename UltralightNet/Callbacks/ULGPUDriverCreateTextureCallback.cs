using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULGPUDriverCreateTextureCallback__PInvoke__(
		uint texture_id,
		IntPtr bitmap
	);
	public delegate void ULGPUDriverCreateTextureCallback(
		uint texture_id,
		ULBitmap bitmap
	);
}
