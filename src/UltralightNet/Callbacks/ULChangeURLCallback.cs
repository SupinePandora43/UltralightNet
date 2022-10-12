using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULChangeURLCallback__PInvoke__(
		IntPtr user_data,
		IntPtr caller,
		ULString* url
	);
	public delegate void ULChangeURLCallback(
		IntPtr user_data,
		View caller,
		string url
	);
	public delegate void ULChangeURLCallbackEvent(
		string url
	);
}
