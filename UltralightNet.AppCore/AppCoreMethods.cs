using System.Runtime.InteropServices;

namespace UltralightNet.AppCore
{
	public static class AppCoreMethods
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
