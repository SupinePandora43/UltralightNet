using System;
using System.Runtime.InteropServices;
using UltralightNet.LowStuff;

namespace UltralightNet.AppCore;

public static partial class AppCoreMethods
{
	[GeneratedDllImport(LibAppCore)]
	public static partial Handle<ULWindow> ulCreateWindow(ULMonitor monitor, uint width, uint height, [MarshalAs(UnmanagedType.I1)] bool fullscreen, ULWindowFlags flags);

	[DllImport(LibAppCore)]
	public static extern void ulDestroyWindow(Handle<ULWindow> window);

	[GeneratedDllImport(LibAppCore)]
	public static partial void ulWindowSetCloseCallback(Handle<ULWindow> window, ULCloseCallback__PInvoke__? callback, IntPtr user_data);

	[GeneratedDllImport(LibAppCore)]
	public static partial void ulWindowSetResizeCallback(Handle<ULWindow> window, ULResizeCallback__PInvoke__? callback, IntPtr user_data);

	[DllImport(LibAppCore)]
	public static extern uint ulWindowGetScreenWidth(Handle<ULWindow> window);

	[DllImport(LibAppCore)]
	public static extern uint ulWindowGetWidth(Handle<ULWindow> window);

	[DllImport(LibAppCore)]
	public static extern uint ulWindowGetScreenHeight(Handle<ULWindow> window);

	[DllImport(LibAppCore)]
	public static extern uint ulWindowGetHeight(Handle<ULWindow> window);

	[DllImport(LibAppCore)]
	public static extern void ulWindowMoveTo(Handle<ULWindow> window, int x, int y);

	[DllImport(LibAppCore)]
	public static extern void ulWindowMoveToCenter(Handle<ULWindow> window);

	[DllImport(LibAppCore)]
	public static extern int ulWindowGetPositionX(Handle<ULWindow> window);

	[DllImport(LibAppCore)]
	public static extern int ulWindowGetPositionY(Handle<ULWindow> window);

	[GeneratedDllImport(LibAppCore)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulWindowIsFullscreen(Handle<ULWindow> window);

	[DllImport(LibAppCore)]
	public static extern double ulWindowGetScale(Handle<ULWindow> window);

	[GeneratedDllImport(LibAppCore)]
	public static partial void ulWindowSetTitle(Handle<ULWindow> window, [MarshalUsing(typeof(UTF8Marshaller))] string title);

	[DllImport(LibAppCore)]
	public static extern void ulWindowSetCursor(Handle<ULWindow> window, ULCursor cursor);

	[DllImport(LibAppCore)]
	public static extern void ulWindowShow(Handle<ULWindow> window);

	[DllImport(LibAppCore)]
	public static extern void ulWindowHide(Handle<ULWindow> window);

	[GeneratedDllImport(LibAppCore)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulWindowIsVisible(Handle<ULWindow> window);

	[DllImport(LibAppCore)]
	public static extern void ulWindowClose(Handle<ULWindow> window);

	[DllImport(LibAppCore)]
	public static extern int ulWindowScreenToPixels(Handle<ULWindow> window, int val);

	[DllImport(LibAppCore)]
	public static extern int ulWindowPixelsToScreen(Handle<ULWindow> window, int val);

	[DllImport(LibAppCore)]
	public static extern IntPtr ulWindowGetNativeHandle(Handle<ULWindow> window);
}

public class ULWindow : INativeContainer<ULWindow>, INativeContainerInterface<ULWindow>, IEquatable<ULWindow>
{
	private readonly GCHandle[] handles = new GCHandle[2];

	private ULWindow() { }

	public void SetCloseCallback(ULCloseCallback callback, IntPtr userData = default)
	{
		if (callback is not null)
		{
			ULCloseCallback__PInvoke__ callback__PInvoke__ = (user_data, window) => callback(user_data, ULWindow.FromHandle(window, false));
			if (handles[0].IsAllocated) handles[0].Free();
			handles[0] = GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal);
			AppCoreMethods.ulWindowSetCloseCallback(Handle, callback__PInvoke__, userData);
		}
		else
		{
			if (handles[0].IsAllocated) handles[0].Free();
			handles[0] = default;
			AppCoreMethods.ulWindowSetCloseCallback(Handle, null, userData);
		}
		GC.KeepAlive(this);
	}
	public void SetResizeCallback(ULResizeCallback callback, IntPtr userData = default)
	{
		if (callback is not null)
		{
			ULResizeCallback__PInvoke__ callback__PInvoke__ = (user_data, window, width, height) => callback(user_data, ULWindow.FromHandle(window, false), width, height);
			if (handles[1].IsAllocated) handles[1].Free();
			handles[1] = GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal);
			AppCoreMethods.ulWindowSetResizeCallback(Handle, callback__PInvoke__, userData);
		}
		else
		{
			if (handles[1].IsAllocated) handles[1].Free();
			handles[1] = default;
			AppCoreMethods.ulWindowSetResizeCallback(Handle, null, userData);
		}
		GC.KeepAlive(this);
	}

