using System.Runtime.InteropServices;

namespace UltralightNet
{
	public delegate void ULClipboardWritePlainTextCallback(
		[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
		string text
	);
}
