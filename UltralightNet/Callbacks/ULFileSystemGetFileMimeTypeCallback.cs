using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	// todo: test result
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public delegate bool ULFileSystemGetFileMimeTypeCallback(
		/*[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
		string path,
		[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
		out string result*/
		IntPtr path,
		IntPtr result
	);
}
