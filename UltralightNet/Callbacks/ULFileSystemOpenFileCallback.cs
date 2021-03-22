using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int ULFileSystemOpenFileCallback(
		[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
		string path,
		bool open_for_writing
	);
}
