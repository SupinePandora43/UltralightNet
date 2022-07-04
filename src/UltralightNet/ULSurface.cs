using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet;

public static partial class Methods
{
	/// <summary>Width (in pixels).</summary>
	[DllImport("Ultralight")]
	public static extern uint ulSurfaceGetWidth(IntPtr surface);

	/// <summary>Height (in pixels).</summary>
	[DllImport("Ultralight")]
	public static extern uint ulSurfaceGetHeight(IntPtr surface);

	/// <summary>Number of bytes between rows (usually width * 4)</summary>
	[DllImport("Ultralight")]
	public static extern uint ulSurfaceGetRowBytes(IntPtr surface);

	/// <summary>Size in bytes.</summary>
	[DllImport("Ultralight")]
	public static extern nuint ulSurfaceGetSize(IntPtr surface);

	/// <summary>
	/// Lock the pixel buffer and get a pointer to the beginning of the data
	/// for reading/writing.
	/// <br/>
	/// Native pixel format is premultiplied BGRA 32-bit (8 bits per channel).
	/// </summary>
	[DllImport("Ultralight")]
	public static extern IntPtr ulSurfaceLockPixels(IntPtr surface);

	/// <summary>Unlock the pixel buffer.</summary>
	[DllImport("Ultralight")]
	public static extern void ulSurfaceUnlockPixels(IntPtr surface);

	/// <summary>Resize the pixel buffer to a certain width and height (both in pixels).</summary>
	[DllImport("Ultralight")]
	public static extern void ulSurfaceResize(IntPtr surface, uint width, uint height);

	/// <summary>Set the dirty bounds to a certain value.</summary>
	[DllImport("Ultralight")]
	public static extern void ulSurfaceSetDirtyBounds(IntPtr surface, ULIntRect bounds);

	/// <summary>Get the dirty bounds.</summary>
	// todo: example code
	[DllImport("Ultralight")]
	public static extern ULIntRect ulSurfaceGetDirtyBounds(IntPtr surface);

	/// <summary>Clear the dirty bounds.</summary>
	[DllImport("Ultralight")]
	public static extern void ulSurfaceClearDirtyBounds(IntPtr surface);

	/// <summary>Get the underlying user data pointer (this is only valid if you have set a custom surface implementation via ulPlatformSetSurfaceDefinition).</summary>
	[DllImport("Ultralight")]
	public static extern IntPtr ulSurfaceGetUserData(IntPtr surface);

	/// <summary>Get the underlying Bitmap from the default Surface.</summary>
	[DllImport("Ultralight")]
	public static extern IntPtr ulBitmapSurfaceGetBitmap(IntPtr surface);
}

public class ULSurface
{
	public readonly IntPtr Ptr;
	public ULSurface(IntPtr ptr) => Ptr = ptr;

	public uint Width => Methods.ulSurfaceGetWidth(Ptr);
	public uint Height => Methods.ulSurfaceGetHeight(Ptr);

	public uint RowBytes => Methods.ulSurfaceGetRowBytes(Ptr);
	public nuint Size => Methods.ulSurfaceGetSize(Ptr);

	public IntPtr LockPixels() => Methods.ulSurfaceLockPixels(Ptr);
	public void UnlockPixels() => Methods.ulSurfaceUnlockPixels(Ptr);

	public void Resize(uint width, uint height) => Methods.ulSurfaceResize(Ptr, width, height);

	public ULIntRect DirtyBounds
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Methods.ulSurfaceGetDirtyBounds(Ptr);
		set => Methods.ulSurfaceSetDirtyBounds(Ptr, value);
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ClearDirtyBounds() => Methods.ulSurfaceClearDirtyBounds(Ptr);

	public IntPtr UserData => Methods.ulSurfaceGetUserData(Ptr);

	public ULBitmap Bitmap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new(Methods.ulBitmapSurfaceGetBitmap(Ptr));
	}
}
