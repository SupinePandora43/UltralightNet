using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULChangeTooltipCallback__PInvoke__(
		IntPtr user_data,
		IntPtr caller,
		IntPtr tooltip
	);
	public delegate void ULChangeTooltipCallback(
		IntPtr user_data,
		View caller,
		string tooltip
	);
	public delegate void ULChangeTooltipCallbackEvent(
		string tooltip
	);
}
