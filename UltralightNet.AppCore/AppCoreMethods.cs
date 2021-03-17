using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore
{
    public static class AppCoreMethods
    {
		[DllImport("AppCore")]
		public static extern void ulEnablePlatformFontLoader();

		[DllImport("AppCore")]
		public static extern void ulEnablePlatformFileSystem(IntPtr base_dir);

		[DllImport("AppCore")]
		public static extern void ulEnableDefaultLogger(IntPtr log_path);
	}

	public static class AppCore
	{
		public static void EnablePlatformFontLoader() => AppCoreMethods.ulEnablePlatformFontLoader();
		public static void EnablePlatformFileSystem(ULString base_dir) => AppCoreMethods.ulEnablePlatformFileSystem(base_dir.Ptr);
		public static void EnableDefaultLogger(ULString log_path) => AppCoreMethods.ulEnableDefaultLogger(log_path.Ptr);
	}
}
