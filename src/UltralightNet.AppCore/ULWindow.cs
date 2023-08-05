using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using UltralightNet.LowStuff;

namespace UltralightNet.AppCore;

public static partial class AppCoreMethods
{
	[LibraryImport(LibAppCore)]
	internal static unsafe partial void* ulCreateWindow(ULMonitor monitor, uint width, uint height, [MarshalAs(UnmanagedType.U1)] bool fullscreen, ULWindowFlags flags);

	[LibraryImport(LibAppCore)]
	internal static partial void ulDestroyWindow(ULWindow window);

	[LibraryImport(LibAppCore)]
	internal static partial void ulWindowSetCloseCallback(ULWindow window, ULCloseCallback__PInvoke__? callback, IntPtr user_data);

	[LibraryImport(LibAppCore)]
	internal static partial void ulWindowSetResizeCallback(ULWindow window, ULResizeCallback__PInvoke__? callback, IntPtr user_data);

	[LibraryImport(LibAppCore)]
	internal static partial uint ulWindowGetScreenWidth(ULWindow window);

	[LibraryImport(LibAppCore)]
	internal static partial uint ulWindowGetWidth(ULWindow window);

	[LibraryImport(LibAppCore)]
	internal static partial uint ulWindowGetScreenHeight(ULWindow window);

	[LibraryImport(LibAppCore)]
	internal static partial uint ulWindowGetHeight(ULWindow window);

	[LibraryImport(LibAppCore)]
	internal static partial void ulWindowMoveTo(ULWindow window, int x, int y);

	[LibraryImport(LibAppCore)]
	internal static partial void ulWindowMoveToCenter(ULWindow window);

	[LibraryImport(LibAppCore)]
	internal static partial int ulWindowGetPositionX(ULWindow window);

	[LibraryImport(LibAppCore)]
	internal static partial int ulWindowGetPositionY(ULWindow window);

	[LibraryImport(LibAppCore)]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static partial bool ulWindowIsFullscreen(ULWindow window);

	[LibraryImport(LibAppCore)]
	internal static partial double ulWindowGetScale(ULWindow window);

	[LibraryImport(LibAppCore)]
	internal static partial void ulWindowSetTitle(ULWindow window, [MarshalUsing(typeof(Utf8StringMarshaller))] string title);

	[LibraryImport(LibAppCore)]
	internal static partial void ulWindowSetCursor(ULWindow window, ULCursor cursor);

	[LibraryImport(LibAppCore)]
	internal static partial void ulWindowShow(ULWindow window);

	[LibraryImport(LibAppCore)]
	internal static partial void ulWindowHide(ULWindow window);

	[LibraryImport(LibAppCore)]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static partial bool ulWindowIsVisible(ULWindow window);

	[LibraryImport(LibAppCore)]
	internal static partial void ulWindowClose(ULWindow window);

	[LibraryImport(LibAppCore)]
	internal static partial int ulWindowScreenToPixels(ULWindow window, int val);

	[LibraryImport(LibAppCore)]
	internal static partial int ulWindowPixelsToScreen(ULWindow window, int val);

	[LibraryImport(LibAppCore)]
	internal static unsafe partial void* ulWindowGetNativeHandle(ULWindow window);
}

[NativeMarshalling(typeof(Marshaller))]
public sealed class ULWindow : NativeContainer, IEquatable<ULWindow>
{
	private readonly GCHandle[] handles = new GCHandle[2];

	private ULWindow() { }

