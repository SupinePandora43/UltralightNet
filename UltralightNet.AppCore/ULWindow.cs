using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore
{
	public static partial class AppCoreMethods
	{
		[GeneratedDllImport("AppCore")]
		public static partial IntPtr ulCreateWindow(IntPtr monitor, uint width, uint height, bool fullscreen, ULWindowFlags flags);

		[DllImport("AppCore")]
		public static extern void ulDestroyWindow(IntPtr window);

		[DllImport("AppCore")]
		public static extern void ulWindowSetCloseCallback(IntPtr window, ULCloseCallback callback, IntPtr user_data);

		[DllImport("AppCore")]
		public static extern void ulWindowSetResizeCallback(IntPtr window, ULResizeCallback callback, IntPtr user_data);
	}

	public class ULWindow: IDisposable
	{
		public IntPtr Ptr { get; private set; }
		public bool IsDisposed { get; private set; }

		public ULWindow(IntPtr ptr, bool dispose = false)
		{
			Ptr = ptr;
			IsDisposed = !dispose;
		}

		public ULWindow(ULMonitor monitor, uint width, uint height, bool fullscreen, ULWindowFlags flags)
		{
			Ptr = AppCoreMethods.ulCreateWindow(monitor.Ptr, width, height, fullscreen, flags);
		}

		public void Dispose()
		{
			if (IsDisposed) return;
			AppCoreMethods.ulDestroyWindow(Ptr);

			IsDisposed = true;
			GC.SuppressFinalize(this);
		}
	}
}
