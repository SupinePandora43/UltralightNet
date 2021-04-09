using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore
{
	public static partial class AppCoreMethods
	{
		[GeneratedDllImport("AppCore")]
		public static partial IntPtr ulCreateWindow(IntPtr monitor, uint width, uint height, [MarshalAs(UnmanagedType.I1)] bool fullscreen, ULWindowFlags flags);

		[DllImport("AppCore")]
		public static extern void ulDestroyWindow(IntPtr window);

		[DllImport("AppCore")]
		public static extern void ulWindowSetCloseCallback(IntPtr window, ULCloseCallback callback, IntPtr user_data);

		[DllImport("AppCore")]
		public static extern void ulWindowSetResizeCallback(IntPtr window, ULResizeCallback callback, IntPtr user_data);

		[DllImport("AppCore")]
		public static extern uint ulWindowGetWidth(IntPtr window);

		[DllImport("AppCore")]
		public static extern uint ulWindowGetHeight(IntPtr window);

		[GeneratedDllImport("AppCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulWindowIsFullscreen(IntPtr window);

		[DllImport("AppCore")]
		public static extern double ulWindowGetScale(IntPtr window);

		[GeneratedDllImport("AppCore", CharSet = CharSet.Ansi)]
		public static partial void ulWindowSetTitle(IntPtr window, string title);

		[DllImport("AppCore")]
		public static extern void ulWindowSetCursor(IntPtr window, ULCursor cursor);

		[DllImport("AppCore")]
		public static extern void ulWindowClose(IntPtr window);

		[DllImport("AppCore")]
		public static extern int ulWindowDeviceToPixel(IntPtr window, int val);

		[DllImport("AppCore")]
		public static extern int ulWindowPixelsToDevice(IntPtr window, int val);

		[DllImport("AppCore")]
		public static extern IntPtr ulWindowGetNativeHandle(IntPtr window);
	}

	public class ULWindow : IDisposable
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

		public void SetCloseCallback(ULCloseCallback callback, IntPtr userData = default) => AppCoreMethods.ulWindowSetCloseCallback(Ptr, callback, userData);
		public void SetResizeCallback(ULResizeCallback callback, IntPtr userData = default) => AppCoreMethods.ulWindowSetResizeCallback(Ptr, callback, userData);

		public uint Width => AppCoreMethods.ulWindowGetWidth(Ptr);
		public uint Height => AppCoreMethods.ulWindowGetHeight(Ptr);

		public bool IsFullscreen => AppCoreMethods.ulWindowIsFullscreen(Ptr);

		public double Scale => AppCoreMethods.ulWindowGetScale(Ptr);

		public string Title { set => AppCoreMethods.ulWindowSetTitle(Ptr, value); }

		public ULCursor Cursor { set => AppCoreMethods.ulWindowSetCursor(Ptr, value); }

		public void Close() => AppCoreMethods.ulWindowClose(Ptr);

		public int DeviceToPixel(int val) => AppCoreMethods.ulWindowDeviceToPixel(Ptr, val);
		public int PixelsToDevice(int val) => AppCoreMethods.ulWindowPixelsToDevice(Ptr, val);

		/// <summary>
		/// HWND on windows <br/>
		/// NSWindow* on mac <br/>
		/// GLFWwindow* on linux <br/>
		/// </summary>
		public IntPtr NativeHandle => AppCoreMethods.ulWindowGetNativeHandle(Ptr);

		public void Dispose()
		{
			if (IsDisposed) return;
			AppCoreMethods.ulDestroyWindow(Ptr);

			IsDisposed = true;
			GC.SuppressFinalize(this);
		}
	}
}