	public unsafe void SetCloseCallback(ULCloseCallback callback, IntPtr userData = default)
	{
		if (callback is not null)
		{
			ULCloseCallback__PInvoke__ callback__PInvoke__ = (user_data, window) => callback(user_data, FromHandle(window, false));
			if (handles[0].IsAllocated) handles[0].Free();
			handles[0] = GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal);
			AppCoreMethods.ulWindowSetCloseCallback(this, callback__PInvoke__, userData);
		}
		else
		{
			if (handles[0].IsAllocated) handles[0].Free();
			handles[0] = default;
			AppCoreMethods.ulWindowSetCloseCallback(this, null, userData);
		}
	}
	public unsafe void SetResizeCallback(ULResizeCallback callback, IntPtr userData = default)
	{
		if (callback is not null)
		{
			ULResizeCallback__PInvoke__ callback__PInvoke__ = (user_data, window, width, height) => callback(user_data, FromHandle(window, false), width, height);
			if (handles[1].IsAllocated) handles[1].Free();
			handles[1] = GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal);
			AppCoreMethods.ulWindowSetResizeCallback(this, callback__PInvoke__, userData);
		}
		else
		{
			if (handles[1].IsAllocated) handles[1].Free();
			handles[1] = default;
			AppCoreMethods.ulWindowSetResizeCallback(this, null, userData);
		}
	}

	public uint ScreenWidth => AppCoreMethods.ulWindowGetScreenWidth(this);
	public uint ScreenHeight => AppCoreMethods.ulWindowGetScreenHeight(this);

	public uint Width => AppCoreMethods.ulWindowGetWidth(this);
	public uint Height => AppCoreMethods.ulWindowGetHeight(this);

	public void MoveToCenter() => AppCoreMethods.ulWindowMoveToCenter(this);

	public (int X, int Y) Position // INTEROPTODO: TEST
	{
		get => new(AppCoreMethods.ulWindowGetPositionX(this), AppCoreMethods.ulWindowGetPositionY(this));
		set => AppCoreMethods.ulWindowMoveTo(this, value.X, value.Y);
	}

	public bool IsFullscreen => AppCoreMethods.ulWindowIsFullscreen(this);

	public double Scale => AppCoreMethods.ulWindowGetScale(this);
	public string Title { set => AppCoreMethods.ulWindowSetTitle(this, value); }

	public ULCursor Cursor { set => AppCoreMethods.ulWindowSetCursor(this, value); }

	public void Show() => AppCoreMethods.ulWindowShow(this);
	public void Hide() => AppCoreMethods.ulWindowHide(this);

	public void Close() => AppCoreMethods.ulWindowClose(this);

	public int ScreenToPixel(int val) => AppCoreMethods.ulWindowScreenToPixels(this, val);
	public int PixelsToScreen(int val) => AppCoreMethods.ulWindowPixelsToScreen(this, val);

	/// <summary>
	/// HWND on windows <br/>
	/// NSWindow* on mac <br/>
	/// GLFWwindow* on linux <br/>
	/// </summary>
	public unsafe void* NativeWindowHandle => AppCoreMethods.ulWindowGetNativeHandle(this);

	public unsafe ULOverlay CreateOverlay(uint width, uint height, int x = 0, int y = 0) => ULOverlay.FromHandle(AppCoreMethods.ulCreateOverlay(this, width, height, x, y), true);
	public unsafe ULOverlay CreateOverlay(View view, int x = 0, int y = 0) => ULOverlay.FromHandle(AppCoreMethods.ulCreateOverlayWithView(this, view, x, y), true);

	public bool Equals(ULWindow? other)
	{
		if (other is null) return false;
		return base.Equals(other);
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

		if (!IsDisposed && Owns) AppCoreMethods.ulDestroyWindow(this);

		base.Dispose();
	}

	public static unsafe ULWindow FromHandle(void* ptr, bool dispose) => new() { Handle = ptr, Owns = dispose };

	[CustomMarshaller(typeof(ULWindow), MarshalMode.ManagedToUnmanagedIn, typeof(Marshaller))]
	internal ref struct Marshaller
	{
		private ULWindow window;

		public void FromManaged(ULWindow window) => this.window = window;
		public readonly unsafe void* ToUnmanaged() => window.Handle;
		public readonly void Free() => GC.KeepAlive(window);
	}
}
