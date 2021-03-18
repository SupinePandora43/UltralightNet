using System;
using System.Runtime.InteropServices;
using UltralightNet.Structs;

namespace UltralightNet
{
	public static partial class Methods
	{
		[GeneratedDllImport("Ultralight")]
		public static partial IntPtr ulCreateView(IntPtr renderer, uint width, uint height, bool transparent, IntPtr session, bool force_cpu_renderer);

		[DllImport("Ultralight")]
		public static extern void ulDestroyView(IntPtr view);

		[DllImport("Ultralight", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))]
		public static extern string ulViewGetURL(IntPtr view);

		[DllImport("Ultralight")]
		public static extern IntPtr ulViewGetTitle(IntPtr view);

		[DllImport("Ultralight")]
		public static extern uint ulViewGetWidth(IntPtr view);

		[DllImport("Ultralight")]
		public static extern uint ulViewGetHeight(IntPtr view);

		[GeneratedDllImport("Ultralight")]
		public static partial bool ulViewIsLoading(IntPtr view);

		[GeneratedDllImport("Ultralight")]
		public static partial RenderTarget ulViewGetRenderTarget(IntPtr view);

		[DllImport("Ultralight")]
		public static extern IntPtr ulViewGetSurface(IntPtr view);

		[DllImport("Ultralight")]
		public static extern void ulViewLoadHTML(IntPtr view, IntPtr html_string);

		[DllImport("Ultralight")]
		public static extern void ulViewLoadURL(IntPtr view, [MarshalAs(UnmanagedType.CustomMarshaler,MarshalTypeRef =typeof(ULStringMarshaler))]string url_string);

		[DllImport("Ultralight")]
		public static extern void ulViewResize(IntPtr view, uint width, uint height);

		//todo: JavaScriptCore bindings
		[DllImport("Ultralight")]
		public static extern IntPtr ulViewLockJSContext(IntPtr view);

		[DllImport("Ultralight")]
		public static extern void ulViewUnlockJSContext(IntPtr view);

		[DllImport("Ultralight")]
		public static extern IntPtr ulViewEvaluateScript(IntPtr view, IntPtr js_string, out IntPtr exception);

		/// <summary>Check if can navigate backwards in history.</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial bool ulViewCanGoBack(IntPtr view);

		/// <summary>Check if can navigate forwards in history.</summary>
		[GeneratedDllImport("Ultralight")]
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
		public static partial bool ulViewHasFocus(IntPtr view);

		/// <summary>Whether or not the View has an input element with visible keyboard focus (indicated by a blinking caret).</summary>
		/// <remarks>
		/// You can use this to decide whether or not the View should consume
		/// keyboard input events (useful in games with mixed UI and key handling).
		/// </remarks>
		[GeneratedDllImport("Ultralight")]
		public static partial bool ulViewHasInputFocus(IntPtr view);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulViewFireKeyEvent(IntPtr view, ULKeyEvent key_event);
		// to be continued https://github.com/ultralight-ux/Ultralight-API/blob/7f9de24ca1c7ec8b385e895c4899b9d96626da58/Ultralight/CAPI.h#L744
	}

	public class View : IDisposable
	{
		public IntPtr Ptr { get; private set; }
		public bool IsDisposed { get; private set; }

		public View(Renderer renderer, uint width, uint height, bool transparent, Session session, bool force_cpu_renderer)
		{
			Ptr = Methods.ulCreateView(renderer.Ptr, width, height, transparent, session.Ptr, force_cpu_renderer);
		}

		public string URL { get => Methods.ulViewGetURL(Ptr); set => Methods.ulViewLoadURL(Ptr, value); }
		public ULString HTML { set => Methods.ulViewLoadHTML(Ptr, value.Ptr); }
		public ULString Title { get => new(Methods.ulViewGetTitle(Ptr)); }

		public uint Width => Methods.ulViewGetWidth(Ptr);
		public uint Height => Methods.ulViewGetHeight(Ptr);

		public bool IsLoading => Methods.ulViewIsLoading(Ptr);

		public RenderTarget RenderTarget => Methods.ulViewGetRenderTarget(Ptr);

		public ULSurface Surface => new(Methods.ulViewGetSurface(Ptr));

		public void Resize(uint width, uint height) => Methods.ulViewResize(Ptr, width, height);

		public IntPtr LockJSContext() => Methods.ulViewLockJSContext(Ptr);
		public void UnlockJSContext() => Methods.ulViewUnlockJSContext(Ptr);

		public ULString EvaluateScript(ULString js_string, out ULString exception)
		{
			IntPtr result_ptr = Methods.ulViewEvaluateScript(Ptr, js_string.Ptr, out IntPtr exception_ptr);
			exception = new(exception_ptr);
			return new(result_ptr);
		}

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

		~View() => Dispose();
		public void Dispose()
		{
			if (IsDisposed) return;
			Methods.ulDestroyView(Ptr);

			IsDisposed = true;
			GC.SuppressFinalize(this);
		}
	}
}
