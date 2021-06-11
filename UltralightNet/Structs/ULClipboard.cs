using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public struct ULClipboard
	{
		public ULClipboardClearCallback Clear;
		public ULClipboardReadPlainTextCallback__PInvoke__ _ReadPlainText;
		public ULClipboardWritePlainTextCallback__PInvoke__ _WritePlainText;

		/// <summary>
		/// untested
		/// </summary>
		public ULClipboardReadPlainTextCallback ReadPlainText { set
			{
				unsafe
				{
					_ReadPlainText = (result) =>
					{
						value(out string managedResult);

						result->data = (ushort*)Marshal.StringToHGlobalUni(managedResult);
						result->length = (nuint)managedResult.Length;
					};
				}
			}
		}

		public ULClipboardWritePlainTextCallback WritePlainText { set {
				unsafe {
					_WritePlainText = (text) => value(ULString.NativeToManaged(text));
				}
			}
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct _ULClipboard
	{
		public delegate* unmanaged[Cdecl]<void> Clear;
		public delegate* unmanaged[Cdecl]<ULStringMarshaler.ULStringPTR*, void> ReadPlainText;
		public delegate* unmanaged[Cdecl]<ULStringMarshaler.ULStringPTR*, void> WritePlainText;
	}
}
