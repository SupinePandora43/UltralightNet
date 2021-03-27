using System.Runtime.InteropServices;

namespace UltralightNet.AppCore
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "<Pending>")]
	public static partial class AppCoreMethods
	{
		static AppCoreMethods() => Methods.Preload();

		[DllImport("AppCore")]
		public static extern void ulEnablePlatformFontLoader();

		[DllImport("AppCore")]
		public static extern void ulEnablePlatformFileSystem([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string base_dir);

		[DllImport("AppCore")]
		public static extern void ulEnableDefaultLogger([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string log_path);
	}
}
