using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		/// <see cref="ULPlatform"/>
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "<Pending>")]
	public static class ULPlatform
	{
		[DllImport("Ultralight", EntryPoint = "ulPlatformSetLogger")]
		public static extern void SetLogger(ULLogger logger);
		[DllImport("Ultralight", EntryPoint = "ulPlatformSetLogger")]
		public static extern void SetLogger(_ULLogger logger);

		[DllImport("Ultralight", EntryPoint = "ulPlatformSetFileSystem")]
		public static extern void SetFileSystem(ULFileSystem file_system);
		[DllImport("Ultralight", EntryPoint = "ulPlatformSetFileSystem")]
		public static extern void SetFileSystem(_ULFileSystem file_system);

		[DllImport("Ultralight", EntryPoint = "ulPlatformSetGPUDriver")]
		public static extern void SetGPUDriver(ULGPUDriver gpu_driver);
		[DllImport("Ultralight", EntryPoint = "ulPlatformSetGPUDriver")]
		public static extern void SetGPUDriver(_ULGPUDriver gpu_driver);

		[DllImport("Ultralight", EntryPoint = "ulPlatformSetSurfaceDefinition")]
		public static extern void SetSurfaceDefinition(ULSurfaceDefinition surface_definition);

		[DllImport("Ultralight", EntryPoint = "ulPlatformSetClipboard")]
		public static extern void SetClipboard(ULClipboard clipboard);
		[DllImport("Ultralight", EntryPoint = "ulPlatformSetClipboard")]
		public static extern void SetClipboard(_ULClipboard clipboard);
	}
}
