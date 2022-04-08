using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ULClipboard : IDisposable
	{
		public ULClipboardClearCallback? Clear
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __Clear = null });
				else ULPlatform.Handle(ref this, this with { __Clear = (delegate* unmanaged[Cdecl]<void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __Clear;
				return p is null ? null : () => p();
			}
		}
		public ULClipboardReadPlainTextCallback? ReadPlainText
		{
			set => _ReadPlainText = value is null ? null : (result) =>
				{
					value(out string managedResult);

					using ULStringDisposableStackToNativeMarshaller marshaller = new(managedResult);
					Methods.ulStringAssignString(result, marshaller.Native);
				};
			get
			{
				var c = _ReadPlainText;
				return c is null ? null : (out string managedResult) =>
				{
					using ULStringDisposableStackToNativeMarshaller marshaller = new(string.Empty);
					c(marshaller.Native);
					managedResult = ULString.NativeToManaged(marshaller.Native);
				};
			}
		}
		public ULClipboardReadPlainTextCallback__PInvoke__? _ReadPlainText
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __ReadPlainText = null });
				else ULPlatform.Handle(ref this, this with { __ReadPlainText = (delegate* unmanaged[Cdecl]<ULString*, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __ReadPlainText;
				return p is null ? null : (result) => p(result);
			}
		}
		public ULClipboardWritePlainTextCallback? WritePlainText
		{
			set => _WritePlainText = value is null ? null : (text) => value(ULString.NativeToManaged(text));
			get
			{
				var c = _WritePlainText;
				return c is null ? null : (in string text) =>
				{
					using ULStringDisposableStackToNativeMarshaller marshaller = new(text);
					c(marshaller.Native);
				};
			}
		}
		public ULClipboardWritePlainTextCallback__PInvoke__? _WritePlainText
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __WritePlainText = null });
				else ULPlatform.Handle(ref this, this with { __WritePlainText = (delegate* unmanaged[Cdecl]<ULString*, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __WritePlainText;
				return p is null ? null : (text) => p(text);
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
