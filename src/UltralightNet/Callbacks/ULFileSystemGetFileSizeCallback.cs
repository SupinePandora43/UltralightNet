using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	[return: MarshalAs(UnmanagedType.I1)]
	public delegate bool ULFileSystemGetFileSizeCallback(
		nuint fileHandle,
		out long result
	);
}
