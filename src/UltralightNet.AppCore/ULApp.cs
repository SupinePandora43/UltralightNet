using System;
using System.Runtime.InteropServices;
using System.Threading;
using UltralightNet.LowStuff;

namespace UltralightNet.AppCore;

public static unsafe partial class AppCoreMethods
{
	public const string LibAppCore = "AppCore";

	[DllImport(LibAppCore)]
	public static extern void* ulCreateApp(_ULSettings* settings, _ULConfig* config);

	// INTEROPTODO: NATIVEMARSHALLING
	//[GeneratedDllImport("AppCore")]
	public static IntPtr ulCreateApp(in ULSettings settings, in ULConfig config)
	{
		using _ULSettings nativeSettings = new(settings);
		using _ULConfig nativeConfig = new(config);
		var ret = ulCreateApp(&nativeSettings, &nativeConfig);
		return (IntPtr)ret;
	}

	[DllImport(LibAppCore)]
	public static extern void ulDestroyApp(IntPtr app);

	[DllImport(LibAppCore)]
	public static extern unsafe void ulAppSetUpdateCallback(IntPtr app, delegate* unmanaged[Cdecl]<void*, void> callback, void* user_data);

	[GeneratedDllImport(LibAppCore)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulAppIsRunning(IntPtr app);

	[DllImport(LibAppCore)]
	public static extern IntPtr ulAppGetMainMonitor(IntPtr app);

	[DllImport(LibAppCore)]
	public static extern Handle<Renderer> ulAppGetRenderer(IntPtr app);

	[DllImport(LibAppCore)]
	public static extern void ulAppRun(IntPtr app);

	[DllImport(LibAppCore)]
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
		Ptr = AppCoreMethods.ulCreateApp(settings, config);
		ULPlatform.thread = Thread.CurrentThread;
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

	public Renderer Renderer => Renderer.FromHandle(AppCoreMethods.ulAppGetRenderer(Ptr), false);

	public void Run() => AppCoreMethods.ulAppRun(Ptr);
	public void Quit() => AppCoreMethods.ulAppQuit(Ptr);

	~ULApp() => Dispose();

	public void Dispose()
	{
		if (updateHandle.IsAllocated) updateHandle.Free();
		updateHandle = default;

		if (IsDisposed) return;
		AppCoreMethods.ulDestroyApp(Ptr);

		IsDisposed = true;
		GC.SuppressFinalize(this);
	}
}
