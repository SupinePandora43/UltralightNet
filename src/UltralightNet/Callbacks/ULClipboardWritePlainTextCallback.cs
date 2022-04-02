using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public delegate void ULClipboardWritePlainTextCallback(
		in string text
	);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULClipboardWritePlainTextCallback__PInvoke__(
		ULString* text
	);
}
