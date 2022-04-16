using System.IO;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate long ULFileSystemReadFromFileCallback__PInvoke__(
		nuint handle,
		byte* data,
		long length
	);
	public delegate long ULFileSystemReadFromFileCallback(
		nuint handle,
		in UnmanagedMemoryStream data
	);
}
