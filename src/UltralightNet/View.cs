using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using UltralightNet.Callbacks;
using UltralightNet.LowStuff;

namespace UltralightNet;

public static unsafe partial class Methods
{
	[LibraryImport(LibUltralight)]
	public static partial void* ulCreateView(Renderer renderer, uint width, uint height, in ULViewConfig viewConfig, Session session);

	[LibraryImport(LibUltralight)]
	public static partial void ulDestroyView(View view);

	[LibraryImport(LibUltralight)]
	[return: MarshalUsing(typeof(ULString))]
	public static partial string ulViewGetURL(View view);

	[LibraryImport(LibUltralight)]
	[return: MarshalUsing(typeof(ULString))]
	public static partial string ulViewGetTitle(View view);

	[LibraryImport(LibUltralight)]
	public static partial uint ulViewGetWidth(View view);

	[LibraryImport(LibUltralight)]
	public static partial uint ulViewGetHeight(View view);

	[LibraryImport(LibUltralight)]
	public static partial double ulViewGetDeviceScale(View view);

	[LibraryImport(LibUltralight)]
	public static partial void ulViewSetDeviceScale(View view, double scale);

	[LibraryImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.U1)]
	public static partial bool ulViewIsAccelerated(View view);

	[LibraryImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.U1)]
	public static partial bool ulViewIsTransparent(View view);

	[LibraryImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.U1)]
	public static partial bool ulViewIsLoading(View view);

	[LibraryImport(LibUltralight)]
	public static partial RenderTarget ulViewGetRenderTarget(View view);

	[LibraryImport(LibUltralight)]
	public static partial nuint ulViewGetSurface(View view);

	[LibraryImport(LibUltralight)]
	public static partial void ulViewLoadHTML(View view, [MarshalUsing(typeof(ULString))] string html_string);

	[LibraryImport(LibUltralight)]
	public static partial void ulViewLoadURL(View view, [MarshalUsing(typeof(ULString))] string url_string);

	[LibraryImport(LibUltralight)]
	public static partial void ulViewResize(View view, uint width, uint height);

	//todo: JavaScriptCore bindings
	[LibraryImport(LibUltralight)]
	public static partial void* ulViewLockJSContext(View view);

	[LibraryImport(LibUltralight)]
	public static partial void ulViewUnlockJSContext(View view);

	[LibraryImport(LibUltralight)]
	[return: MarshalUsing(typeof(ULString))]
	public static partial string ulViewEvaluateScript(View view, [MarshalUsing(typeof(ULString))] string js, [MarshalUsing(typeof(ULString))] out string exception);

	/// <summary>Check if can navigate backwards in history.</summary>
	[LibraryImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.U1)]
	public static partial bool ulViewCanGoBack(View view);

	/// <summary>Check if can navigate forwards in history.</summary>
	[LibraryImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.U1)]
	public static partial bool ulViewCanGoForward(View view);

	/// <summary>Navigate backwards in history.</summary>
	[LibraryImport(LibUltralight)]
	public static partial void ulViewGoBack(View view);

	/// <summary>Navigate forwards in history.</summary>
	[LibraryImport(LibUltralight)]
	public static partial void ulViewGoForward(View view);

	/// <summary>Navigate to arbitrary offset in history.</summary>
	[LibraryImport(LibUltralight)]
	public static partial void ulViewGoToHistoryOffset(View view, int offset);

	/// <summary>Reload current page.</summary>
	[LibraryImport(LibUltralight)]
	public static partial void ulViewReload(View view);

	/// <summary>Stop all page loads.</summary>
	[LibraryImport(LibUltralight)]
	public static partial void ulViewStop(View view);

	/// <summary>Give focus to the View.</summary>
	/// <remarks>
	/// You should call this to give visual indication that the View has input
	/// focus (changes active text selection colors, for example).
	/// </remarks>
	[LibraryImport(LibUltralight)]
	public static partial void ulViewFocus(View view);

	/// <summary>Remove focus from the View and unfocus any focused input elements.</summary>
	/// <remarks>
	/// You should call this to give visual indication that the View has lost
	/// input focus.
	/// </remarks>
	[LibraryImport(LibUltralight)]
	public static partial void ulViewUnfocus(View view);

	/// <summary>Whether or not the View has focus.</summary>
	[LibraryImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.U1)]
	public static partial bool ulViewHasFocus(View view);

	/// <summary>Whether or not the View has an input element with visible keyboard focus (indicated by a blinking caret).</summary>
	/// <remarks>
	/// You can use this to decide whether or not the View should consume
	/// keyboard input events (useful in games with mixed UI and key handling).
	/// </remarks>
	[LibraryImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.U1)]
	public static partial bool ulViewHasInputFocus(View view);

	[LibraryImport(LibUltralight)]
	public static partial void ulViewFireKeyEvent(View view, ULKeyEvent keyEvent);

	[LibraryImport(LibUltralight)]
	public static partial void ulViewFireMouseEvent(View view, ULMouseEvent* mouseEvent);

	[LibraryImport(LibUltralight)]
	public static partial void ulViewFireScrollEvent(View view, ULScrollEvent* scrollEvent);

	[LibraryImport(LibUltralight)]
	internal static partial void ulViewSetChangeTitleCallback(View view, delegate* unmanaged[Cdecl]<nuint, void*, ULString*, void> callback, nuint id);

	[LibraryImport(LibUltralight)]
	internal static partial void ulViewSetChangeURLCallback(View view, delegate* unmanaged[Cdecl]<nuint, void*, ULString*, void> callback, nuint id);

	[LibraryImport(LibUltralight)]
	internal static partial void ulViewSetChangeTooltipCallback(View view, delegate* unmanaged[Cdecl]<nuint, void*, ULString*, void> callback, nuint id);

	[LibraryImport(LibUltralight)]
	internal static partial void ulViewSetChangeCursorCallback(View view, delegate* unmanaged[Cdecl]<nuint, void*, ULCursor, void> callback, nuint id);

	[LibraryImport(LibUltralight)]
	internal static partial void ulViewSetAddConsoleMessageCallback(View view, delegate* unmanaged[Cdecl]<nuint, void*, ULMessageSource, ULMessageLevel, ULString*, uint, uint, ULString*, void> callback, nuint id);

	[LibraryImport(LibUltralight)]
	internal static partial void ulViewSetCreateChildViewCallback(View view, delegate* unmanaged[Cdecl]<nuint, void*, ULString*, ULString*, bool, ULIntRect, void*> callback, nuint id);

	[LibraryImport(LibUltralight)]
	internal static partial void ulViewSetCreateInspectorViewCallback(View view, delegate* unmanaged[Cdecl]<nuint, void*, bool, ULString*, void*> callback, nuint id);

	[LibraryImport(LibUltralight)]
	internal static partial void ulViewSetBeginLoadingCallback(View view, delegate* unmanaged[Cdecl]<nuint, void*, ulong, bool, ULString*, void> callback, nuint id);

	[LibraryImport(LibUltralight)]
	internal static partial void ulViewSetFinishLoadingCallback(View view, delegate* unmanaged[Cdecl]<nuint, void*, ulong, bool, ULString*, void> callback, nuint id);

	[LibraryImport(LibUltralight)]
	internal static partial void ulViewSetFailLoadingCallback(View view, delegate* unmanaged[Cdecl]<nuint, void*, ulong, bool, ULString*, ULString*, ULString*, int, void> callback, nuint id);

	[LibraryImport(LibUltralight)]
	internal static partial void ulViewSetWindowObjectReadyCallback(View view, delegate* unmanaged[Cdecl]<nuint, void*, ulong, bool, ULString*, void> callback, nuint id);

	[LibraryImport(LibUltralight)]
	internal static partial void ulViewSetDOMReadyCallback(View view, delegate* unmanaged[Cdecl]<nuint, void*, ulong, bool, ULString*, void> callback, nuint id);

	[LibraryImport(LibUltralight)]
	internal static partial void ulViewSetUpdateHistoryCallback(View view, delegate* unmanaged[Cdecl]<nuint, void*, void> callback, nuint id);

	[LibraryImport(LibUltralight)]
	public static partial void ulViewSetNeedsPaint(View view, [MarshalAs(UnmanagedType.U1)] bool needs_paint);

	[LibraryImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.U1)]
	public static partial bool ulViewGetNeedsPaint(View view);

	[LibraryImport(LibUltralight)]
	public static partial void* ulViewCreateInspectorView(View view);
}

