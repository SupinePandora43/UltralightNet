using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULUpdateHistoryCallback(
		IntPtr user_data,
		[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(View.Marshaler))]
		View caller
	);
}
