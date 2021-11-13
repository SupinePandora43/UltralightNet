using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate IntPtr ULCreateChildViewCallback__PInvoke__(
		IntPtr user_data,
		IntPtr caller,
		ULString* opener_url,
		ULString* target_url,
		byte is_popup,
		ULIntRect popup_rect
	);
	public delegate View ULCreateChildViewCallback(
		IntPtr user_data,
		View caller,
		string opener_url,
		string target_url,
		bool is_popup,
		ULIntRect popup_rect
	);
	public delegate View ULCreateChildViewCallbackEvent(
		string openerUrl,
		string targetUrl,
		bool isPopup,
		ULIntRect popupRect
	);
}