[NativeMarshalling(typeof(Marshaller))]
public sealed unsafe class View : NativeContainer
{
	protected override void* Handle
	{
		get
		{
			Renderer?.AssertNotWrongThread();
			return base.Handle;
		}
	}
	internal Renderer? Renderer { get; set; }

	//private JSContext Context { get; set; }
	//private JSContext? lockedContext;

	public string URL
	{
		get => Methods.ulViewGetURL(this);
		set => Methods.ulViewLoadURL(this, value);
	}
	public string HTML { set => Methods.ulViewLoadHTML(this, value); }
	public string Title { get => Methods.ulViewGetTitle(this); }

	public uint Width => Methods.ulViewGetWidth(this);
	public uint Height => Methods.ulViewGetHeight(this);
	public double DeviceScale { get => Methods.ulViewGetDeviceScale(this); set => Methods.ulViewSetDeviceScale(this, value); }

	public bool IsAccelerated => Methods.ulViewIsAccelerated(this);
	public bool IsTransparent => Methods.ulViewIsTransparent(this);

	public bool IsLoading => Methods.ulViewIsLoading(this);

	/// <summary>
	/// Provides info used to display texture in your application
	/// </summary>
	/// <remarks>Only valid when <see cref="ULGPUDriver"/> is used</remarks>
	public RenderTarget RenderTarget
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Methods.ulViewGetRenderTarget(this);
	}

	public ULSurface? Surface
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			nuint surfaceHandle = Methods.ulViewGetSurface(this);
			if (surfaceHandle is 0) return null;
			return ULSurface.FromHandle(surfaceHandle);
		}
	}

	public void Resize(in uint width, in uint height) => Methods.ulViewResize(this, width, height);

	/*public ref readonly JSContext LockJSContext()
	{
		void* contextHandle = Methods.ulViewLockJSContext(this);
		Context.OnLocked(contextHandle);
		lockedContext = Context;
		return ref lockedContext!;
	}
	public void UnlockJSContext()
	{
		lockedContext = null;
		Methods.ulViewUnlockJSContext(this);
	}*/

	public string EvaluateScript(string js_string, out string exception) => Methods.ulViewEvaluateScript(this, js_string, out exception);

	public bool CanGoBack => Methods.ulViewCanGoBack(this);
	public bool CanGoForward => Methods.ulViewCanGoForward(this);

	public void GoBack() => Methods.ulViewGoBack(this);
	public void GoForward() => Methods.ulViewGoForward(this);
	public void GoToHistoryOffset(in int offset) => Methods.ulViewGoToHistoryOffset(this, offset);

	public void Reload() => Methods.ulViewReload(this);
	public void Stop() => Methods.ulViewStop(this);

	public void Focus() => Methods.ulViewFocus(this);
	public void Unfocus() => Methods.ulViewUnfocus(this);
	public bool HasFocus => Methods.ulViewHasFocus(this);
	public bool HasInputFocus => Methods.ulViewHasInputFocus(this);

	public void FireKeyEvent(ULKeyEvent keyEvent) => Methods.ulViewFireKeyEvent(this, keyEvent);
	public void FireMouseEvent(ULMouseEvent mouseEvent) => Methods.ulViewFireMouseEvent(this, &mouseEvent);
	public void FireScrollEvent(ULScrollEvent scrollEvent) => Methods.ulViewFireScrollEvent(this, &scrollEvent);

	public event Action<string>? OnChangeTitle;
	public event Action<string>? OnChangeURL;
	public event Action<string>? OnChangeTooltip;
	public event Action<ULCursor>? OnChangeCursor;
	public event AddConsoleMessageCallback? OnAddConsoleMessage;
	public event CreateChildViewCallback? OnCreateChildView;
	public event CreateInspectorViewCallback? OnCreateInspectorViewCallback;
	public event BeginLoadingCallback? OnBeginLoading;
	public event FinishLoadingCallback? OnFinishLoading;
	public event FailLoadingCallback? OnFailLoading;
	public event WindowObjectReadyCallback? OnWindowObjectReady;
	public event DOMReadyCallback? OnDomReady;
	public event Action? OnUpdateHistory;


	internal void SetUpCallbacks()
	{
		var data = Renderer!.GetCallbackData();
		Methods.ulViewSetChangeTitleCallback(this, &NativeOnChangeTitle, data);
		Methods.ulViewSetChangeURLCallback(this, &NativeOnChangeURL, data);
		Methods.ulViewSetChangeTooltipCallback(this, &NativeOnChangeTooltip, data);
		Methods.ulViewSetChangeCursorCallback(this, &NativeOnChangeCursor, data);
		Methods.ulViewSetAddConsoleMessageCallback(this, &NativeOnAddConsoleMessage, data);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
	static void NativeOnChangeTitle(nuint userData, void* caller, ULString* title) => GetView(userData, caller).OnChangeTitle?.Invoke(title->ToString());
	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
	static void NativeOnChangeURL(nuint userData, void* caller, ULString* url) => GetView(userData, caller).OnChangeURL?.Invoke(url->ToString());
	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
	static void NativeOnChangeTooltip(nuint userData, void* caller, ULString* tooltip) => GetView(userData, caller).OnChangeTooltip?.Invoke(tooltip->ToString());
	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
	static void NativeOnChangeCursor(nuint userData, void* caller, ULCursor cursor) => GetView(userData, caller).OnChangeCursor?.Invoke(cursor);
	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
	static void NativeOnAddConsoleMessage(nuint userData, void* caller, ULMessageSource source, ULMessageLevel level, ULString* message, uint lineNumber, uint columnNumber, ULString* sourceId) => GetView(userData, caller).OnAddConsoleMessage?.Invoke(source, level, message->ToString(), lineNumber, columnNumber, sourceId->ToString());



	public bool NeedsPaint { get => Methods.ulViewGetNeedsPaint(this); set => Methods.ulViewSetNeedsPaint(this, value); }

	public View CreateInspectorView() => FromHandle(Methods.ulViewCreateInspectorView(this));


	public override void Dispose()
	{
		if (!IsDisposed && Owns) Methods.ulDestroyView(this);
		GC.KeepAlive(Renderer);
		base.Dispose();
	}

	internal static View FromHandle(void* handle, bool dispose = true) => new() { Handle = handle, Owns = dispose };

	[CustomMarshaller(typeof(View), MarshalMode.ManagedToUnmanagedIn, typeof(Marshaller))]
	internal ref struct Marshaller
	{
		private View view;

		public void FromManaged(View view) => this.view = view;
		public readonly void* ToUnmanaged() => view.Handle;
		public readonly void Free() => GC.KeepAlive(view);
	}

	static View GetView(nuint userData, void* caller)
	{
		if (Renderer.renderers[userData].TryGetTarget(out var renderer))
		{
			if (renderer.views[(nuint)caller].TryGetTarget(out var view))
			{
				return view;
			}
			else throw new ObjectDisposedException(nameof(View));
		}
		else throw new ObjectDisposedException(nameof(UltralightNet.Renderer));
	}
}
