using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULAddConsoleMessageCallback__PInvoke__(
		IntPtr user_data,
		IntPtr caller,
		ULMessageSource source,
		ULMessageLevel level,
		IntPtr message,
		uint line_number,
		uint column_number,
		IntPtr source_id
	);
	public delegate void ULAddConsoleMessageCallback(
		IntPtr user_data,
		View caller,
		ULMessageSource source,
		ULMessageLevel level,
		string message,
		uint line_number,
		uint column_number,
		string source_id
	);
	public delegate void ULAddConsoleMessageCallbackEvent(
		ULMessageSource source,
		ULMessageLevel level,
		string message,
		uint line_number,
		uint column_number,
		string source_id
	);
}
