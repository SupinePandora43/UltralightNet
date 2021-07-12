using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate long ULFileSystemReadFromFileCallback__PInvoke__(
		int handle,
		byte* data,
		long length
	);
	public delegate long ULFileSystemReadFromFileCallback(
		int handle,
		Span<byte> data,
		long length
	);
}
