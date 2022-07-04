using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore;

public static partial class AppCoreMethods
{
	[DllImport(LibAppCore)]
	public static extern IntPtr ulCreateOverlay(IntPtr window, uint width, uint height, int x, int y);

	[DllImport(LibAppCore)]
	public static extern IntPtr ulCreateOverlayWithView(IntPtr window, IntPtr view, int x, int y);

	[DllImport(LibAppCore)]
	public static extern void ulDestroyOverlay(IntPtr overlay);

	[DllImport(LibAppCore)]
	public static extern IntPtr ulOverlayGetView(IntPtr overlay);

	[DllImport(LibAppCore)]
	public static extern uint ulOverlayGetWidth(IntPtr overlay);

	[DllImport(LibAppCore)]
	public static extern uint ulOverlayGetHeight(IntPtr overlay);

	[DllImport(LibAppCore)]
	public static extern int ulOverlayGetX(IntPtr overlay);

	[DllImport(LibAppCore)]
	public static extern int ulOverlayGetY(IntPtr overlay);

	[DllImport(LibAppCore)]
	public static extern void ulOverlayMoveTo(IntPtr overlay, int x, int y);

	[DllImport(LibAppCore)]
	public static extern void ulOverlayResize(IntPtr overlay, uint width, uint height);

	[GeneratedDllImport(LibAppCore)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulOverlayIsHidden(IntPtr overlay);

	[DllImport(LibAppCore)]
	public static extern void ulOverlayHide(IntPtr overlay);

	[DllImport(LibAppCore)]
	public static extern void ulOverlayShow(IntPtr overlay);

	[GeneratedDllImport(LibAppCore)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulOverlayHasFocus(IntPtr overlay);

	[DllImport(LibAppCore)]
	public static extern void ulOverlayFocus(IntPtr overlay);

	[DllImport(LibAppCore)]
	public static extern void ulOverlayUnfocus(IntPtr overlay);
}

public class ULOverlay : IDisposable
{
	public IntPtr Ptr { get; private set; }
	public bool IsDisposed { get; private set; }

	public ULOverlay(IntPtr ptr, bool dispose = false)
	{
		Ptr = ptr;
		IsDisposed = !dispose;
	}

	public ULOverlay(ULWindow window, uint width, uint height, int x = 0, int y = 0)
	{
		Ptr = AppCoreMethods.ulCreateOverlay(window.Ptr, width, height, x, y);
	}
	public ULOverlay(ULWindow window, View view, int x = 0, int y = 0)
	{
		Ptr = AppCoreMethods.ulCreateOverlayWithView(window.Ptr, view.Ptr, x, y);
	}

	public View View => new(AppCoreMethods.ulOverlayGetView(Ptr));

	public uint Width => AppCoreMethods.ulOverlayGetWidth(Ptr);
	public uint Height => AppCoreMethods.ulOverlayGetHeight(Ptr);
	public int X => AppCoreMethods.ulOverlayGetX(Ptr);
	public int Y => AppCoreMethods.ulOverlayGetY(Ptr);
	public void MoveTo(int x, int y) => AppCoreMethods.ulOverlayMoveTo(Ptr, x, y);
	public void Resize(uint width, uint height) => AppCoreMethods.ulOverlayResize(Ptr, width, height);

	public bool IsHidden => AppCoreMethods.ulOverlayIsHidden(Ptr);
	public void Hide() => AppCoreMethods.ulOverlayHide(Ptr);
	public void Show() => AppCoreMethods.ulOverlayShow(Ptr);

	public bool HasFocus => AppCoreMethods.ulOverlayHasFocus(Ptr);
	public void Focus() => AppCoreMethods.ulOverlayFocus(Ptr);
	public void Unfocus() => AppCoreMethods.ulOverlayUnfocus(Ptr);

	public void Dispose()
	{
		if (IsDisposed) return;
		AppCoreMethods.ulDestroyOverlay(Ptr);

		IsDisposed = true;
		GC.SuppressFinalize(this);
	}
}
