using System.Runtime.InteropServices;

namespace UltralightNet
{
	//todo: long long
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate long ULFileSystemReadFromFileCallback(
		int handle,
		out string data,
		long length
	);
}
