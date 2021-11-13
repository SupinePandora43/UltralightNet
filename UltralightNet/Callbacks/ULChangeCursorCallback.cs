using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULChangeCursorCallback__PInvoke__(
		IntPtr user_data,
		IntPtr caller,
		ULCursor cursor
	);
	public delegate void ULChangeCursorCallback(
		IntPtr user_data,
		View caller,
		ULCursor cursor
	);
	public delegate void ULChangeCursorCallbackEvent(
		ULCursor cursor
	);
}
