using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public struct ULLogger
	{
		public ULLoggerLogMessageCallback LogMessage { set => _LogMessage = (level, msg) => value(level, ULStringMarshaler.NativeToManaged(msg)); }

		private ULLoggerLogMessageCallback__PInvoke__ _LogMessage;
	}
}