	public uint ScreenWidth
	{
		get
		{
			var returnValue = AppCoreMethods.ulWindowGetScreenWidth(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}
	public uint Width
	{
		get
		{
			var returnValue = AppCoreMethods.ulWindowGetWidth(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}
	public uint ScreenHeight
	{
		get
		{
			var returnValue = AppCoreMethods.ulWindowGetScreenHeight(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}
	public uint Height
	{
		get
		{
			var returnValue = AppCoreMethods.ulWindowGetHeight(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}

	public void MoveToCenter() { AppCoreMethods.ulWindowMoveToCenter(Handle); GC.KeepAlive(this); }

	public (int X, int Y) Position // INTEROPTODO: TEST
	{
		get
		{
			(int X, int Y) pos = new(AppCoreMethods.ulWindowGetPositionX(Handle), AppCoreMethods.ulWindowGetPositionY(Handle));
			GC.KeepAlive(this);
			return pos;
		}
		set
		{
			AppCoreMethods.ulWindowMoveTo(Handle, value.X, value.Y);
			GC.KeepAlive(this);
		}
	}

	public bool IsFullscreen
	{
		get
		{
			var returnValue = AppCoreMethods.ulWindowIsFullscreen(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}
	public double Scale
	{
		get
		{
			var returnValue = AppCoreMethods.ulWindowGetScale(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}
	public string Title
	{
		set
		{
			AppCoreMethods.ulWindowSetTitle(Handle, value);
			GC.KeepAlive(this);
		}
	}

	public ULCursor Cursor
	{
		set
		{
			AppCoreMethods.ulWindowSetCursor(Handle, value);
			GC.KeepAlive(this);
		}
	}

	public void Show() { AppCoreMethods.ulWindowShow(Handle); GC.KeepAlive(this); }
	public void Hide() { AppCoreMethods.ulWindowHide(Handle); GC.KeepAlive(this); }

	public void Close() { AppCoreMethods.ulWindowClose(Handle); GC.KeepAlive(this); }

	public int ScreenToPixel(int val)
	{
		var returnValue = AppCoreMethods.ulWindowScreenToPixels(Handle, val);
		GC.KeepAlive(this);
		return returnValue;
	}
	public int PixelsToScreen(int val)
	{
		var returnValue = AppCoreMethods.ulWindowPixelsToScreen(Handle, val);
		GC.KeepAlive(this);
		return returnValue;
	}

	/// <summary>
	/// HWND on windows <br/>
	/// NSWindow* on mac <br/>
	/// GLFWwindow* on linux <br/>
	/// </summary>
	public IntPtr NativeWindowHandle
	{
		get
		{
			var returnValue = AppCoreMethods.ulWindowGetNativeHandle(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}

	public ULOverlay CreateOverlay(uint width, uint height, int x = 0, int y = 0)
	{
		ULOverlay returnValue = ULOverlay.FromHandle(AppCoreMethods.ulCreateOverlay(Handle, width, height, x, y), true);
		GC.KeepAlive(this);
		return returnValue;
	}
	public ULOverlay CreateOverlay(View view, int x = 0, int y = 0)
	{
		ULOverlay returnValue = ULOverlay.FromHandle(AppCoreMethods.ulCreateOverlayWithView(Handle, (Handle<View>)view.Ptr, x, y), true);
		GC.KeepAlive(this);
		return returnValue;
	}

	public bool Equals(ULWindow? other)
	{
		if (other is null) return false;
		return _ptr == other._ptr;
	}

	public override void Dispose()
	{
		foreach (GCHandle handle in handles)
		{
			if (handle.IsAllocated)
			{
				handle.Free();
			}
		}

		if (!IsDisposed && Owns) AppCoreMethods.ulDestroyWindow(Handle);

		base.Dispose();
	}

	public static ULWindow FromHandle(Handle<ULWindow> ptr, bool dispose) => new() { Handle = ptr, Owns = dispose };
}
