using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ULLogger : IDisposable
	{
		public ULLoggerLogMessageCallback? LogMessage
		{
			set => _LogMessage = value is null ? null : (level, msg) => value(level, ULString.NativeToManaged(msg));
			get
			{
				var c = _LogMessage;
				return c is null ? null : (ULLogLevel level, in string message) =>
				{
					using ULStringDisposableStackToNativeMarshaller marshaller = new(message);
					c(level, marshaller.Native);
				};
			}
		}

		public ULLoggerLogMessageCallback__PInvoke__? _LogMessage
		{
			set
			{
				if (value is null) ULPlatform.Handle<ULLoggerLogMessageCallback__PInvoke__>(ref this, this with { __LogMessage = null });
				else ULPlatform.Handle<ULLoggerLogMessageCallback__PInvoke__>(ref this, this with { __LogMessage = (delegate* unmanaged[Cdecl]<ULLogLevel, ULString*, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __LogMessage;
				return p is null ? null : (ULLogLevel level, ULString* message) => p(level, message);
			}
		}

		public delegate* unmanaged[Cdecl]<ULLogLevel, ULString*, void> __LogMessage;

		public void Dispose()
		{
			ULPlatform.Free(this);
		}
	}
}
