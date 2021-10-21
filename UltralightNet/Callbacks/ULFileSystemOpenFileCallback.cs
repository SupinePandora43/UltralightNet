using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate int ULFileSystemOpenFileCallback__PInvoke__(
		ULString* path,
		[MarshalAs(UnmanagedType.I1)]
		bool openForWriting
	);
	public delegate int ULFileSystemOpenFileCallback(
		string path,
		bool openForWriting
	);
}
