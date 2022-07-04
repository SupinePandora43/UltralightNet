using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore;

public static partial class AppCoreMethods
{
	[GeneratedDllImport(LibAppCore)]
	public static partial IntPtr ulCreateWindow(IntPtr monitor, uint width, uint height, [MarshalAs(UnmanagedType.I1)] bool fullscreen, ULWindowFlags flags);

	[DllImport(LibAppCore)]
	public static extern void ulDestroyWindow(IntPtr window);

	[GeneratedDllImport(LibAppCore)]
	public static partial void ulWindowSetCloseCallback(IntPtr window, ULCloseCallback__PInvoke__? callback, IntPtr user_data);

	[GeneratedDllImport(LibAppCore)]
	public static partial void ulWindowSetResizeCallback(IntPtr window, ULResizeCallback__PInvoke__? callback, IntPtr user_data);

	[DllImport(LibAppCore)]
	public static extern uint ulWindowGetScreenWidth(IntPtr window);

	[DllImport(LibAppCore)]
	public static extern uint ulWindowGetWidth(IntPtr window);

	[DllImport(LibAppCore)]
	public static extern uint ulWindowGetScreenHeight(IntPtr window);

	[DllImport(LibAppCore)]
	public static extern uint ulWindowGetHeight(IntPtr window);

	[DllImport(LibAppCore)]
	public static extern void ulWindowMoveTo(IntPtr window, int x, int y);

	[DllImport(LibAppCore)]
	public static extern void ulWindowMoveToCenter(IntPtr window);

	[DllImport(LibAppCore)]
	public static extern int ulWindowGetPositionX(IntPtr window);

	[DllImport(LibAppCore)]
	public static extern int ulWindowGetPositionY(IntPtr window);

	[GeneratedDllImport(LibAppCore)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulWindowIsFullscreen(IntPtr window);

	[DllImport(LibAppCore)]
	public static extern double ulWindowGetScale(IntPtr window);

	[GeneratedDllImport(LibAppCore)]
	public static partial void ulWindowSetTitle(IntPtr window, [MarshalUsing(typeof(UTF8Marshaller))] string title);

	[DllImport(LibAppCore)]
	public static extern void ulWindowSetCursor(IntPtr window, ULCursor cursor);

	[DllImport(LibAppCore)]
	public static extern void ulWindowShow(IntPtr window);

	[DllImport(LibAppCore)]
	public static extern void ulWindowHide(IntPtr window);

	[GeneratedDllImport(LibAppCore)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulWindowIsVisible(IntPtr window);

	[DllImport(LibAppCore)]
	public static extern void ulWindowClose(IntPtr window);

	[DllImport(LibAppCore)]
	public static extern int ulWindowScreenToPixels(IntPtr window, int val);

	[DllImport(LibAppCore)]
	public static extern int ulWindowPixelsToScreen(IntPtr window, int val);

	[DllImport(LibAppCore)]
	public static extern IntPtr ulWindowGetNativeHandle(IntPtr window);
}

public class ULWindow : IDisposable
{
	public readonly IntPtr Ptr;
	public bool IsDisposed { get; private set; }

	private readonly GCHandle[] handles = new GCHandle[2];

	public ULWindow(IntPtr ptr, bool dispose = false)
	{
		Ptr = ptr;
		IsDisposed = !dispose;
	}

	public ULWindow(ULMonitor monitor, uint width, uint height, bool fullscreen = false, ULWindowFlags flags = ULWindowFlags.Titled | ULWindowFlags.Resizable | ULWindowFlags.Maximizable)
	{
		Ptr = AppCoreMethods.ulCreateWindow(monitor.Ptr, width, height, fullscreen, flags);
	}

	public void SetCloseCallback(ULCloseCallback callback, IntPtr userData = default)
	{
		if (callback is not null)
		{
			ULCloseCallback__PInvoke__ callback__PInvoke__ = (user_data, window) => callback(user_data, new ULWindow(window));
			if (handles[0].IsAllocated) handles[0].Free();
			handles[0] = GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal);
			AppCoreMethods.ulWindowSetCloseCallback(Ptr, callback__PInvoke__, userData);
		}
		else
		{
			if (handles[0].IsAllocated) handles[0].Free();
			handles[0] = default;
			AppCoreMethods.ulWindowSetCloseCallback(Ptr, null, userData);
		}
	}
	public void SetResizeCallback(ULResizeCallback callback, IntPtr userData = default)
	{
		if (callback is not null)
		{
			ULResizeCallback__PInvoke__ callback__PInvoke__ = (user_data, window, width, height) => callback(user_data, new ULWindow(window), width, height);
			if (handles[1].IsAllocated) handles[1].Free();
			handles[1] = GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal);
			AppCoreMethods.ulWindowSetResizeCallback(Ptr, callback__PInvoke__, userData);
		}
		else
		{
			if (handles[1].IsAllocated) handles[1].Free();
			handles[1] = default;
			AppCoreMethods.ulWindowSetResizeCallback(Ptr, null, userData);
		}
	}
	public uint ScreenWidth => AppCoreMethods.ulWindowGetScreenWidth(Ptr);
	public uint Width => AppCoreMethods.ulWindowGetWidth(Ptr);
	public uint ScreenHeight => AppCoreMethods.ulWindowGetScreenHeight(Ptr);
	public uint Height => AppCoreMethods.ulWindowGetHeight(Ptr);

	public void MoveTo(int x, int y) => AppCoreMethods.ulWindowMoveTo(Ptr, x, y);
	public void MoveToCenter() => AppCoreMethods.ulWindowMoveToCenter(Ptr);

	public int X => AppCoreMethods.ulWindowGetPositionX(Ptr);
	public int Y => AppCoreMethods.ulWindowGetPositionY(Ptr);

	public bool IsFullscreen => AppCoreMethods.ulWindowIsFullscreen(Ptr);

	public double Scale => AppCoreMethods.ulWindowGetScale(Ptr);

	public string Title { set => AppCoreMethods.ulWindowSetTitle(Ptr, value); }

	public ULCursor Cursor { set => AppCoreMethods.ulWindowSetCursor(Ptr, value); }

	public void Show() => AppCoreMethods.ulWindowShow(Ptr);
	public void Hide() => AppCoreMethods.ulWindowHide(Ptr);

	public void Close() => AppCoreMethods.ulWindowClose(Ptr);

	public int ScreenToPixel(int val) => AppCoreMethods.ulWindowScreenToPixels(Ptr, val);
	public int PixelsToScreen(int val) => AppCoreMethods.ulWindowPixelsToScreen(Ptr, val);

	/// <summary>
	/// HWND on windows <br/>
	/// NSWindow* on mac <br/>
	/// GLFWwindow* on linux <br/>
	/// </summary>
	public IntPtr NativeWindowHandle => AppCoreMethods.ulWindowGetNativeHandle(Ptr);

	public void Dispose()
	{
		foreach (GCHandle handle in handles)
		{
			if (handle.IsAllocated)
			{
				handle.Free();
			}
		}

		if (IsDisposed) return;
		AppCoreMethods.ulDestroyWindow(Ptr);

		IsDisposed = true;
		GC.SuppressFinalize(this);
	}
}
