using System;
using System.Runtime.InteropServices;

namespace UltralightNet;

public static partial class Methods
{
	/// <summary>Create a Session to store local data in (such as cookies, local storage, application cache, indexed db, etc).</summary>
	[GeneratedDllImport(LibUltralight)]
	public static partial IntPtr ulCreateSession(IntPtr renderer, [MarshalAs(UnmanagedType.I1)] bool is_persistent, [MarshalUsing(typeof(ULString.ToNative))] string name);

	/// <summary>Destroy a Session.</summary>
	[DllImport(LibUltralight)]
	public static extern void ulDestroySession(IntPtr session);

	/// <summary>Get the default session (persistent session named "default").</summary>
	/// <remarks>This session is owned by the Renderer, you shouldn't destroy it.</remarks>
	[DllImport(LibUltralight)]
	public static extern IntPtr ulDefaultSession(IntPtr renderer);

	/// <summary>Whether or not is persistent (backed to disk).</summary>
	[GeneratedDllImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulSessionIsPersistent(IntPtr session);

	/// <summary>Unique name identifying the session (used for unique disk path).</summary>
	[GeneratedDllImport(LibUltralight)]
	[return: MarshalUsing(typeof(ULString.ToManaged_))]
	public static partial string ulSessionGetName(IntPtr session);

	/// <summary>Unique numeric Id for the session.</summary>
	[DllImport(LibUltralight)]
	public static extern ulong ulSessionGetId(IntPtr session);

	/// <summary>The disk path to write to (used by persistent sessions only).</summary>
	[GeneratedDllImport(LibUltralight)]
	[return: MarshalUsing(typeof(ULString.ToManaged_))]
	public static partial string ulSessionGetDiskPath(IntPtr session);
}

///<summary>Stores local data such as cookies, local storage, and application cache for one or more Views. </summary>
public class Session : IDisposable
{
	public readonly IntPtr Ptr;
	public bool IsDisposed { get; private set; }

	public Session(IntPtr ptr, bool dispose = false)
	{
		Ptr = ptr;
		IsDisposed = !dispose;
	}
	/// <summary>Create a Session to store local data in (such as cookies, local storage, application cache, indexed db, etc).</summary>
	/// <remarks>A default, persistent Session is already created for you. You only need to call this if you want to create private, in-memory session or use a separate session for each View.</remarks>
	/// <param name="is_persistent">Whether or not to store the session on disk.<br/>Persistent sessions will be written to the path set in <see cref="ULConfig.CachePath"/></param>
	/// <param name="name">A unique name for this session, this will be used to generate a unique disk path for persistent sessions.</param>
	internal Session(Renderer renderer, bool is_persistent, string name) => Ptr = Methods.ulCreateSession(renderer.Ptr, is_persistent, name);

	/// <summary>Whether or not this session is written to disk.</summary>
	public bool IsPersistent => Methods.ulSessionIsPersistent(Ptr);
	/// <summary>A unique name identifying this session.</summary>
	public string Name => Methods.ulSessionGetName(Ptr);
	/// <summary>A unique numeric ID identifying this session.</summary>
	public ulong Id => Methods.ulSessionGetId(Ptr);
	/// <summary>The disk path of this session (only valid for persistent sessions).</summary>
	public string DiskPath => Methods.ulSessionGetDiskPath(Ptr);

	~Session() => Dispose();
	public void Dispose()
	{
		if (IsDisposed) return;
		Methods.ulDestroySession(Ptr);

		IsDisposed = true;
		GC.SuppressFinalize(this);
	}
}
