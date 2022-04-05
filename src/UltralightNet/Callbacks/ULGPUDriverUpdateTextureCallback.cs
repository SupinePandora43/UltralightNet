using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULGPUDriverUpdateTextureCallback__PInvoke__(
		uint texture_id,
		void* bitmap
	);
	public delegate void ULGPUDriverUpdateTextureCallback(
		uint texture_id,
		ULBitmap bitmap
	);
}
