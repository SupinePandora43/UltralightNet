using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using UltralightNet.LowStuff;

namespace UltralightNet;

public static unsafe partial class Methods
{
	/// <summary>Create a Session to store local data in (such as cookies, local storage, application cache, indexed db, etc).</summary>
	[LibraryImport(LibUltralight)]
	internal static partial void* ulCreateSession(Renderer renderer, [MarshalAs(UnmanagedType.U1)] bool is_persistent, [MarshalUsing(typeof(ULString))] string name);

	/// <summary>Destroy a Session.</summary>
	[LibraryImport(LibUltralight)]
	internal static partial void ulDestroySession(Session session);

	/// <summary>Get the default session (persistent session named "default").</summary>
	/// <remarks>This session is owned by the Renderer, you shouldn't destroy it.</remarks>
	[LibraryImport(LibUltralight)]
	internal static partial void* ulDefaultSession(Renderer renderer);

	/// <summary>Whether or not is persistent (backed to disk).</summary>
	[LibraryImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static partial bool ulSessionIsPersistent(Session session);

	/// <summary>Unique name identifying the session (used for unique disk path).</summary>
	[LibraryImport(LibUltralight)]
	[return: MarshalUsing(typeof(ULString))]
	internal static partial string ulSessionGetName(Session session);

	/// <summary>Unique numeric Id for the session.</summary>
	[LibraryImport(LibUltralight)]
	internal static partial ulong ulSessionGetId(Session session);

	/// <summary>The disk path to write to (used by persistent sessions only).</summary>
	[LibraryImport(LibUltralight)]
	[return: MarshalUsing(typeof(ULString))]
	internal static partial string ulSessionGetDiskPath(Session session);
}

///<summary>Stores local data such as cookies, local storage, and application cache for one or more Views. </summary>
[NativeMarshalling(typeof(Marshaller))]
public sealed class Session : NativeContainer
{
	private Session() { }

	/// <summary>Whether or not this session is written to disk.</summary>
	public bool IsPersistent => Methods.ulSessionIsPersistent(this);
	/// <summary>A unique name identifying this session.</summary>
	public string Name => Methods.ulSessionGetName(this);
	/// <summary>A unique numeric ID identifying this session.</summary>
	public ulong Id => Methods.ulSessionGetId(this);
	/// <summary>The disk path of this session (only valid for persistent sessions).</summary>
	public string DiskPath => Methods.ulSessionGetDiskPath(this);

	public override void Dispose()
	{
		if (!IsDisposed && Owns) Methods.ulDestroySession(this);
		base.Dispose();
	}

	internal static unsafe Session FromHandle(void* handle, bool dispose) => new() { Handle = handle, Owns = dispose };

	[CustomMarshaller(typeof(Session), MarshalMode.ManagedToUnmanagedIn, typeof(Marshaller))]
	internal ref struct Marshaller
	{
		private Session session;

		public void FromManaged(Session session) => this.session = session;
		public readonly unsafe void* ToUnmanaged() => session.Handle;
		public readonly void Free() => GC.KeepAlive(session);
	}
}
