using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public struct ULLogger
	{
		public ULLoggerLogMessageCallback LogMessage { set => log_message = (level, msg) => value(level, ULStringMarshaler.NativeToManaged(msg)); }

		private ULLoggerLogMessageCallback__PInvoke__ log_message;
	}
}
