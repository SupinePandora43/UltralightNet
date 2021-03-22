using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		[DllImport("Ultralight")]
		public static extern void ulPlatformSetLogger(ULLogger logger);

		[DllImport("Ultralight")]
		public static extern void ulPlatformSetFileSystem(ULFileSystem file_system);

		// [DllImport("Ultralight")]
		// public static extern void ulPlatformSetSurfaceDefinition(ULSurfaceDefinition surface_definition);

		// [DllImport("Ultralight")]
		// public static extern void ulPlatformSetGPUDriver(ULGPUDriver gpu_driver);

		[DllImport("Ultralight")]
		public static extern void ulPlatformSetClipboard(ULClipboard clipboard);
	}

	public static class ULPlatform
	{
		public static void SetLogger(ULLogger logger) => Methods.ulPlatformSetLogger(logger);
		public static void SetFileSystem(ULFileSystem fileSystem) => Methods.ulPlatformSetFileSystem(fileSystem);
	}
}
