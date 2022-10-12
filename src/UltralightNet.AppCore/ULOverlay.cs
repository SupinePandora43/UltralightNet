using System;
using System.Runtime.InteropServices;
using UltralightNet.LowStuff;

namespace UltralightNet.AppCore;

public static partial class AppCoreMethods
{
	[DllImport(LibAppCore)]
	public static extern Handle<ULOverlay> ulCreateOverlay(Handle<ULWindow> window, uint width, uint height, int x, int y);

	[DllImport(LibAppCore)]
	public static extern Handle<ULOverlay> ulCreateOverlayWithView(Handle<ULWindow> window, Handle<View> view, int x, int y);

	[DllImport(LibAppCore)]
	public static extern void ulDestroyOverlay(Handle<ULOverlay> overlay);

	[DllImport(LibAppCore)]
	public static extern Handle<View> ulOverlayGetView(Handle<ULOverlay> overlay);

	[DllImport(LibAppCore)]
	public static extern uint ulOverlayGetWidth(Handle<ULOverlay> overlay);

	[DllImport(LibAppCore)]
	public static extern uint ulOverlayGetHeight(Handle<ULOverlay> overlay);

	[DllImport(LibAppCore)]
	public static extern int ulOverlayGetX(Handle<ULOverlay> overlay);

	[DllImport(LibAppCore)]
	public static extern int ulOverlayGetY(Handle<ULOverlay> overlay);

	[DllImport(LibAppCore)]
	public static extern void ulOverlayMoveTo(Handle<ULOverlay> overlay, int x, int y);

	[DllImport(LibAppCore)]
	public static extern void ulOverlayResize(Handle<ULOverlay> overlay, uint width, uint height);

	[GeneratedDllImport(LibAppCore)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulOverlayIsHidden(Handle<ULOverlay> overlay);

	[DllImport(LibAppCore)]
	public static extern void ulOverlayHide(Handle<ULOverlay> overlay);

	[DllImport(LibAppCore)]
	public static extern void ulOverlayShow(Handle<ULOverlay> overlay);

	[GeneratedDllImport(LibAppCore)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulOverlayHasFocus(Handle<ULOverlay> overlay);

	[DllImport(LibAppCore)]
	public static extern void ulOverlayFocus(Handle<ULOverlay> overlay);

	[DllImport(LibAppCore)]
	public static extern void ulOverlayUnfocus(Handle<ULOverlay> overlay);
}

public class ULOverlay : INativeContainer<ULOverlay>, INativeContainerInterface<ULOverlay>, IEquatable<ULOverlay>
{
	private ULOverlay() { }

	public View View
	{
		get
		{
			View view = new((IntPtr)AppCoreMethods.ulOverlayGetView(Handle));
			GC.KeepAlive(this);
			return view;
		}
	}
	public uint Width
	{
		get
		{
			var returnValue = AppCoreMethods.ulOverlayGetWidth(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}
	public uint Height
	{
		get
		{
			var returnValue = AppCoreMethods.ulOverlayGetHeight(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}

	public (int X, int Y) Position
	{
		get
		{
			(int X, int Y) pos = new(AppCoreMethods.ulOverlayGetX(Handle), AppCoreMethods.ulOverlayGetY(Handle));
			GC.KeepAlive(this);
			return pos;
		}
		set
		{
			AppCoreMethods.ulOverlayMoveTo(Handle, value.X, value.Y);
			GC.KeepAlive(this);
		}
	}
	public void Resize(uint width, uint height) { AppCoreMethods.ulOverlayResize(Handle, width, height); GC.KeepAlive(this); }

	public bool IsHidden
	{
		get
		{
			var returnValue = AppCoreMethods.ulOverlayIsHidden(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}
	public void Hide() { AppCoreMethods.ulOverlayHide(Handle); GC.KeepAlive(this); }
	public void Show() { AppCoreMethods.ulOverlayShow(Handle); GC.KeepAlive(this); }

	public bool HasFocus
	{
		get
		{
			var returnValue = AppCoreMethods.ulOverlayHasFocus(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}
	public void Focus() { AppCoreMethods.ulOverlayFocus(Handle); GC.KeepAlive(this); }
	public void Unfocus() { AppCoreMethods.ulOverlayUnfocus(Handle); GC.KeepAlive(this); }

	public bool Equals(ULOverlay? other)
	{
		if (other is null) return false;
		return _ptr == other._ptr;
	}

	public override void Dispose()
	{
		if (!IsDisposed && Owns) AppCoreMethods.ulDestroyOverlay(Handle);
		base.Dispose();
	}

	public static ULOverlay FromHandle(Handle<ULOverlay> ptr, bool dispose) => new() { Handle = ptr, Owns = dispose };
}
