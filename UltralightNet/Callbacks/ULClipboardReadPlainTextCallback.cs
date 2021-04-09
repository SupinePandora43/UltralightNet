using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	// todo: out string
	/// <summary>
	/// 
	/// </summary>
	/// <param name="result">pointer to write to</param>
	/// <example>
	/// ULStringMarshaler.ManagedToNative("idk", result);
	/// </example>
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULClipboardReadPlainTextCallback(IntPtr result);
}
