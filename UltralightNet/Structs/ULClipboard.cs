using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ULClipboard : IDisposable
	{
		public ULClipboardClearCallback Clear
		{
			set
			{
				ULClipboardClearCallback callback = value;
				ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
				__Clear = (delegate* unmanaged[Cdecl]<void>)Marshal.GetFunctionPointerForDelegate(callback);
			}
		}
		public ULClipboardReadPlainTextCallback ReadPlainText
		{
			set
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
		public ULClipboardReadPlainTextCallback__PInvoke__ _ReadPlainText
		{
			set
			{
				ULClipboardReadPlainTextCallback__PInvoke__ callback = value;
				ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
				__ReadPlainText = (delegate* unmanaged[Cdecl]<ULString*, void>)Marshal.GetFunctionPointerForDelegate(callback);
			}
		}
		public ULClipboardWritePlainTextCallback WritePlainText
		{
			set
			{
				unsafe
				{
					_WritePlainText = (text) => value(ULString.NativeToManaged(text));
				}
			}
		}
		public ULClipboardWritePlainTextCallback__PInvoke__ _WritePlainText
		{
			set
			{
				ULClipboardWritePlainTextCallback__PInvoke__ callback = value;
				ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
				__WritePlainText = (delegate* unmanaged[Cdecl]<ULString*, void>)Marshal.GetFunctionPointerForDelegate(callback);
			}
		}

		public delegate* unmanaged[Cdecl]<void> __Clear;
		public delegate* unmanaged[Cdecl]<ULString*, void> __ReadPlainText;
		public delegate* unmanaged[Cdecl]<ULString*, void> __WritePlainText;

		public void Dispose()
		{
			ULPlatform.Free(this);
		}
	}
}
