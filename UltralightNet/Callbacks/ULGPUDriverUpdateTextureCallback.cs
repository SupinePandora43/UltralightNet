using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULGPUDriverUpdateTextureCallback(
		uint texture_id,
		[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULBitmap.Marshaler))]
		ULBitmap bitmap
	);
}
