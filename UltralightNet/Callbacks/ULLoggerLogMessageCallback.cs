using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULLoggerLogMessageCallback__PInvoke__(
		ULLogLevel log_level,
		IntPtr message
	);
	public delegate void ULLoggerLogMessageCallback(
		ULLogLevel log_level,
		string message
	);
}
