using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULFailLoadingCallback__PInvoke__(
		IntPtr user_data,
		IntPtr caller,
		ulong frame_id,
		byte is_main_frame,
		ULString* url,
		ULString* description,
		ULString* error_domain,
		int error_code
	);
	public delegate void ULFailLoadingCallback(
		IntPtr user_data,
		View caller,
		ulong frame_id,
		bool is_main_frame,
		string url,
		string description,
		string error_domain,
		int error_code
	);
	public delegate void ULFailLoadingCallbackEvent(
		ulong frameId,
		bool isMainFrame,
		string url,
		string description,
		string errorDomain,
		int errorCode
	);
}
