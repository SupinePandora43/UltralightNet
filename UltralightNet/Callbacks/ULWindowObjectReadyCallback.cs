using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULWindowObjectReadyCallback__PInvoke__(
		IntPtr user_data,
		IntPtr caller,
		ulong frame_id,
		byte is_main_frame,
		IntPtr url
	);
	public delegate void ULWindowObjectReadyCallback(
		IntPtr user_data,
		View caller,
		ulong frame_id,
		bool is_main_frame,
		string url
	);
	public delegate void ULWindowObjectReadyCallbackEvent(
		ulong frameId,
		bool isMainFrame,
		string url
	);
}
