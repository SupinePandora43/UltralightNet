using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate nuint ULFileSystemOpenFileCallback__PInvoke__(
		ULString* path,
		byte openForWriting
	);
	public delegate nuint ULFileSystemOpenFileCallback(
		in string path,
		bool openForWriting
	);
}
