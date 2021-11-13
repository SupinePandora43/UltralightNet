using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULGPUDriverUpdateTextureCallback__PInvoke__(
		uint texture_id,
		IntPtr bitmap
	);
	public delegate void ULGPUDriverUpdateTextureCallback(
		uint texture_id,
		ULBitmap bitmap
	);
}
