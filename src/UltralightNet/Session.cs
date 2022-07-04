using System;
using System.Runtime.InteropServices;
using UltralightNet.LowStuff;

namespace UltralightNet;

public static partial class Methods
{
	/// <summary>Create a Session to store local data in (such as cookies, local storage, application cache, indexed db, etc).</summary>
	[GeneratedDllImport(LibUltralight)]
	public static partial Handle<Session> ulCreateSession(Handle<Renderer> renderer, [MarshalAs(UnmanagedType.I1)] bool is_persistent, [MarshalUsing(typeof(ULString.ToNative))] string name);

	/// <summary>Destroy a Session.</summary>
	[DllImport(LibUltralight)]
	public static extern void ulDestroySession(Handle<Session> session);

	/// <summary>Get the default session (persistent session named "default").</summary>
	/// <remarks>This session is owned by the Renderer, you shouldn't destroy it.</remarks>
	[DllImport(LibUltralight)]
	public static extern Handle<Session> ulDefaultSession(Handle<Renderer> renderer);

	/// <summary>Whether or not is persistent (backed to disk).</summary>
	[GeneratedDllImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulSessionIsPersistent(Handle<Session> session);

	/// <summary>Unique name identifying the session (used for unique disk path).</summary>
	[GeneratedDllImport(LibUltralight)]
	[return: MarshalUsing(typeof(ULString.ToManaged_))]
	public static partial string ulSessionGetName(Handle<Session> session);

	/// <summary>Unique numeric Id for the session.</summary>
	[DllImport(LibUltralight)]
	public static extern ulong ulSessionGetId(Handle<Session> session);

	/// <summary>The disk path to write to (used by persistent sessions only).</summary>
	[GeneratedDllImport(LibUltralight)]
	[return: MarshalUsing(typeof(ULString.ToManaged_))]
	public static partial string ulSessionGetDiskPath(Handle<Session> session);
}

///<summary>Stores local data such as cookies, local storage, and application cache for one or more Views. </summary>
public class Session : INativeContainer<Session>, INativeContainerInterface<Session>, IEquatable<Session>
{
	private Session() { }

	/// <summary>Whether or not this session is written to disk.</summary>
	public bool IsPersistent => Methods.ulSessionIsPersistent(Handle);
	/// <summary>A unique name identifying this session.</summary>
	public string Name => Methods.ulSessionGetName(Handle);
	/// <summary>A unique numeric ID identifying this session.</summary>
	public ulong Id => Methods.ulSessionGetId(Handle);
	/// <summary>The disk path of this session (only valid for persistent sessions).</summary>
	public string DiskPath => Methods.ulSessionGetDiskPath(Handle);

	public override void Dispose()
	{
		if (IsDisposed) return;
		if (Owns) Methods.ulDestroySession(Handle);

		IsDisposed = true;
		GC.SuppressFinalize(this);
	}

	public bool Equals(Session? other)
	{
		if (other is null) return IsDisposed;
		if (Handle == other.Handle) return true;
		if (IsDisposed != other.IsDisposed) return false;
		return IsPersistent == other.IsPersistent && Name == other.Name && Id == other.Id && DiskPath == other.DiskPath;
	}

	public static unsafe Session FromHandle(Handle<Session> handle, bool dispose) => new() { Handle = handle, Owns = dispose };
}
