using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore
{
	public static unsafe partial class AppCoreMethods
	{
		[DllImport("AppCore")]
		public static extern void* ulCreateApp(void* settings, _ULConfig* config);

		// INTEROPTODO: NATIVEMARSHALLING
		//[GeneratedDllImport("AppCore")]
		public static IntPtr ulCreateApp(IntPtr settings, in ULConfig config)
		{
			_ULConfig nativeConfig = new(config);
			var ret = ulCreateApp((void*)settings, &nativeConfig);
			nativeConfig.FreeNative();
			return (IntPtr)ret;
		}

		[DllImport("AppCore")]
		public static extern void ulDestroyApp(IntPtr app);

		[DllImport("AppCore")]
		public static extern unsafe void ulAppSetUpdateCallback(IntPtr app, delegate* unmanaged[Cdecl]<void*, void> callback, void* user_data);

		[GeneratedDllImport("AppCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulAppIsRunning(IntPtr app);

		[DllImport("AppCore")]
		public static extern IntPtr ulAppGetMainMonitor(IntPtr app);

		[DllImport("AppCore")]
		public static extern IntPtr ulAppGetRenderer(IntPtr app);

		[DllImport("AppCore")]
		public static extern void ulAppRun(IntPtr app);

		[DllImport("AppCore")]
		public static extern void ulAppQuit(IntPtr app);
	}
	public class ULApp : IDisposable
	{
		public IntPtr Ptr { get; private set; }
		public bool IsDisposed { get; private set; }

		private GCHandle updateHandle;

		public ULApp(IntPtr ptr, bool dispose = false)
		{
			Ptr = ptr;
			IsDisposed = !dispose;
		}

		public ULApp(ULSettings settings, ULConfig config = default)
		{
			Ptr = AppCoreMethods.ulCreateApp(settings.Ptr, config);
		}

		public unsafe void SetUpdateCallback(ULUpdateCallback callback, IntPtr userData = default)
		{
			if (updateHandle.IsAllocated) updateHandle.Free();
			if (callback is not null)
			{
				updateHandle = GCHandle.Alloc(callback, GCHandleType.Normal);
				SetUpdateCallback((delegate* unmanaged[Cdecl]<void*, void>)Marshal.GetFunctionPointerForDelegate(callback), (void*)userData);
			}
			else
			{
				SetUpdateCallback((delegate* unmanaged[Cdecl]<void*, void>)null, (void*)userData);
			}
		}
		public unsafe void SetUpdateCallback(delegate* unmanaged[Cdecl]<void*, void> callback, void* userData = null) => AppCoreMethods.ulAppSetUpdateCallback(Ptr, callback, userData);

		public bool IsRunning => AppCoreMethods.ulAppIsRunning(Ptr);

		public ULMonitor MainMonitor => new(AppCoreMethods.ulAppGetMainMonitor(Ptr));

		public Renderer Renderer => Renderer.FromIntPtr(AppCoreMethods.ulAppGetRenderer(Ptr));

		public void Run() => AppCoreMethods.ulAppRun(Ptr);
		public void Quit() => AppCoreMethods.ulAppQuit(Ptr);

		~ULApp() => Dispose();

		public void Dispose()
		{
			if (updateHandle.IsAllocated) updateHandle.Free();

			if (IsDisposed) return;
			AppCoreMethods.ulDestroyApp(Ptr);

			IsDisposed = true;
			GC.SuppressFinalize(this);
		}
	}
}
