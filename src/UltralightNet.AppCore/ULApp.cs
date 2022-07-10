using System;
using System.Runtime.InteropServices;
using System.Threading;
using UltralightNet.LowStuff;

namespace UltralightNet.AppCore;

public static unsafe partial class AppCoreMethods
{
	public const string LibAppCore = "AppCore";

	[DllImport(LibAppCore)]
	public static extern Handle<ULApp> ulCreateApp(_ULSettings* settings, _ULConfig* config);

	// INTEROPTODO: NATIVEMARSHALLING
	//[GeneratedDllImport("AppCore")]
	public static Handle<ULApp> ulCreateApp(in ULSettings settings, in ULConfig config)
	{
		using _ULSettings nativeSettings = new(settings);
		using _ULConfig nativeConfig = new(config);
		var ret = ulCreateApp(&nativeSettings, &nativeConfig);
		return ret;
	}

	[DllImport(LibAppCore)]
	public static extern void ulDestroyApp(Handle<ULApp> app);

	[DllImport(LibAppCore)]
	public static extern unsafe void ulAppSetUpdateCallback(Handle<ULApp> app, delegate* unmanaged[Cdecl]<void*, void> callback, void* user_data);

	[GeneratedDllImport(LibAppCore)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulAppIsRunning(Handle<ULApp> app);

	[DllImport(LibAppCore)]
	public static extern ULMonitor ulAppGetMainMonitor(Handle<ULApp> app);

	[DllImport(LibAppCore)]
	public static extern Handle<Renderer> ulAppGetRenderer(Handle<ULApp> app);

	[DllImport(LibAppCore)]
	public static extern void ulAppRun(Handle<ULApp> app);

	[DllImport(LibAppCore)]
	public static extern void ulAppQuit(Handle<ULApp> app);
}
public class ULApp : INativeContainer<ULApp>, INativeContainerInterface<ULApp>, IEquatable<ULApp>
{
	private GCHandle updateHandle;

	private ULApp(Handle<ULApp> handle)
	{
		Handle = handle;
		Renderer = Renderer.FromHandle(AppCoreMethods.ulAppGetRenderer(Handle), false);
		Renderer.ThreadId = Thread.CurrentThread.ManagedThreadId;
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
	public unsafe void SetUpdateCallback(delegate* unmanaged[Cdecl]<void*, void> callback, void* userData = null)
	{
		AppCoreMethods.ulAppSetUpdateCallback(Handle, callback, userData);
		GC.KeepAlive(this);
	}

	public bool IsRunning
	{
		get
		{
			var returnValue = AppCoreMethods.ulAppIsRunning(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}

	public ULMonitor MainMonitor
	{
		get
		{
			ULMonitor returnValue = AppCoreMethods.ulAppGetMainMonitor(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}

	public Renderer Renderer { get; }

	public void Run()
	{
		AppCoreMethods.ulAppRun(Handle);
		GC.KeepAlive(this);
	}
	public void Quit()
	{
		AppCoreMethods.ulAppQuit(Handle);
		GC.KeepAlive(this);
	}

	public override void Dispose()
	{
		if (updateHandle.IsAllocated) updateHandle.Free();
		updateHandle = default;

		if (!IsDisposed && Owns) AppCoreMethods.ulDestroyApp(Handle);

		base.Dispose();
	}

	public bool Equals(ULApp? other)
	{
		if (other is null) return false;
		if (IsDisposed != other.IsDisposed) return false;
		if (IsDisposed) return true;
		var returnValue = Handle == other.Handle;
		GC.KeepAlive(this);
		GC.KeepAlive(other);
		return returnValue;
	}
	public override bool Equals(object? other) => other is ULApp app ? Equals(app) : false;

	public static ULApp FromHandle(Handle<ULApp> ptr, bool dispose) => new(ptr) { Owns = dispose };

	public static ULApp Create(in ULSettings settings, in ULConfig config) => FromHandle(AppCoreMethods.ulCreateApp(settings, config), true);
}
