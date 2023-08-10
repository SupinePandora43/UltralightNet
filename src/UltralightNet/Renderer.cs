using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using UltralightNet.Gamepad;
using UltralightNet.LowStuff;
using UltralightNet.Platform;

namespace UltralightNet;

public static unsafe partial class Methods
{
	[LibraryImport(LibUltralight)]
	internal static partial void* ulCreateRenderer(in ULConfig config);

	/// <summary>Destroy the renderer.</summary>
	[LibraryImport(LibUltralight)]
	internal static partial void ulDestroyRenderer(Renderer renderer);

	/// <summary>Update timers and dispatch internal callbacks (JavaScript and network).</summary>
	[LibraryImport(LibUltralight)]
	internal static partial void ulUpdate(Renderer renderer);

	/// <summary>Render all active Views.</summary>
	[LibraryImport(LibUltralight)]
	internal static partial void ulRender(Renderer renderer);

	/// <summary>Attempt to release as much memory as possible. Don't call this from any callbacks or driver code.</summary>
	[LibraryImport(LibUltralight)]
	internal static partial void ulPurgeMemory(Renderer renderer);

	[LibraryImport(LibUltralight)]
	internal static partial void ulLogMemoryUsage(Renderer renderer);

	[LibraryImport(LibUltralight, StringMarshalling = StringMarshalling.Utf8)]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static partial bool ulStartRemoteInspectorServer(Renderer renderer, string address, ushort port);

	[LibraryImport(LibUltralight)]
	internal static partial void ulSetGamepadDetails(Renderer renderer, uint index, [MarshalUsing(typeof(ULString))] string id, uint axisCount, uint buttonCount);

	[LibraryImport(LibUltralight)]
	internal static partial void ulFireGamepadEvent(Renderer renderer, GamepadEvent* gamepadEvent);

	[LibraryImport(LibUltralight)]
	internal static partial void ulFireGamepadAxisEvent(Renderer renderer, GamepadAxisEvent* gamepadAxisEvent);

	[LibraryImport(LibUltralight)]
	internal static partial void ulFireGamepadButtonEvent(Renderer renderer, GamepadButtonEvent* gamepadButtonEvent);
}

[NativeMarshalling(typeof(Marshaller))]
public sealed unsafe class Renderer : NativeContainer
{
	protected override void* Handle
	{
		get
		{
			AssertNotWrongThread();
			return base.Handle;
		}
		init
		{
			renderers[(nuint)value] = new WeakReference<Renderer>(this);
			base.Handle = value;
		}
	}

	internal int ThreadId { get; set; } = -1;
	internal void AssertNotWrongThread() // hungry
	{
		if (ThreadId is not -1 or int.MaxValue && ULPlatform.ErrorWrongThread && ThreadId != Environment.CurrentManagedThreadId) throw new AggregateException("Wrong thread. (ULPlatform.ErrorWrongThread)");
	}

	public View CreateView(uint width, uint height, ULViewConfig? viewConfig = null, Session? session = null, bool dispose = true)
	{
		viewConfig ??= new();
		if (Owns && ULPlatform.ErrorGPUDriverNotSet && viewConfig.Value.IsAccelerated && (gpuDriverWrapper?.IsDisposed).GetValueOrDefault(true))
		{
			throw new Exception("No ULPlatform.GPUDriver set, but ULViewConfig.IsAccelerated was set to true. (Disable check by setting ULPlatform.ErrorGPUDriverNotSet to false.)");
		}
		void* viewHandle;
		var view = View.FromHandle(viewHandle = Methods.ulCreateView(this, width, height, viewConfig.Value, session ?? DefaultSession), dispose);
		view.Renderer = this;
		views[(nuint)viewHandle] = new WeakReference<View>(view);
		view.SetUpCallbacks();
		return view;
	}
	/// <summary>Create a Session to store local data in (such as cookies, local storage, application cache, indexed db, etc).</summary>
	/// <remarks>A default, persistent Session is already created for you. You only need to call this if you want to create private, in-memory session or use a separate session for each View.</remarks>
	/// <param name="is_persistent">Whether or not to store the session on disk.<br/>Persistent sessions will be written to the path set in <see cref="ULConfig.CachePath"/></param>
	/// <param name="name">A unique name for this session, this will be used to generate a unique disk path for persistent sessions.</param>
	public Session CreateSession(bool isPersistent, string name) => Session.FromHandle(Methods.ulCreateSession(this, isPersistent, name), true);
	public Session DefaultSession => Session.FromHandle(Methods.ulDefaultSession(this), false);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Update() => Methods.ulUpdate(this);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Render() => Methods.ulRender(this);
	public void PurgeMemory() => Methods.ulPurgeMemory(this);
	public void LogMemoryUsage() => Methods.ulLogMemoryUsage(this);

	public bool TryStartRemoteInspectorServer(string address, ushort port) => Methods.ulStartRemoteInspectorServer(this, address, port);

	public void SetGamepadDetails(uint index, string id, uint axisCount, uint buttonCount) => Methods.ulSetGamepadDetails(this, index, id, axisCount, buttonCount);
	public void FireGamepadEvent(GamepadEvent gamepadEvent) => Methods.ulFireGamepadEvent(this, &gamepadEvent);
	public void FireGamepadAxisEvent(GamepadAxisEvent gamepadAxisEvent) => Methods.ulFireGamepadAxisEvent(this, &gamepadAxisEvent);
	public void FireGamepadButtonEvent(GamepadButtonEvent gamepadbuttonEvent) => Methods.ulFireGamepadButtonEvent(this, &gamepadbuttonEvent);

	public override void Dispose()
	{
		if (!IsDisposed && Owns) Methods.ulDestroyRenderer(this);
		GC.KeepAlive(loggerWrapper);
		GC.KeepAlive(filesystemWrapper);
		GC.KeepAlive(fontloaderWrapper);
		GC.KeepAlive(gpuDriverWrapper);
		GC.KeepAlive(surfaceDefinitionWrapper);
		GC.KeepAlive(clipboardWrapper);
		base.Dispose();
	}

	internal static Dictionary<nuint, WeakReference<Renderer>> renderers = new(1);
	internal Dictionary<nuint, WeakReference<View>> views = new(1);

	// Soul keepers
	internal ILogger.Wrapper? loggerWrapper;
	internal IFileSystem.Wrapper? filesystemWrapper;
	internal IFontLoader.Wrapper? fontloaderWrapper;
	internal IGPUDriver.Wrapper? gpuDriverWrapper;
	internal ISurfaceDefinition.Wrapper? surfaceDefinitionWrapper;
	internal IClipboard.Wrapper? clipboardWrapper;

	internal static Renderer FromHandle(void* handle, bool dispose) => new() { Handle = handle, Owns = dispose };
	internal nuint GetCallbackData() => (nuint)Handle;

	[CustomMarshaller(typeof(Renderer), MarshalMode.ManagedToUnmanagedIn, typeof(Marshaller))]
	internal ref struct Marshaller
	{
		private Renderer renderer;

		public void FromManaged(Renderer renderer) => this.renderer = renderer;
		public readonly void* ToUnmanaged() => renderer.Handle;
		public readonly void Free() => GC.KeepAlive(renderer);
	}
}
