using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using UltralightNet.LowStuff;

namespace UltralightNet.AppCore;

public static unsafe partial class AppCoreMethods
{
	public const string LibAppCore = "AppCore";

	[LibraryImport(LibAppCore)]
	public static unsafe partial void* ulCreateApp(in ULSettings settings, in ULConfig config);

	[LibraryImport(LibAppCore)]
	public static partial void ulDestroyApp(ULApp app);

	[LibraryImport(LibAppCore)]
	public static unsafe partial void ulAppSetUpdateCallback(ULApp app, delegate* unmanaged[Cdecl]<void*, void> callback, void* user_data);

	[LibraryImport(LibAppCore)]
	[return: MarshalAs(UnmanagedType.U1)]
	public static partial bool ulAppIsRunning(ULApp app);

	[LibraryImport(LibAppCore)]
	public static partial ULMonitor ulAppGetMainMonitor(ULApp app);

	[LibraryImport(LibAppCore)]
	public static unsafe partial void* ulAppGetRenderer(ULApp app);

	[LibraryImport(LibAppCore)]
	public static partial void ulAppRun(ULApp app);

	[LibraryImport(LibAppCore)]
	public static partial void ulAppQuit(ULApp app);
}

[NativeMarshalling(typeof(Marshaller))]
public class ULApp : NativeContainer
{
	private GCHandle updateHandle;

	private unsafe ULApp(void* handle)
	{
		Handle = handle;
		Renderer = Renderer.FromHandle(AppCoreMethods.ulAppGetRenderer(this), false);
		Renderer.ThreadId = Environment.CurrentManagedThreadId;
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
	public unsafe void SetUpdateCallback(delegate* unmanaged[Cdecl]<void*, void> callback, void* userData = null) => AppCoreMethods.ulAppSetUpdateCallback(this, callback, userData);

	public bool IsRunning => AppCoreMethods.ulAppIsRunning(this);


	public ULMonitor MainMonitor => AppCoreMethods.ulAppGetMainMonitor(this);

	public Renderer Renderer { get; }

	public void Run() => AppCoreMethods.ulAppRun(this);
	public void Quit() => AppCoreMethods.ulAppQuit(this);

	public override void Dispose()
	{
		if (updateHandle.IsAllocated) updateHandle.Free();
		// updateHandle = default; // INTEROPTODO: Do we even need it?

		if (!IsDisposed && Owns) AppCoreMethods.ulDestroyApp(this);

		base.Dispose();
	}

	public static unsafe ULApp FromHandle(void* ptr, bool dispose) => new(ptr) { Owns = dispose };

	public static unsafe ULApp Create(in ULSettings settings, in ULConfig config) => FromHandle(AppCoreMethods.ulCreateApp(settings, config), true);

	[CustomMarshaller(typeof(ULApp), MarshalMode.ManagedToUnmanagedIn, typeof(Marshaller))]
	internal ref struct Marshaller
	{
		private ULApp app;

		public void FromManaged(ULApp app) => this.app = app;
		public readonly unsafe void* ToUnmanaged() => app.Handle;
		public readonly void Free() => GC.KeepAlive(app);
	}
}
