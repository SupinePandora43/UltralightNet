using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate bool ULFileSystemGetFileSizeCallback(
		int fileHandle,
		out long result
	);
}
