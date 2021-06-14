using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore
{
	public static partial class AppCoreMethods
	{
		[DllImport("AppCore")]
		public static extern IntPtr ulCreateApp(IntPtr settings, IntPtr config);

		[DllImport("AppCore")]
		public static extern void ulDestroyApp(IntPtr app);

		[DllImport("AppCore")]
		public static extern void ulAppSetUpdateCallback(IntPtr app, ULUpdateCallback callback, IntPtr user_data);
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

		public ULApp(ULSettings settings, ULConfig config)
		{
			Ptr = AppCoreMethods.ulCreateApp(settings.Ptr, config.Ptr);
		}

		public void SetUpdateCallback(ULUpdateCallback callback, IntPtr userData = default)
		{
			if (callback is not null)
			{
				if (updateHandle.IsAllocated) updateHandle.Free();
				updateHandle = GCHandle.Alloc(callback, GCHandleType.Normal);
				AppCoreMethods.ulAppSetUpdateCallback(Ptr, callback, userData);
			}
			else
			{
				if (updateHandle.IsAllocated) updateHandle.Free();
				AppCoreMethods.ulAppSetUpdateCallback(Ptr, null, userData);
			}
		}
		public unsafe void SetUpdateCallback(delegate* unmanaged[Cdecl]<void*, void> callback, void* userData = null){
			AppCoreMethods.ulAppSetUpdateCallback(Ptr, callback, userData);
		}

		public bool IsRunning => AppCoreMethods.ulAppIsRunning(Ptr);

		public ULMonitor MainMonitor => new(AppCoreMethods.ulAppGetMainMonitor(Ptr));

		public Renderer Renderer => new(AppCoreMethods.ulAppGetRenderer(Ptr));

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
