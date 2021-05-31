using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public struct ULClipboard
	{
		public ULClipboardClearCallback Clear;
		public ULClipboardReadPlainTextCallback ReadPlainText;
		public ULClipboardWritePlainTextCallback__PInvoke__ _WritePlainText;

		public ULClipboardWritePlainTextCallback WritePlainText { set { unsafe { _WritePlainText = (text) => value(ULStringMarshaler.NativeToManaged(text)); } } }
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct _ULClipboard
	{
		public delegate* unmanaged[Cdecl]<void> Clear;
		public delegate* unmanaged[Cdecl]<void*, void> ReadPlainText;
		public delegate* unmanaged[Cdecl]<ULStringMarshaler.ULStringPTR*, void> WritePlainText;
	}
}
