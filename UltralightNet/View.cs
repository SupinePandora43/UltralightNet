using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		[GeneratedDllImport("Ultralight")]
		public static partial IntPtr ulCreateView(IntPtr renderer, uint width, uint height, [MarshalAs(UnmanagedType.I1)] bool transparent, IntPtr session, [MarshalAs(UnmanagedType.I1)] bool force_cpu_renderer);

		[DllImport("Ultralight")]
		public static extern void ulDestroyView(IntPtr view);

		[DllImport("Ultralight", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
		public static extern string ulViewGetURL(IntPtr view);

		[DllImport("Ultralight")]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
		public static extern string ulViewGetTitle(IntPtr view);

		[DllImport("Ultralight")]
		public static extern uint ulViewGetWidth(IntPtr view);

		[DllImport("Ultralight")]
		public static extern uint ulViewGetHeight(IntPtr view);

		[GeneratedDllImport("Ultralight")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulViewIsLoading(IntPtr view);

		[GeneratedDllImport("Ultralight")]
		public static partial RenderTarget ulViewGetRenderTarget(IntPtr view);

		[DllImport("Ultralight")]
		public static extern IntPtr ulViewGetSurface(IntPtr view);

		[DllImport("Ultralight")]
		public static extern void ulViewLoadHTML(IntPtr view, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string html_string);

		[DllImport("Ultralight")]
		public static extern void ulViewLoadURL(IntPtr view, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string url_string);

		[DllImport("Ultralight")]
		public static extern void ulViewResize(IntPtr view, uint width, uint height);

		//todo: JavaScriptCore bindings
		[DllImport("Ultralight")]
		public static extern IntPtr ulViewLockJSContext(IntPtr view);

		[DllImport("Ultralight")]
		public static extern void ulViewUnlockJSContext(IntPtr view);

		[DllImport("Ultralight")]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
		public static extern string ulViewEvaluateScript(IntPtr view, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string js_string, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] out string exception);

		/// <summary>Check if can navigate backwards in history.</summary>
		[GeneratedDllImport("Ultralight")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulViewCanGoBack(IntPtr view);

		/// <summary>Check if can navigate forwards in history.</summary>
		[GeneratedDllImport("Ultralight")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulViewCanGoForward(IntPtr view);

		/// <summary>Navigate backwards in history.</summary>
		[DllImport("Ultralight")]
		public static extern void ulViewGoBack(IntPtr view);

		/// <summary>Navigate forwards in history.</summary>
		[DllImport("Ultralight")]
		public static extern void ulViewGoForward(IntPtr view);

		/// <summary>Navigate to arbitrary offset in history.</summary>
		[DllImport("Ultralight")]
		public static extern void ulViewGoToHistoryOffset(IntPtr view, int offset);

		/// <summary>Reload current page.</summary>
		[DllImport("Ultralight")]
		public static extern void ulViewReload(IntPtr view);

		/// <summary>Stop all page loads.</summary>
		[DllImport("Ultralight")]
		public static extern void ulViewStop(IntPtr view);

		/// <summary>Give focus to the View.</summary>
		/// <remarks>
		/// You should call this to give visual indication that the View has input
		/// focus (changes active text selection colors, for example).
		/// </remarks>
		[DllImport("Ultralight")]
		public static extern void ulViewFocus(IntPtr view);

		/// <summary>Remove focus from the View and unfocus any focused input elements.</summary>
		/// <remarks>
		/// You should call this to give visual indication that the View has lost
		/// input focus.
		/// </remarks>
		[DllImport("Ultralight")]
		public static extern void ulViewUnfocus(IntPtr view);

		/// <summary>Whether or not the View has focus.</summary>
		[GeneratedDllImport("Ultralight")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulViewHasFocus(IntPtr view);

		/// <summary>Whether or not the View has an input element with visible keyboard focus (indicated by a blinking caret).</summary>
		/// <remarks>
		/// You can use this to decide whether or not the View should consume
		/// keyboard input events (useful in games with mixed UI and key handling).
		/// </remarks>
		[GeneratedDllImport("Ultralight")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulViewHasInputFocus(IntPtr view);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulViewFireKeyEvent(IntPtr view, ULKeyEvent key_event);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulViewFireMouseEvent(IntPtr view, ULMouseEvent mouse_event);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulViewFireScrollEvent(IntPtr view, ULScrollEvent scroll_event);

		[DllImport("Ultralight")]
		public static extern void ulViewSetChangeTitleCallback(IntPtr view, ULChangeTitleCallback callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetChangeURLCallback(IntPtr view, ULChangeURLCallback callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetChangeTooltipCallback(IntPtr view, ULChangeTooltipCallback callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetChangeCursorCallback(IntPtr view, ULChangeCursorCallback callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetAddConsoleMessageCallback(IntPtr view, ULAddConsoleMessageCallback callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetCreateChildViewCallback(IntPtr view, ULCreateChildViewCallback callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetBeginLoadingCallback(IntPtr view, ULBeginLoadingCallback callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetFinishLoadingCallback(IntPtr view, ULFinishLoadingCallback callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetFailLoadingCallback(IntPtr view, ULFailLoadingCallback callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetWindowObjectReadyCallback(IntPtr view, ULWindowObjectReadyCallback callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetDOMReadyCallback(IntPtr view, ULDOMReadyCallback callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetUpdateHistoryCallback(IntPtr view, ULUpdateHistoryCallback callback, IntPtr user_data);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulViewSetNeedsPaint(IntPtr view, [MarshalAs(UnmanagedType.I1)] bool needs_paint);

		[GeneratedDllImport("Ultralight")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulViewGetNeedsPaint(IntPtr view);

		[DllImport("Ultralight")]
		public static extern IntPtr ulViewCreateInspectorView(IntPtr view);

		// to be continued https://github.com/ultralight-ux/Ultralight-API/blob/7f9de24ca1c7ec8b385e895c4899b9d96626da58/Ultralight/CAPI.h#L854
	}

	public class View : IDisposable
	{
		public IntPtr Ptr { get; private set; }
		public bool IsDisposed { get; private set; }

		public View(IntPtr ptr, bool dispose = false)
		{
			Ptr = ptr;
			IsDisposed = !dispose;
		}
		public View(Renderer renderer, uint width, uint height, bool transparent, Session session, bool force_cpu_renderer)
		{
			Ptr = Methods.ulCreateView(renderer.Ptr, width, height, transparent, session.Ptr, force_cpu_renderer);
		}

		public string URL { get => Methods.ulViewGetURL(Ptr); set => Methods.ulViewLoadURL(Ptr, value); }
		public string HTML { set => Methods.ulViewLoadHTML(Ptr, value); }
		public string Title { get => Methods.ulViewGetTitle(Ptr); }

		public uint Width => Methods.ulViewGetWidth(Ptr);
		public uint Height => Methods.ulViewGetHeight(Ptr);

		public bool IsLoading => Methods.ulViewIsLoading(Ptr);

		/// <summary>
		/// Provides info used to display texture in your application
		/// </summary>
		/// <remarks>Only valid when <see cref="ULGPUDriver"/> is used</remarks>
		public RenderTarget RenderTarget => Methods.ulViewGetRenderTarget(Ptr);

		public ULSurface Surface
		{
			get
			{
				IntPtr surfacePtr = Methods.ulViewGetSurface(Ptr);
				if (surfacePtr == IntPtr.Zero) return null;
				return new(surfacePtr);
			}
		}

		public void Resize(uint width, uint height) => Methods.ulViewResize(Ptr, width, height);

		public IntPtr LockJSContext() => Methods.ulViewLockJSContext(Ptr);
		public void UnlockJSContext() => Methods.ulViewUnlockJSContext(Ptr);

		public string EvaluateScript(string js_string, out string exception) => Methods.ulViewEvaluateScript(Ptr, js_string, out exception);

		public bool CanGoBack() => Methods.ulViewCanGoBack(Ptr);
		public bool CanGoForward() => Methods.ulViewCanGoForward(Ptr);

		public void GoBack() => Methods.ulViewGoBack(Ptr);
		public void GoForward() => Methods.ulViewGoForward(Ptr);
		public void GoToHistoryOffset(int offset) => Methods.ulViewGoToHistoryOffset(Ptr, offset);

		public void Reload() => Methods.ulViewReload(Ptr);
		public void Stop() => Methods.ulViewStop(Ptr);

		public void Focus() => Methods.ulViewFocus(Ptr);
		public void Unfocus() => Methods.ulViewUnfocus(Ptr);
		public bool HasFocus() => Methods.ulViewHasFocus(Ptr);
		public bool HasInputFocus() => Methods.ulViewHasInputFocus(Ptr);

		public void FireKeyEvent(ULKeyEvent keyEvent) => Methods.ulViewFireKeyEvent(Ptr, keyEvent);
		public void FireMouseEvent(ULMouseEvent mouseEvent) => Methods.ulViewFireMouseEvent(Ptr, mouseEvent);
		public void FireScrollEvent(ULScrollEvent scrollEvent) => Methods.ulViewFireScrollEvent(Ptr, scrollEvent);


		public void SetChangeTitleCallback(ULChangeTitleCallback callback, IntPtr userData = default) => Methods.ulViewSetChangeTitleCallback(Ptr, callback, userData);
		public void SetChangeURLCallback(ULChangeURLCallback callback, IntPtr userData = default) => Methods.ulViewSetChangeURLCallback(Ptr, callback, userData);
		public void SetChangeTooltipCallback(ULChangeTooltipCallback callback, IntPtr userData = default) => Methods.ulViewSetChangeTooltipCallback(Ptr, callback, userData);
		public void SetChangeCursorCallback(ULChangeCursorCallback callback, IntPtr userData = default) => Methods.ulViewSetChangeCursorCallback(Ptr, callback, userData);
		public void SetAddConsoleMessageCallback(ULAddConsoleMessageCallback callback, IntPtr userData = default) => Methods.ulViewSetAddConsoleMessageCallback(Ptr, callback, userData);
		public void SetCreateChildViewCallback(ULCreateChildViewCallback callback, IntPtr userData = default) => Methods.ulViewSetCreateChildViewCallback(Ptr, callback, userData);
		public void SetBeginLoadingCallback(ULBeginLoadingCallback callback, IntPtr userData = default) => Methods.ulViewSetBeginLoadingCallback(Ptr, callback, userData);
		public void SetFinishLoadingCallback(ULFinishLoadingCallback callback, IntPtr userData = default) => Methods.ulViewSetFinishLoadingCallback(Ptr, callback, userData);
		public void SetFailLoadingCallback(ULFailLoadingCallback callback, IntPtr userData = default) => Methods.ulViewSetFailLoadingCallback(Ptr, callback, userData);
		public void SetWindowObjectReadyCallback(ULWindowObjectReadyCallback callback, IntPtr userData = default) => Methods.ulViewSetWindowObjectReadyCallback(Ptr, callback, userData);
		public void SetDOMReadyCallback(ULDOMReadyCallback callback, IntPtr userData = default) => Methods.ulViewSetDOMReadyCallback(Ptr, callback, userData);
		public void SetUpdateHistoryCallback(ULUpdateHistoryCallback callback, IntPtr userData = default) => Methods.ulViewSetUpdateHistoryCallback(Ptr, callback, userData);

		public bool NeedsPaint { get => Methods.ulViewGetNeedsPaint(Ptr); set => Methods.ulViewSetNeedsPaint(Ptr, value); }

		public View CreateInspectorView() => new(Methods.ulViewCreateInspectorView(Ptr), true);


		~View() => Dispose();
		public void Dispose()
		{
			if (IsDisposed) return;
			Methods.ulDestroyView(Ptr);

			IsDisposed = true;
			GC.SuppressFinalize(this);
		}

#nullable enable
		public static bool ReferenceEquals(View? a, View? b)
		{
			if ((a is null) || (b is null)) return object.ReferenceEquals(a, b);
			return a.Ptr == b.Ptr;
		}
#nullable restore

		/// <summary>
		/// literally creates <see cref="View"/> from <see cref="IntPtr"/> and back, pls don't use
		/// </summary>
		public class Marshaler : ICustomMarshaler
		{
			private static readonly Marshaler instance = new();

			public static ICustomMarshaler GetInstance(string cookie) => instance;

			public void CleanUpManagedData(object ManagedObj) { }

			public void CleanUpNativeData(IntPtr pNativeData) { }

			public int GetNativeDataSize() => 1;

			public IntPtr MarshalManagedToNative(object ManagedObj) => ((View)ManagedObj).Ptr;

			public object MarshalNativeToManaged(IntPtr pNativeData) => new View(pNativeData);
		}
	}
}
