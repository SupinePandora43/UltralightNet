using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate long ULFileSystemReadFromFileCallback(
		int handle,
		out string data,
		long length
	);
}
