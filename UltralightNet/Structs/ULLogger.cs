using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public struct ULLogger
	{
		public ULLoggerLogMessageCallback LogMessage { set { unsafe { _LogMessage = (level, msg) => value(level, ULString.NativeToManaged(msg)); } } }

		public ULLoggerLogMessageCallback__PInvoke__ _LogMessage;
	}

	/// <summary>
	/// <see cref="ULLogger"/> with delegate* types
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct _ULLogger
	{
		/// <example>
		///	ULStringGeneratedDllImportMarshaler marshaler = default;
		///	marshaler.Value = message;
		///	string messageString = marshaler.ToManaged();
		/// </example>
		public delegate* unmanaged[Cdecl]<ULLogLevel, ULString*, void> LogMessage;
	}
}
