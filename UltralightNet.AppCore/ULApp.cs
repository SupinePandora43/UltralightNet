using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore
{
	public static partial class AppCoreMethods
	{
		[DllImport("AppCore")]
		public static extern IntPtr ulCreateApp(IntPtr settings, IntPtr config);

		[DllImport("AppCore")]
		public static extern void ulDestroyApp(IntPtr app);

		[DllImport("AppCore")]
		public static extern void ulAppSetWindow(IntPtr app, IntPtr window);

		[DllImport("AppCore")]
		public static extern IntPtr ulAppGetWindow(IntPtr app);

		[DllImport("AppCore")]
		public static extern void ulAppSetUpdateCallback(IntPtr app, ULUpdateCallback callback, IntPtr user_data);

		[GeneratedDllImport("AppCore")]
		public static partial bool ulAppIsRunning(IntPtr app);

		[DllImport("AppCore")]
		public static extern IntPtr ulAppGetMainMonitor(IntPtr app);

		[DllImport("AppCore")]
		public static extern IntPtr ulAppGetRenderer(IntPtr app);

		[DllImport("AppCore")]
		public static extern void ulAppRun(IntPtr app);

		[DllImport("AppCore")]
		public static extern void ulAppQuit(IntPtr app);
	}
	public class ULApp
	{
	}
}
