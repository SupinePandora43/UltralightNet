using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate bool ULFileSystemFileExistsCallback(
		[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
		string path
	);
}
