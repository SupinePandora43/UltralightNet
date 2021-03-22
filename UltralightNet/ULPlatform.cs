using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		[DllImport("Ultralight")]
		public static extern void ulPlatformSetLogger(ULLogger logger);
	}
	public static class ULPlatform
	{
		public static void SetLogger(ULLogger logger) => Methods.ulPlatformSetLogger(logger);
	}
}
