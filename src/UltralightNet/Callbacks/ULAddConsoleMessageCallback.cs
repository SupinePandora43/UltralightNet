using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULAddConsoleMessageCallback__PInvoke__(
		IntPtr user_data,
		IntPtr caller,
		ULMessageSource source,
		ULMessageLevel level,
		ULString* message,
		uint line_number,
		uint column_number,
		ULString* source_id
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
