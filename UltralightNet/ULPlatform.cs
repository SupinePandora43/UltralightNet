using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		[DllImport("Ultralight")]
		public static extern void ulPlatformSetLogger(ULLogger logger);

		[DllImport("Ultralight")]
		public static extern void ulPlatformSetFileSystem(ULFileSystem file_system);
	}

	public static class ULPlatform
	{
		public static void SetLogger(ULLogger logger) => Methods.ulPlatformSetLogger(logger);
		public static void SetFileSystem(ULFileSystem fileSystem) => Methods.ulPlatformSetFileSystem(fileSystem);
	}
}
