using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULChangeURLCallback__PInvoke__(
		IntPtr user_data,
		IntPtr caller,
		IntPtr url
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
