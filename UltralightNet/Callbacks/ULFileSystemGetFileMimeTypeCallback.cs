using System.Runtime.InteropServices;

namespace UltralightNet
{
	// todo: test result
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate bool ULFileSystemGetFileMimeTypeCallback(
		[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
		string path,
		[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
		out string result
	);
}
