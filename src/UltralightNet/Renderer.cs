using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using UltralightNet.Gamepad;
using UltralightNet.LowStuff;

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

	[LibraryImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static partial bool ulStartRemoteInspectorServer(Renderer renderer, byte* address, ushort port);

	[LibraryImport(LibUltralight, StringMarshalling = StringMarshalling.Utf8)]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static partial bool ulStartRemoteInspectorServer(Renderer renderer, string address, ushort port);

	[LibraryImport(LibUltralight)]
	internal static partial void ulSetGamepadDetails(Renderer renderer, uint index, [MarshalUsing(typeof(ULString))] string id, uint axisCount, uint buttonCount);

	[LibraryImport(LibUltralight)]
	internal static partial void ulFireGamepadEvent(Renderer renderer, GamepadEvent gamepadEvent);

	[LibraryImport(LibUltralight)]
	internal static partial void ulFireGamepadAxisEvent(Renderer renderer, GamepadAxisEvent gamepadAxisEvent);

	[LibraryImport(LibUltralight)]
	internal static partial void ulFireGamepadButtonEvent(Renderer renderer, GamepadButtonEvent gamepadButtonEvent);
}

[NativeMarshalling(typeof(Marshaller))]
public unsafe class Renderer : NativeContainer
{
	internal override void* Handle
	{
		get
		{
			AssertNotWrongThread();
			return base.Handle;
		}
		init => base.Handle = value;
	}

	protected Renderer() { }

	internal int ThreadId { get; set; } = -1;
	internal void AssertNotWrongThread() // hungry
	{
		if (ThreadId is not -1 or int.MaxValue && ULPlatform.ErrorWrongThread && ThreadId != Environment.CurrentManagedThreadId) throw new AggregateException("Wrong thread. (ULPlatform.ErrorWrongThread)");
	}

	public View CreateView(uint width, uint height) => CreateView(width, height, new ULViewConfig());
	public View CreateView(uint width, uint height, ULViewConfig viewConfig) => CreateView(width, height, viewConfig, DefaultSession);
	public View CreateView(uint width, uint height, ULViewConfig viewConfig, Session session) => CreateView(width, height, viewConfig, session, true);

	public View CreateView(uint width, uint height, ULViewConfig viewConfig, Session session, bool dispose)
	{
		if (Owns && ULPlatform.ErrorGPUDriverNotSet && viewConfig.IsAccelerated && (ULPlatform.GPUDriver.__UpdateCommandList is null))
		{
			throw new Exception("No ULPlatform.GPUDriver set, but ULViewConfig.IsAccelerated was set to true. (Disable check by setting ULPlatform.ErrorGPUDriverNotSet to false.)");
		}
		View view = new(Methods.ulCreateView(this, width, height, in viewConfig, session), dispose);
		GC.KeepAlive(this);
		view.Renderer = this;
		return view;
	}
	/// <summary>Create a Session to store local data in (such as cookies, local storage, application cache, indexed db, etc).</summary>
	/// <remarks>A default, persistent Session is already created for you. You only need to call this if you want to create private, in-memory session or use a separate session for each View.</remarks>
	/// <param name="is_persistent">Whether or not to store the session on disk.<br/>Persistent sessions will be written to the path set in <see cref="ULConfig.CachePath"/></param>
	/// <param name="name">A unique name for this session, this will be used to generate a unique disk path for persistent sessions.</param>
	public Session CreateSession(bool isPersistent, string name) => Session.FromHandle(Methods.ulCreateSession(this, isPersistent, name), true);
	public Session DefaultSession => Session.FromHandle(Methods.ulDefaultSession(this), true);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Update() => Methods.ulUpdate(this);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Render() => Methods.ulRender(this);
	public void PurgeMemory() => Methods.ulPurgeMemory(this);
	public void LogMemoryUsage() => Methods.ulLogMemoryUsage(this);

	public void StartRemoteInspectorServer(string address, ushort port)
	{
		bool result = Methods.ulStartRemoteInspectorServer(this, address, port);
		if(result) throw new Exception("Failed to start remote inspector server.");
	}

	public void SetGamepadDetails(uint index, string id, uint axisCount, uint buttonCount) => Methods.ulSetGamepadDetails(this, index, id, axisCount, buttonCount);
	public void FireGamepadEvent(GamepadEvent gamepadEvent) => Methods.ulFireGamepadEvent(this, gamepadEvent);
	public void FireGamepadAxisEvent(GamepadAxisEvent gamepadAxisEvent) => Methods.ulFireGamepadAxisEvent(this, gamepadAxisEvent);
	public void FireGamepadButtonEvent(GamepadButtonEvent gamepadbuttonEvent) => Methods.ulFireGamepadButtonEvent(this, gamepadbuttonEvent);

	[SuppressMessage("Usage", "CA1816: Call GC.SupressFinalize correctly")]
	public override void Dispose()
	{
		if (!IsDisposed && Owns) Methods.ulDestroyRenderer(this);
		base.Dispose();
	}

	internal static Renderer FromHandle(void* handle, bool dispose) => new() { Handle = handle, Owns = dispose };

	public bool Equals(Renderer? other)
	{
		if (other is null) return false;
		if (other.IsDisposed) return IsDisposed;
		return Handle == other.Handle;
	}
	public override bool Equals(object? other) => other is Renderer renderer && Equals(renderer);

	[CustomMarshaller(typeof(Renderer), MarshalMode.ManagedToUnmanagedIn, typeof(Marshaller))]
	internal ref struct Marshaller
	{
		private Renderer renderer;

		public void FromManaged(Renderer renderer) => this.renderer = renderer;
		public readonly void* ToUnmanaged() => renderer.Handle;
		public readonly void Free() => GC.KeepAlive(renderer);
	}
}
