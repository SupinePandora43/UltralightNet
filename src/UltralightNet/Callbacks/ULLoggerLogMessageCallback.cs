using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULLoggerLogMessageCallback__PInvoke__(
		ULLogLevel logLevel,
		ULString* message
	);
	public delegate void ULLoggerLogMessageCallback(
		ULLogLevel logLevel,
		string message
	);
}
