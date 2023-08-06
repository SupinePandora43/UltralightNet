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
public unsafe sealed class ULOverlay : NativeContainer
{
	private ULOverlay(void* ptr, Renderer renderer, View? view)
	{
		Handle = ptr;
		if (view is null)
		{
			view = View.FromHandle(AppCoreMethods.ulOverlayGetView(this), false);
			renderer.views[view.GetUserData()] = new(view);
			view.Renderer = renderer;
			view.SetUpCallbacks();
		}
		View = view;
	}

	public View View { get; }

	public uint Width => AppCoreMethods.ulOverlayGetWidth(this);
	public uint Height => AppCoreMethods.ulOverlayGetHeight(this);

	public (int X, int Y) Position
	{
		get => new(AppCoreMethods.ulOverlayGetX(this), AppCoreMethods.ulOverlayGetY(this));
		set => AppCoreMethods.ulOverlayMoveTo(this, value.X, value.Y);
	}
	public void Resize(uint width, uint height) => AppCoreMethods.ulOverlayResize(this, width, height);

	public bool IsHidden => AppCoreMethods.ulOverlayIsHidden(this);
	public void Hide() => AppCoreMethods.ulOverlayHide(this);
	public void Show() => AppCoreMethods.ulOverlayShow(this);

	public bool HasFocus => AppCoreMethods.ulOverlayHasFocus(this);
	public void Focus() => AppCoreMethods.ulOverlayFocus(this);
	public void Unfocus() => AppCoreMethods.ulOverlayUnfocus(this);

	public override void Dispose()
	{
		if (!IsDisposed && Owns) AppCoreMethods.ulDestroyOverlay(this);
		GC.KeepAlive(View);
		base.Dispose();
	}

	internal static unsafe ULOverlay FromHandle(void* ptr, Renderer renderer, View? view = null) => new(ptr, renderer, view);

	[CustomMarshaller(typeof(ULOverlay), MarshalMode.ManagedToUnmanagedIn, typeof(Marshaller))]
	internal ref struct Marshaller
	{
		private ULOverlay overlay;

		public void FromManaged(ULOverlay overlay) => this.overlay = overlay;
		public readonly unsafe void* ToUnmanaged() => overlay.Handle;
		public readonly void Free() => GC.KeepAlive(overlay);
	}
}
