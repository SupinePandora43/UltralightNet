using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	// todo: test result
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public unsafe delegate byte ULFileSystemGetFileMimeTypeCallback__PInvoke__(
		ULString* path,
		ULString* result
	);
	public delegate bool ULFileSystemGetFileMimeTypeCallback(
		in string path,
		out string result
	);
}
