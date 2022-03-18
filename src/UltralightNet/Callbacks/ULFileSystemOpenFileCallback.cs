using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate nuint ULFileSystemOpenFileCallback__PInvoke__(
		ULString* path,
		[MarshalAs(UnmanagedType.I1)]
		bool openForWriting
	);
	public delegate nuint ULFileSystemOpenFileCallback(
		string path,
		bool openForWriting
	);
}
