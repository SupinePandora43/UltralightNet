using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULChangeTitleCallback__PInvoke__(
		IntPtr user_data,
		IntPtr caller,
		ULString* title
	);
	public delegate void ULChangeTitleCallback(
		IntPtr user_data,
		View caller,
		string title
	);
	public delegate void ULChangeTitleCallbackEvent(
		string title
	);
}
