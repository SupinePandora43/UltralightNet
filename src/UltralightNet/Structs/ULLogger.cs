using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace UltralightNet;

public unsafe struct ULLogger : IDisposable, IEquatable<ULLogger>
{
	public ULLoggerLogMessageCallback? LogMessage
	{
		set => _LogMessage = value is null ? null : (level, msg) => value(level, ULString.NativeToManaged(msg));
		readonly get
		{
			var c = _LogMessage;
			return c is null ? null : (ULLogLevel level, in string message) =>
			{
				using ULString str = new(message.AsSpan());
				c(level, &str);
			};
		}
	}

	public ULLoggerLogMessageCallback__PInvoke__? _LogMessage
	{
		set => ULPlatform.Handle(ref this, this with { __LogMessage = value is null ? null : (delegate* unmanaged[Cdecl]<ULLogLevel, ULString*, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
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

#pragma warning disable CS8909
	public readonly bool Equals(ULLogger logger) => __LogMessage == logger.__LogMessage;
#pragma warning restore CS8909

	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is ULLogger logger ? Equals(logger) : false;

	public readonly override int GetHashCode() =>
#if NETSTANDARD2_1 || NETCOREAPP2_1_OR_GREATER
		HashCode.Combine((nuint)__LogMessage);
#else
		unchecked((int)(nuint)__LogMessage);
#endif
}
