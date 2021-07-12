using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	// todo: test result
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public unsafe delegate bool ULFileSystemGetFileMimeTypeCallback__PInvoke__(
		ULString* path,
		ULString* result
	);
	public delegate bool ULFileSystemGetFileMimeTypeCallback(
		string path,
		out string result
	);
}
