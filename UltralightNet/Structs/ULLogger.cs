namespace UltralightNet
{
	public struct ULLogger
	{
		public ULLoggerLogMessageCallback log_message;

		public ULLogger(ULLoggerLogMessageCallback callback) => log_message = callback;
	}
}
