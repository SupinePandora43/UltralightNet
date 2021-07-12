using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULLoggerLogMessageCallback__PInvoke__(
		ULLogLevel log_level,
		ULString* message
	);
	public delegate void ULLoggerLogMessageCallback(
		ULLogLevel log_level,
		string message
	);
}
