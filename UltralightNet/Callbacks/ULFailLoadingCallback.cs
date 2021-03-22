using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULFailLoadingCallback(
		IntPtr user_data,
		[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(View.Marshaler))]
		View caller,
		ulong frame_id,
		bool is_main_frame,
		[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
		string url,
		[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
		string description,
		[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
		string error_domain,
		int error_code
	);
}
