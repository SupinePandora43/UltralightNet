using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using UltralightNet.LowStuff;

namespace UltralightNet.AppCore;

public static partial class AppCoreMethods
{
	[LibraryImport(LibAppCore)]
	internal static unsafe partial void* ulCreateOverlay(ULWindow window, uint width, uint height, int x, int y);

	[LibraryImport(LibAppCore)]
	internal static unsafe partial void* ulCreateOverlayWithView(ULWindow window, View view, int x, int y);

	[LibraryImport(LibAppCore)]
	internal static partial void ulDestroyOverlay(ULOverlay overlay);

	[LibraryImport(LibAppCore)]
	// INTEROPTODO
	internal static unsafe partial void* ulOverlayGetView(ULOverlay overlay);

	[LibraryImport(LibAppCore)]
	internal static partial uint ulOverlayGetWidth(ULOverlay overlay);

	[LibraryImport(LibAppCore)]
	internal static partial uint ulOverlayGetHeight(ULOverlay overlay);

	[LibraryImport(LibAppCore)]
	internal static partial int ulOverlayGetX(ULOverlay overlay);

	[LibraryImport(LibAppCore)]
	internal static partial int ulOverlayGetY(ULOverlay overlay);

	[LibraryImport(LibAppCore)]
	internal static partial void ulOverlayMoveTo(ULOverlay overlay, int x, int y);

	[LibraryImport(LibAppCore)]
	internal static partial void ulOverlayResize(ULOverlay overlay, uint width, uint height);

	[LibraryImport(LibAppCore)]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static partial bool ulOverlayIsHidden(ULOverlay overlay);

	[LibraryImport(LibAppCore)]
	internal static partial void ulOverlayHide(ULOverlay overlay);

	[LibraryImport(LibAppCore)]
	internal static partial void ulOverlayShow(ULOverlay overlay);

	[LibraryImport(LibAppCore)]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static partial bool ulOverlayHasFocus(ULOverlay overlay);

	[LibraryImport(LibAppCore)]
	internal static partial void ulOverlayFocus(ULOverlay overlay);

	[LibraryImport(LibAppCore)]
	internal static partial void ulOverlayUnfocus(ULOverlay overlay);
}

[NativeMarshalling(typeof(Marshaller))]
public class ULOverlay : NativeContainer
{
	private ULOverlay() { }

	public unsafe View View
	{
		get
		{
			// INTEROPTODO: GC
			return View.FromHandle(AppCoreMethods.ulOverlayGetView(this), false);
		}
	}
	public uint Width => AppCoreMethods.ulOverlayGetWidth(this);
	public uint Height => AppCoreMethods.ulOverlayGetHeight(this);

	public (int X, int Y) Position
	{
		get => new(AppCoreMethods.ulOverlayGetX(this), AppCoreMethods.ulOverlayGetY(this));
		set => AppCoreMethods.ulOverlayMoveTo(this, value.X, value.Y);
	}
	public void Resize(uint width, uint height) { AppCoreMethods.ulOverlayResize(this, width, height); GC.KeepAlive(this); }

	public bool IsHidden
	{
		get
		{
			var returnValue = AppCoreMethods.ulOverlayIsHidden(this);
			GC.KeepAlive(this);
			return returnValue;
		}
	}
	public void Hide() { AppCoreMethods.ulOverlayHide(this); GC.KeepAlive(this); }
	public void Show() { AppCoreMethods.ulOverlayShow(this); GC.KeepAlive(this); }

	public bool HasFocus
	{
		get
		{
			var returnValue = AppCoreMethods.ulOverlayHasFocus(this);
			GC.KeepAlive(this);
			return returnValue;
		}
	}
	public void Focus() { AppCoreMethods.ulOverlayFocus(this); GC.KeepAlive(this); }
	public void Unfocus() { AppCoreMethods.ulOverlayUnfocus(this); GC.KeepAlive(this); }

	public override void Dispose()
	{
		if (!IsDisposed && Owns) AppCoreMethods.ulDestroyOverlay(this);
		base.Dispose();
	}

	public static unsafe ULOverlay FromHandle(void* ptr, bool dispose) => new() { Handle = ptr, Owns = dispose };

	[CustomMarshaller(typeof(ULOverlay), MarshalMode.ManagedToUnmanagedIn, typeof(Marshaller))]
	internal ref struct Marshaller
	{
		private ULOverlay overlay;

		public void FromManaged(ULOverlay overlay) => this.overlay = overlay;
		public readonly unsafe void* ToUnmanaged() => overlay.Handle;
		public readonly void Free() => GC.KeepAlive(overlay);
	}
}
