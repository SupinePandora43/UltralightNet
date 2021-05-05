using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		[DllImport("Ultralight")]
		public static extern IntPtr ulCreateView(IntPtr renderer, uint width, uint height, IntPtr view_config, IntPtr session);

		[DllImport("Ultralight")]
		public static extern void ulDestroyView(IntPtr view);

		[GeneratedDllImport("Ultralight", CharSet = CharSet.Unicode)]
		[return: MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))]
		public static partial string ulViewGetURL(IntPtr view);

		[GeneratedDllImport("Ultralight")]
		[return: MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))]
		public static partial string ulViewGetTitle(IntPtr view);

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

		[GeneratedDllImport("Ultralight")]
		public static partial void ulViewLoadHTML(IntPtr view, [MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))] string html_string);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulViewLoadURL(IntPtr view, [MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))] string url_string);

		[DllImport("Ultralight")]
		public static extern void ulViewResize(IntPtr view, uint width, uint height);

		//todo: JavaScriptCore bindings
		[DllImport("Ultralight")]
		public static extern IntPtr ulViewLockJSContext(IntPtr view);

		[DllImport("Ultralight")]
		public static extern void ulViewUnlockJSContext(IntPtr view);

		[GeneratedDllImport("Ultralight")]
		[return: MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))]
		public static partial string ulViewEvaluateScript(IntPtr view, [MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))] string js_string, [MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))] out string exception);

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

		[DllImport("Ultralight")]
		public static extern void ulViewFireKeyEvent(IntPtr view, IntPtr key_event);

		[DllImport("Ultralight")]
		public static extern void ulViewFireMouseEvent(IntPtr view, IntPtr mouse_event);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulViewFireScrollEvent(IntPtr view, ref ULScrollEvent scroll_event);

		[DllImport("Ultralight")]
		public static extern void ulViewSetChangeTitleCallback(IntPtr view, ULChangeTitleCallback__PInvoke__ callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetChangeURLCallback(IntPtr view, ULChangeURLCallback__PInvoke__ callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetChangeTooltipCallback(IntPtr view, ULChangeTooltipCallback__PInvoke__ callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetChangeCursorCallback(IntPtr view, ULChangeCursorCallback__PInvoke__ callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetAddConsoleMessageCallback(IntPtr view, ULAddConsoleMessageCallback__PInvoke__ callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetCreateChildViewCallback(IntPtr view, ULCreateChildViewCallback__PInvoke__ callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetBeginLoadingCallback(IntPtr view, ULBeginLoadingCallback__PInvoke__ callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetFinishLoadingCallback(IntPtr view, ULFinishLoadingCallback__PInvoke__ callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetFailLoadingCallback(IntPtr view, ULFailLoadingCallback__PInvoke__ callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetWindowObjectReadyCallback(IntPtr view, ULWindowObjectReadyCallback__PInvoke__ callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetDOMReadyCallback(IntPtr view, ULDOMReadyCallback__PInvoke__ callback, IntPtr user_data);

		[DllImport("Ultralight")]
		public static extern void ulViewSetUpdateHistoryCallback(IntPtr view, ULUpdateHistoryCallback__PInvoke__ callback, IntPtr user_data);

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
		//public View(Renderer renderer, uint width, uint height, ULViewConfig view_config) : this(renderer, width, height, view_config, Session.DefaultSession(renderer)) {}
		public View(Renderer renderer, uint width, uint height, ULViewConfig view_config = default, Session session = default)
		{
			Ptr = Methods.ulCreateView(renderer.Ptr, width, height, (view_config is default(ULViewConfig)) ? new ULViewConfig().Ptr : view_config.Ptr, (session is default(Session)) ? Session.DefaultSession(renderer).Ptr : session.Ptr);
		}

		private readonly GCHandle[] handles = new GCHandle[12];
		private void Handle(int key, GCHandle handle)
		{
			if (handles[key].IsAllocated)
			{
				handles[key].Free();
			}
			handles[key] = handle;
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
		public RenderTarget RenderTarget
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Methods.ulViewGetRenderTarget(Ptr);
		}

		public ULSurface Surface
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

		public void FireKeyEvent(ULKeyEvent keyEvent) => Methods.ulViewFireKeyEvent(Ptr, keyEvent.Ptr);
		public void FireMouseEvent(ULMouseEvent mouseEvent) => Methods.ulViewFireMouseEvent(Ptr, mouseEvent.Ptr);
		public void FireScrollEvent(ref ULScrollEvent scrollEvent) => Methods.ulViewFireScrollEvent(Ptr, ref scrollEvent);
		public void FireScrollEvent(ULScrollEvent scrollEvent) => Methods.ulViewFireScrollEvent(Ptr, ref scrollEvent);

		#region Callbacks
		public void SetChangeTitleCallback(ULChangeTitleCallback callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULChangeTitleCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, title) => callback(user_data, new View(caller), ULStringMarshaler.NativeToManaged(title));
				Handle(0, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetChangeTitleCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(0, default);
				Methods.ulViewSetChangeTitleCallback(Ptr, null, userData);
			}
		}
		public void SetChangeURLCallback(ULChangeURLCallback callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULChangeURLCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, url) => callback(user_data, new View(caller), ULStringMarshaler.NativeToManaged(url));
				Handle(1, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetChangeURLCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(1, default);
				Methods.ulViewSetChangeURLCallback(Ptr, null, userData);
			}
		}
		public void SetChangeTooltipCallback(ULChangeTooltipCallback callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULChangeTooltipCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, tooltip) => callback(user_data, new View(caller), ULStringMarshaler.NativeToManaged(tooltip));
				Handle(2, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetChangeTooltipCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(2, default);
				Methods.ulViewSetChangeTooltipCallback(Ptr, null, userData);
			}
		}
		public void SetChangeCursorCallback(ULChangeCursorCallback callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULChangeCursorCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, cursor) => callback(user_data, new View(caller), cursor);
				Handle(3, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetChangeCursorCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(3, default);
				Methods.ulViewSetChangeCursorCallback(Ptr, null, userData);
			}
		}
		public void SetAddConsoleMessageCallback(ULAddConsoleMessageCallback callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULAddConsoleMessageCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, source, level, message, line_number, column_number, source_id) => callback(user_data, new View(caller), source, level, ULStringMarshaler.NativeToManaged(message), line_number, column_number, ULStringMarshaler.NativeToManaged(source_id));
				Handle(4, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetAddConsoleMessageCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(4, default);
				Methods.ulViewSetAddConsoleMessageCallback(Ptr, null, userData);
			}
		}
		public void SetCreateChildViewCallback(ULCreateChildViewCallback callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULCreateChildViewCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, opener_url, target_url, is_popup, popup_rect) =>
				{
					View view = callback(user_data, new View(caller), ULStringMarshaler.NativeToManaged(opener_url), ULStringMarshaler.NativeToManaged(target_url), is_popup != 0, popup_rect);
					return view is null ? IntPtr.Zero : view.Ptr;
				};
				Handle(5, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetCreateChildViewCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(5, default);
				Methods.ulViewSetCreateChildViewCallback(Ptr, null, userData);
			}
		}
		public void SetBeginLoadingCallback(ULBeginLoadingCallback callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULBeginLoadingCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, frame_id, is_main_frame, url) => callback(user_data, new View(caller), frame_id, is_main_frame != 0, ULStringMarshaler.NativeToManaged(url));
				Handle(6, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetBeginLoadingCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(6, default);
				Methods.ulViewSetBeginLoadingCallback(Ptr, null, userData);
			}
		}
		public void SetFinishLoadingCallback(ULFinishLoadingCallback callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULFinishLoadingCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, frame_id, is_main_frame, url) => callback(user_data, new View(caller), frame_id, is_main_frame != 0, ULStringMarshaler.NativeToManaged(url));
				Handle(7, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetFinishLoadingCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(7, default);
				Methods.ulViewSetFinishLoadingCallback(Ptr, null, userData);
			}
		}
		public void SetFailLoadingCallback(ULFailLoadingCallback callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULFailLoadingCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, frame_id, is_main_frame, url, description, error_domain, error_code) => callback(user_data, new View(caller), frame_id, is_main_frame != 0, ULStringMarshaler.NativeToManaged(url), ULStringMarshaler.NativeToManaged(description), ULStringMarshaler.NativeToManaged(error_domain), error_code);
				Handle(8, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetFailLoadingCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(8, default);
				Methods.ulViewSetFailLoadingCallback(Ptr, null, userData);
			}
		}
		public void SetWindowObjectReadyCallback(ULWindowObjectReadyCallback callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULWindowObjectReadyCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, frame_id, is_main_frame, url) => callback(user_data, new View(caller), frame_id, is_main_frame != 0, ULStringMarshaler.NativeToManaged(url));
				Handle(9, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetWindowObjectReadyCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(9, default);
				Methods.ulViewSetWindowObjectReadyCallback(Ptr, null, userData);
			}
		}
		public void SetDOMReadyCallback(ULDOMReadyCallback callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULDOMReadyCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, frame_id, is_main_frame, url) => callback(user_data, new View(caller), frame_id, is_main_frame != 0, ULStringMarshaler.NativeToManaged(url));
				Handle(10, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetDOMReadyCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(10, default);
				Methods.ulViewSetDOMReadyCallback(Ptr, null, userData);
			}
		}
		public void SetUpdateHistoryCallback(ULUpdateHistoryCallback callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULUpdateHistoryCallback__PInvoke__ callback__PInvoke__ = (user_data, caller) => callback(user_data, new View(caller));
				Handle(11, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetUpdateHistoryCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(11, default);
				Methods.ulViewSetUpdateHistoryCallback(Ptr, null, userData);
			}
		}

		#endregion Callbacks

		public bool NeedsPaint { get => Methods.ulViewGetNeedsPaint(Ptr); set => Methods.ulViewSetNeedsPaint(Ptr, value); }

		public View CreateInspectorView() => new(Methods.ulViewCreateInspectorView(Ptr), true);


		~View() => Dispose();
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

			public static ICustomMarshaler GetInstance(string _) => instance;

			public void CleanUpManagedData(object ManagedObj) { }

			public void CleanUpNativeData(IntPtr pNativeData) { }

			public int GetNativeDataSize() => 1;

			public IntPtr MarshalManagedToNative(object ManagedObj) => ((View)ManagedObj).Ptr;

			public object MarshalNativeToManaged(IntPtr pNativeData) => new View(pNativeData);
		}
	}
}
