using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULAddConsoleMessageCallback(
		IntPtr user_data,
		[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(View.Marshaler))]
		View caller,
		[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
		string source,
		uint line_number,
		uint column_number,
		[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
		string source_id
	);
}
