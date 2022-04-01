using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate byte ULFileSystemGetFileSizeCallback__PInvoke__(
		nuint fileHandle,
		long* result
	);
	public delegate bool ULFileSystemGetFileSizeCallback(
		nuint fileHandle,
		out long result
	);
}
