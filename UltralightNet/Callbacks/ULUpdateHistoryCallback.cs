using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULUpdateHistoryCallback__PInvoke__(
		IntPtr user_data,
		IntPtr caller
	);
	public delegate void ULUpdateHistoryCallback(
		IntPtr user_data,
		View caller
	);
	public delegate void ULUpdateHistoryCallbackEvent();
}
