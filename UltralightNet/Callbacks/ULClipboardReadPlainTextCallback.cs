using System.Runtime.InteropServices;

namespace UltralightNet
{
	public delegate void ULClipboardReadPlainTextCallback(out string result);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULClipboardReadPlainTextCallback__PInvoke__(
		ULString* result
	);
}
