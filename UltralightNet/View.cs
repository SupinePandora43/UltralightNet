using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		[GeneratedDllImport("Ultralight")]
		public static partial IntPtr ulCreateView(IntPtr renderer, uint width, uint height, bool transparent, IntPtr session, bool force_cpu_renderer);

		[DllImport("Ultralight")]
		public static extern void ulDestroyView(IntPtr view);

		[DllImport("Ultralight")]
		public static extern IntPtr ulViewGetURL(IntPtr view);

		[DllImport("Ultralight")]
		public static extern IntPtr ulViewGetTitle(IntPtr view);

		[GeneratedDllImport("Ultralight")]
		public static partial uint ulViewGetWidth(IntPtr view);

		[GeneratedDllImport("Ultralight")]
		public static partial uint ulViewGetHeight(IntPtr view);

		[GeneratedDllImport("Ultralight")]
		public static partial bool ulViewIsLoading(IntPtr view);

		// to be continued https://github.com/ultralight-ux/Ultralight-API/blob/7f9de24ca1c7ec8b385e895c4899b9d96626da58/Ultralight/CAPI.h#L601
	}

	public class View : IDisposable
	{
		public IntPtr Ptr { get; private set; }
		public bool IsDisposed { get; private set; }

		public View(Renderer renderer, uint width, uint height, bool transparent, Session session, bool force_cpu_renderer)
		{
			Ptr = Methods.ulCreateView(renderer.Ptr, width, height, transparent, session.Ptr, force_cpu_renderer);
		}

		public ULString URL { get => new(Methods.ulViewGetURL(Ptr)); }
		public ULString Title { get => new(Methods.ulViewGetTitle(Ptr)); }

		public uint Width => Methods.ulViewGetWidth(Ptr);
		public uint Height => Methods.ulViewGetHeight(Ptr);

		public bool IsLoading => Methods.ulViewIsLoading(Ptr);

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
