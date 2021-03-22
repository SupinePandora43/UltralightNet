using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	// todo: out string
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULClipboardReadPlainTextCallback(IntPtr result);
}
