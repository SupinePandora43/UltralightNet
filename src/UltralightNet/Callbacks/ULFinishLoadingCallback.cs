using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULFinishLoadingCallback__PInvoke__(
		IntPtr user_data,
		IntPtr caller,
		ulong frame_id,
		byte is_main_frame,
		ULString* url
	);
	public delegate void ULFinishLoadingCallback(
		IntPtr user_data,
		View caller,
		ulong frame_id,
		bool is_main_frame,
		string url
	);
	public delegate void ULFinishLoadingCallbackEvent(
		ulong frameId,
		bool isMainFrame,
		string url
	);
}
