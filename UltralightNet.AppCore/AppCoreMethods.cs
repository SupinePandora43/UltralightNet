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

		[GeneratedDllImport("AppCore")]
		public static partial void ulEnablePlatformFileSystem([MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))] string base_dir);

		[GeneratedDllImport("AppCore")]
		public static partial void ulEnableDefaultLogger([MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))] string log_path);
	}
}
