using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static unsafe partial class Methods
	{
		[DllImport(LibUltralight)]
		public static extern void* ulCreateView(void* renderer, uint width, uint height, _ULViewConfig* viewConfig, void* session);
		// INTEROPTODO: NATIVEMARSHALLING
		//[GeneratedDllImport(LibUltralight)]
		public static IntPtr ulCreateView(IntPtr renderer, uint width, uint height, in ULViewConfig viewConfig, IntPtr session)
		{
			_ULViewConfig nativeConfig = new(viewConfig);
			var ret = ulCreateView((void*)renderer, width, height, &nativeConfig, (void*)session);
			nativeConfig.Dispose();
			return (IntPtr)ret;
		}

		[DllImport(LibUltralight)]
		public static extern void ulDestroyView(IntPtr view);

		[GeneratedDllImport(LibUltralight)]
		[return: MarshalUsing(typeof(ULString.ToManaged_))]
		public static partial string ulViewGetURL(IntPtr view);

		[GeneratedDllImport(LibUltralight)]
		[return: MarshalUsing(typeof(ULString.ToManaged_))]
		public static partial string ulViewGetTitle(IntPtr view);

		[DllImport(LibUltralight)]
		public static extern uint ulViewGetWidth(IntPtr view);

		[DllImport(LibUltralight)]
		public static extern uint ulViewGetHeight(IntPtr view);

		[DllImport(LibUltralight)]
		public static extern double ulViewGetDeviceScale(IntPtr view);

		[DllImport(LibUltralight)]
		public static extern void ulViewSetDeviceScale(IntPtr view, double scale);

		[GeneratedDllImport(LibUltralight)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulViewIsAccelerated(IntPtr view);

		[GeneratedDllImport(LibUltralight)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulViewIsTransparent(IntPtr view);

		[GeneratedDllImport(LibUltralight)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulViewIsLoading(IntPtr view);

		[DllImport(LibUltralight)]
		public static extern RenderTarget ulViewGetRenderTarget(IntPtr view);

		[DllImport(LibUltralight)]
		public static extern IntPtr ulViewGetSurface(IntPtr view);

		[GeneratedDllImport(LibUltralight)]
		public static partial void ulViewLoadHTML(IntPtr view, [MarshalUsing(typeof(ULString.ToNative))] string html_string);

		[GeneratedDllImport(LibUltralight)]
		public static partial void ulViewLoadURL(IntPtr view, [MarshalUsing(typeof(ULString.ToNative))] string url_string);

		[DllImport(LibUltralight)]
		public static extern void ulViewResize(IntPtr view, uint width, uint height);

		//todo: JavaScriptCore bindings
		[DllImport(LibUltralight)]
		public static extern unsafe void* ulViewLockJSContext(IntPtr view);

		[DllImport(LibUltralight)]
		public static extern void ulViewUnlockJSContext(IntPtr view);

		[GeneratedDllImport(LibUltralight)]
		[return: MarshalUsing(typeof(ULString.ToManaged_))]
		public static partial string ulViewEvaluateScript(IntPtr view, [MarshalUsing(typeof(ULString.ToNative))] string js, [MarshalUsing(typeof(ULString.ToManaged_))] out string exception);

		/// <summary>Check if can navigate backwards in history.</summary>
		[GeneratedDllImport(LibUltralight)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulViewCanGoBack(IntPtr view);

		/// <summary>Check if can navigate forwards in history.</summary>
		[GeneratedDllImport(LibUltralight)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulViewCanGoForward(IntPtr view);

		/// <summary>Navigate backwards in history.</summary>
		[DllImport(LibUltralight)]
		public static extern void ulViewGoBack(IntPtr view);

		/// <summary>Navigate forwards in history.</summary>
		[DllImport(LibUltralight)]
		public static extern void ulViewGoForward(IntPtr view);

		/// <summary>Navigate to arbitrary offset in history.</summary>
		[DllImport(LibUltralight)]
		public static extern void ulViewGoToHistoryOffset(IntPtr view, int offset);

		/// <summary>Reload current page.</summary>
		[DllImport(LibUltralight)]
		public static extern void ulViewReload(IntPtr view);

		/// <summary>Stop all page loads.</summary>
		[DllImport(LibUltralight)]
		public static extern void ulViewStop(IntPtr view);

		/// <summary>Give focus to the View.</summary>
		/// <remarks>
		/// You should call this to give visual indication that the View has input
		/// focus (changes active text selection colors, for example).
		/// </remarks>
		[DllImport(LibUltralight)]
		public static extern void ulViewFocus(IntPtr view);

		/// <summary>Remove focus from the View and unfocus any focused input elements.</summary>
		/// <remarks>
		/// You should call this to give visual indication that the View has lost
		/// input focus.
		/// </remarks>
		[DllImport(LibUltralight)]
		public static extern void ulViewUnfocus(IntPtr view);

		/// <summary>Whether or not the View has focus.</summary>
		[GeneratedDllImport(LibUltralight)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulViewHasFocus(IntPtr view);

		/// <summary>Whether or not the View has an input element with visible keyboard focus (indicated by a blinking caret).</summary>
		/// <remarks>
		/// You can use this to decide whether or not the View should consume
		/// keyboard input events (useful in games with mixed UI and key handling).
		/// </remarks>
		[GeneratedDllImport(LibUltralight)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulViewHasInputFocus(IntPtr view);

		[DllImport(LibUltralight)]
		public static extern void ulViewFireKeyEvent(IntPtr view, IntPtr key_event);

		[DllImport(LibUltralight)]
		public static extern unsafe void ulViewFireMouseEvent(IntPtr view, ULMouseEvent* mouseEvent);

		[DllImport(LibUltralight)]
		public static extern unsafe void ulViewFireScrollEvent(IntPtr view, ULScrollEvent* scrollEvent);

		[GeneratedDllImport(LibUltralight)]
		public static partial void ulViewSetChangeTitleCallback(IntPtr view, ULChangeTitleCallback__PInvoke__? callback, IntPtr user_data);

		[GeneratedDllImport(LibUltralight)]
		public static partial void ulViewSetChangeURLCallback(IntPtr view, ULChangeURLCallback__PInvoke__? callback, IntPtr user_data);

		[GeneratedDllImport(LibUltralight)]
		public static partial void ulViewSetChangeTooltipCallback(IntPtr view, ULChangeTooltipCallback__PInvoke__? callback, IntPtr user_data);

		[GeneratedDllImport(LibUltralight)]
		public static partial void ulViewSetChangeCursorCallback(IntPtr view, ULChangeCursorCallback__PInvoke__? callback, IntPtr user_data);

		[GeneratedDllImport(LibUltralight)]
		public static partial void ulViewSetAddConsoleMessageCallback(IntPtr view, ULAddConsoleMessageCallback__PInvoke__? callback, IntPtr user_data);

		[GeneratedDllImport(LibUltralight)]
		public static partial void ulViewSetCreateChildViewCallback(IntPtr view, ULCreateChildViewCallback__PInvoke__? callback, IntPtr user_data);

		[GeneratedDllImport(LibUltralight)]
		public static partial void ulViewSetBeginLoadingCallback(IntPtr view, ULBeginLoadingCallback__PInvoke__? callback, IntPtr user_data);

		[GeneratedDllImport(LibUltralight)]
		public static partial void ulViewSetFinishLoadingCallback(IntPtr view, ULFinishLoadingCallback__PInvoke__? callback, IntPtr user_data);

		[GeneratedDllImport(LibUltralight)]
		public static partial void ulViewSetFailLoadingCallback(IntPtr view, ULFailLoadingCallback__PInvoke__? callback, IntPtr user_data);

		[GeneratedDllImport(LibUltralight)]
		public static partial void ulViewSetWindowObjectReadyCallback(IntPtr view, ULWindowObjectReadyCallback__PInvoke__? callback, IntPtr user_data);

		[GeneratedDllImport(LibUltralight)]
		public static partial void ulViewSetDOMReadyCallback(IntPtr view, ULDOMReadyCallback__PInvoke__? callback, IntPtr user_data);

		[GeneratedDllImport(LibUltralight)]
		public static partial void ulViewSetUpdateHistoryCallback(IntPtr view, ULUpdateHistoryCallback__PInvoke__? callback, IntPtr user_data);

		[GeneratedDllImport(LibUltralight)]
		public static partial void ulViewSetNeedsPaint(IntPtr view, [MarshalAs(UnmanagedType.I1)] bool needs_paint);

		[GeneratedDllImport(LibUltralight)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulViewGetNeedsPaint(IntPtr view);

		[DllImport(LibUltralight)]
		public static extern IntPtr ulViewCreateInspectorView(IntPtr view);
	}

	public unsafe class View : IDisposable
	{
		private void* _ptr;
		public IntPtr Ptr
		{
			get
			{
				static void Throw() => throw new ObjectDisposedException(nameof(View));
				if (IsDisposed) Throw();
				ULPlatform.CheckThread();
				return (IntPtr)_ptr;
			}
			private set => _ptr = value.ToPointer();
		}
		public bool IsDisposed { get; private set; } = false;
		private bool dispose = true;
		internal Renderer? Renderer { get; set; }

		private JSContext Context { get; set; }
		private JSContext? lockedContext;

		public View(IntPtr ptr, bool dispose = false)
		{
			Ptr = ptr;
			this.dispose = dispose;
			Context = new();
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

		public string URL
		{
			get => Methods.ulViewGetURL(Ptr);
			set => Methods.ulViewLoadURL(Ptr, value);
		}
		public string HTML { set => Methods.ulViewLoadHTML(Ptr, value); }
		public string Title { get => Methods.ulViewGetTitle(Ptr); }

		public uint Width => Methods.ulViewGetWidth(Ptr);
		public uint Height => Methods.ulViewGetHeight(Ptr);
		public double DeviceScale { get => Methods.ulViewGetDeviceScale(Ptr); set => Methods.ulViewSetDeviceScale(Ptr, value); }

		public bool IsAccelerated => Methods.ulViewIsAccelerated(Ptr);
		public bool IsTransparent => Methods.ulViewIsTransparent(Ptr);

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

		public ULSurface? Surface
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				IntPtr surfacePtr = Methods.ulViewGetSurface(Ptr);
				if (surfacePtr == IntPtr.Zero) return null;
				return new(surfacePtr);
			}
		}

		public void Resize(in uint width, in uint height) => Methods.ulViewResize(Ptr, width, height);

		public ref readonly JSContext LockJSContext()
		{
			void* contextHandle = Methods.ulViewLockJSContext(Ptr);
			Context.OnLocked(contextHandle);
			lockedContext = Context;
			return ref lockedContext!;
		}
		public void UnlockJSContext()
		{
			lockedContext = null;
			Methods.ulViewUnlockJSContext(Ptr);
		}

		public string EvaluateScript(string js_string, out string exception) => Methods.ulViewEvaluateScript(Ptr, js_string, out exception);

		public bool CanGoBack => Methods.ulViewCanGoBack(Ptr);
		public bool CanGoForward => Methods.ulViewCanGoForward(Ptr);

		public void GoBack() => Methods.ulViewGoBack(Ptr);
		public void GoForward() => Methods.ulViewGoForward(Ptr);
		public void GoToHistoryOffset(in int offset) => Methods.ulViewGoToHistoryOffset(Ptr, offset);

		public void Reload() => Methods.ulViewReload(Ptr);
		public void Stop() => Methods.ulViewStop(Ptr);

		public void Focus() => Methods.ulViewFocus(Ptr);
		public void Unfocus() => Methods.ulViewUnfocus(Ptr);
		public bool HasFocus => Methods.ulViewHasFocus(Ptr);
		public bool HasInputFocus => Methods.ulViewHasInputFocus(Ptr);

		public void FireKeyEvent(in ULKeyEvent keyEvent) => Methods.ulViewFireKeyEvent(Ptr, keyEvent.Ptr);
		public void FireMouseEvent(ULMouseEvent mouseEvent) => Methods.ulViewFireMouseEvent(Ptr, &mouseEvent);
		public void FireScrollEvent(ULScrollEvent scrollEvent) => Methods.ulViewFireScrollEvent(Ptr, &scrollEvent);

		#region Callbacks

		private event ULChangeTitleCallbackEvent? _OnChangeTitle;
		private event ULChangeURLCallbackEvent? _OnChangeURL;
		private event ULChangeTooltipCallbackEvent? _OnChangeTooltip;
		private event ULChangeCursorCallbackEvent? _OnChangeCursor;
		private event ULAddConsoleMessageCallbackEvent? _OnAddConsoleMessage;
		private event ULCreateChildViewCallbackEvent? _OnCreateChildView;
		private event ULBeginLoadingCallbackEvent? _OnBeginLoading;
		private event ULFinishLoadingCallbackEvent? _OnFinishLoading;
		private event ULFailLoadingCallbackEvent? _OnFailLoading;
		private event ULWindowObjectReadyCallbackEvent? _OnWindowObjectReady;
		private event ULDOMReadyCallbackEvent? _OnDomReady;
		private event ULUpdateHistoryCallbackEvent? _OnUpdateHistory;

		public event ULChangeTitleCallbackEvent OnChangeTitle
		{
			add
			{
				if (_OnChangeTitle is null)
				{
					_OnChangeTitle += value;
					SetChangeTitleCallback((user_data, caller, title) => _OnChangeTitle?.Invoke(title));
				}
				else
				{
					_OnChangeTitle += value;
				}
			}
			remove
			{
				_OnChangeTitle -= value;
				if (_OnChangeTitle is null)
				{
					SetChangeTitleCallback(null);
				}
			}
		}
		public event ULChangeURLCallbackEvent OnChangeURL
		{
			add
			{
				if (_OnChangeURL is null)
				{
					_OnChangeURL += value;
					SetChangeURLCallback((user_data, caller, url) => _OnChangeURL?.Invoke(url));
				}
				else
				{
					_OnChangeURL += value;
				}
			}
			remove
			{
				_OnChangeURL -= value;
				if (_OnChangeURL is null)
				{
					SetChangeURLCallback(null);
				}
			}
		}
		public event ULChangeTooltipCallbackEvent OnChangeTooltip
		{
			add
			{
				if (_OnChangeTooltip is null)
				{
					_OnChangeTooltip += value;
					SetChangeTooltipCallback((user_data, caller, tooltip) => _OnChangeTooltip?.Invoke(tooltip));
				}
				else
				{
					_OnChangeTooltip += value;
				}
			}
			remove
			{
				_OnChangeTooltip -= value;
				if (_OnChangeTooltip is null)
				{
					SetChangeTooltipCallback(null);
				}
			}
		}
		public event ULChangeCursorCallbackEvent OnChangeCursor
		{
			add
			{
				if (_OnChangeCursor is null)
				{
					_OnChangeCursor += value;
					SetChangeCursorCallback((user_data, caller, cursor) => _OnChangeCursor?.Invoke(cursor));
				}
				else
				{
					_OnChangeCursor += value;
				}
			}
			remove
			{
				_OnChangeCursor -= value;
				if (_OnChangeCursor is null)
				{
					SetChangeCursorCallback(null);
				}
			}
		}
		public event ULAddConsoleMessageCallbackEvent OnAddConsoleMessage
		{
			add
			{
				if (_OnAddConsoleMessage is null)
				{
					_OnAddConsoleMessage += value;
					SetAddConsoleMessageCallback((user_data, caller, source, level, message, line_number, column_number, source_id) => _OnAddConsoleMessage?.Invoke(source, level, message, line_number, column_number, source_id));
				}
				else
				{
					_OnAddConsoleMessage += value;
				}
			}
			remove
			{
				_OnAddConsoleMessage -= value;
				if (_OnAddConsoleMessage is null)
				{
					SetAddConsoleMessageCallback(null);
				}
			}
		}
		public event ULCreateChildViewCallbackEvent OnCreateChildView
		{
			add
			{
				if (_OnCreateChildView is null)
				{
					_OnCreateChildView += value;
					SetCreateChildViewCallback((user_data, caller, openerUrl, targetUrl, isPopup, popupRect) => _OnCreateChildView?.Invoke(openerUrl, targetUrl, isPopup, popupRect));
				}
				else
				{
					_OnCreateChildView += value;
				}
			}
			remove
			{
				_OnCreateChildView -= value;
				if (_OnCreateChildView is null)
				{
					SetCreateChildViewCallback(null);
				}
			}
		}
		public event ULBeginLoadingCallbackEvent OnBeginLoading
		{
			add
			{
				if (_OnBeginLoading is null)
				{
					_OnBeginLoading += value;
					SetBeginLoadingCallback((user_data, caller, frameId, isMainFrame, url) => _OnBeginLoading?.Invoke(frameId, isMainFrame, url));
				}
				else
				{
					_OnBeginLoading += value;
				}
			}
			remove
			{
				_OnBeginLoading -= value;
				if (_OnBeginLoading is null)
				{
					SetBeginLoadingCallback(null);
				}
			}
		}
		public event ULFinishLoadingCallbackEvent OnFinishLoading
		{
			add
			{
				if (_OnFinishLoading is null)
				{
					_OnFinishLoading += value;
					SetFinishLoadingCallback((user_data, caller, frameId, isMainFrame, url) => _OnFinishLoading?.Invoke(frameId, isMainFrame, url));
				}
				else
				{
					_OnFinishLoading += value;
				}
			}
			remove
			{
				_OnFinishLoading -= value;
				if (_OnFinishLoading is null)
				{
					SetFinishLoadingCallback(null);
				}
			}
		}
		public event ULFailLoadingCallbackEvent OnFailLoading
		{
			add
			{
				if (_OnFailLoading is null)
				{
					_OnFailLoading += value;
					SetFailLoadingCallback((user_data, caller, frameId, isMainFrame, url, description, errorDomain, errorCode) => _OnFailLoading?.Invoke(frameId, isMainFrame, url, description, errorDomain, errorCode));
				}
				else
				{
					_OnFailLoading += value;
				}
			}
			remove
			{
				_OnFailLoading -= value;
				if (_OnFailLoading is null)
				{
					SetFailLoadingCallback(null);
				}
			}
		}
		public event ULWindowObjectReadyCallbackEvent OnWindowObjectReady
		{
			add
			{
				if (_OnWindowObjectReady is null)
				{
					_OnWindowObjectReady += value;
					SetWindowObjectReadyCallback((user_data, caller, frameId, isMainFrame, url) => _OnWindowObjectReady?.Invoke(frameId, isMainFrame, url));
				}
				else
				{
					_OnWindowObjectReady += value;
				}
			}
			remove
			{
				_OnWindowObjectReady -= value;
				if (_OnWindowObjectReady is null)
				{
					SetWindowObjectReadyCallback(null);
				}
			}
		}
		public event ULDOMReadyCallbackEvent OnDomReady
		{
			add
			{
				if (_OnDomReady is null)
				{
					_OnDomReady += value;
					SetDOMReadyCallback((user_data, caller, frameId, isMainFrame, url) => _OnDomReady?.Invoke(frameId, isMainFrame, url));
				}
				else
				{
					_OnDomReady += value;
				}
			}
			remove
			{
				_OnDomReady -= value;
				if (_OnDomReady is null)
				{
					SetDOMReadyCallback(null);
				}
			}
		}
		public event ULUpdateHistoryCallbackEvent OnUpdateHistory
		{
			add
			{
				if (_OnUpdateHistory is null)
				{
					_OnUpdateHistory += value;
					SetUpdateHistoryCallback((user_data, caller) => _OnUpdateHistory?.Invoke());
				}
				else
				{
					_OnUpdateHistory += value;
				}
			}
			remove
			{
				_OnUpdateHistory -= value;
				if (_OnUpdateHistory is null)
				{
					SetUpdateHistoryCallback(null);
				}
			}
		}

		private void SetChangeTitleCallback(ULChangeTitleCallback? callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULChangeTitleCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, title) => callback(user_data, new View(caller), ULString.NativeToManaged(title));
				Handle(0, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetChangeTitleCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(0, default);
				Methods.ulViewSetChangeTitleCallback(Ptr, null, userData);
			}
		}
		private void SetChangeURLCallback(ULChangeURLCallback? callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULChangeURLCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, url) => callback(user_data, new View(caller), ULString.NativeToManaged(url));
				Handle(1, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetChangeURLCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(1, default);
				Methods.ulViewSetChangeURLCallback(Ptr, null, userData);
			}
		}
		private void SetChangeTooltipCallback(ULChangeTooltipCallback? callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULChangeTooltipCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, tooltip) => callback(user_data, new View(caller), ULString.NativeToManaged(tooltip));
				Handle(2, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetChangeTooltipCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(2, default);
				Methods.ulViewSetChangeTooltipCallback(Ptr, null, userData);
			}
		}
		private void SetChangeCursorCallback(ULChangeCursorCallback? callback, IntPtr userData = default)
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
		private void SetAddConsoleMessageCallback(ULAddConsoleMessageCallback? callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULAddConsoleMessageCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, source, level, message, line_number, column_number, source_id) => callback(user_data, new View(caller), source, level, ULString.NativeToManaged(message), line_number, column_number, ULString.NativeToManaged(source_id));
				Handle(4, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetAddConsoleMessageCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(4, default);
				Methods.ulViewSetAddConsoleMessageCallback(Ptr, null, userData);
			}
		}
		private void SetCreateChildViewCallback(ULCreateChildViewCallback? callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULCreateChildViewCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, opener_url, target_url, is_popup, popup_rect) =>
				{
					View? view = callback(user_data, new View(caller), ULString.NativeToManaged(opener_url), ULString.NativeToManaged(target_url), is_popup != 0, popup_rect);
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
		private void SetBeginLoadingCallback(ULBeginLoadingCallback? callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULBeginLoadingCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, frame_id, is_main_frame, url) => callback(user_data, new View(caller), frame_id, is_main_frame != 0, ULString.NativeToManaged(url));
				Handle(6, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetBeginLoadingCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(6, default);
				Methods.ulViewSetBeginLoadingCallback(Ptr, null, userData);
			}
		}
		private void SetFinishLoadingCallback(ULFinishLoadingCallback? callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULFinishLoadingCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, frame_id, is_main_frame, url) => callback(user_data, new View(caller), frame_id, is_main_frame != 0, ULString.NativeToManaged(url));
				Handle(7, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetFinishLoadingCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(7, default);
				Methods.ulViewSetFinishLoadingCallback(Ptr, null, userData);
			}
		}
		private void SetFailLoadingCallback(ULFailLoadingCallback? callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULFailLoadingCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, frame_id, is_main_frame, url, description, error_domain, error_code) => callback(user_data, new View(caller), frame_id, is_main_frame != 0, ULString.NativeToManaged(url), ULString.NativeToManaged(description), ULString.NativeToManaged(error_domain), error_code);
				Handle(8, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetFailLoadingCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(8, default);
				Methods.ulViewSetFailLoadingCallback(Ptr, null, userData);
			}
		}
		private void SetWindowObjectReadyCallback(ULWindowObjectReadyCallback? callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULWindowObjectReadyCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, frame_id, is_main_frame, url) => callback(user_data, new View(caller), frame_id, is_main_frame != 0, ULString.NativeToManaged(url));
				Handle(9, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetWindowObjectReadyCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(9, default);
				Methods.ulViewSetWindowObjectReadyCallback(Ptr, null, userData);
			}
		}
		private void SetDOMReadyCallback(ULDOMReadyCallback? callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				ULDOMReadyCallback__PInvoke__ callback__PInvoke__ = (user_data, caller, frame_id, is_main_frame, url) => callback(user_data, new View(caller), frame_id, is_main_frame != 0, ULString.NativeToManaged(url));
				Handle(10, GCHandle.Alloc(callback__PInvoke__, GCHandleType.Normal));
				Methods.ulViewSetDOMReadyCallback(Ptr, callback__PInvoke__, userData);
			}
			else
			{
				Handle(10, default);
				Methods.ulViewSetDOMReadyCallback(Ptr, null, userData);
			}
		}
		private void SetUpdateHistoryCallback(ULUpdateHistoryCallback? callback, IntPtr userData = default)
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

			if (IsDisposed || !dispose) return;

			Methods.ulDestroyView(Ptr);
			IsDisposed = true;
			GC.KeepAlive(Renderer);

			GC.SuppressFinalize(this);
		}

#nullable enable
		public static bool ReferenceEquals(View? a, View? b)
		{
			if ((a is null) || (b is null)) return object.ReferenceEquals(a, b);
			return a.Ptr == b.Ptr;
		}
#nullable restore
	}
}
