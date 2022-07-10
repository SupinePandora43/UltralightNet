using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UltralightNet.LowStuff;

namespace UltralightNet;

public static unsafe partial class Methods
{
	/// <summary>Create empty bitmap.</summary>
	[DllImport(LibUltralight)]
	public static extern Handle<ULBitmap> ulCreateEmptyBitmap();

	/// <summary>Create bitmap with certain dimensions and pixel format.</summary>
	[DllImport(LibUltralight)]
	public static extern Handle<ULBitmap> ulCreateBitmap(uint width, uint height, ULBitmapFormat format);

	/// <summary>Create bitmap from existing pixel buffer. @see Bitmap for help using this function.</summary>
	[GeneratedDllImport(LibUltralight)]
	public static unsafe partial Handle<ULBitmap> ulCreateBitmapFromPixels(uint width, uint height, ULBitmapFormat format, uint rowBytes, byte* pixels, nuint size, [MarshalAs(UnmanagedType.I1)] bool shouldCopy);

	/// <summary>Create bitmap from copy.</summary>
	[DllImport(LibUltralight)]
	public static extern Handle<ULBitmap> ulCreateBitmapFromCopy(Handle<ULBitmap> existingBitmap);

	/// <summary>Destroy a bitmap (you should only destroy Bitmaps you have explicitly created via one of the creation functions above.</summary>
	[DllImport(LibUltralight)]
	public static extern void ulDestroyBitmap(Handle<ULBitmap> bitmap);

	/// <summary>Get the width in pixels.</summary>
	[DllImport(LibUltralight)]
	public static extern uint ulBitmapGetWidth(Handle<ULBitmap> bitmap);

	/// <summary>Get the height in pixels.</summary>
	[DllImport(LibUltralight)]
	public static extern uint ulBitmapGetHeight(Handle<ULBitmap> bitmap);

	/// <summary>Get the pixel format.</summary>
	[DllImport(LibUltralight)]
	public static extern ULBitmapFormat ulBitmapGetFormat(Handle<ULBitmap> bitmap);

	/// <summary>Get the bytes per pixel.</summary>
	[DllImport(LibUltralight)]
	public static extern uint ulBitmapGetBpp(Handle<ULBitmap> bitmap);

	/// <summary>Get the number of bytes per row.</summary>
	[DllImport(LibUltralight)]
	public static extern uint ulBitmapGetRowBytes(Handle<ULBitmap> bitmap);

	/// <summary>
	/// Get the size in bytes of the underlying pixel buffer.
	/// </summary>
	[DllImport(LibUltralight)]
	public static extern nuint ulBitmapGetSize(Handle<ULBitmap> bitmap);

	/// <summary>Whether or not this bitmap owns its own pixel buffer.</summary>
	[GeneratedDllImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulBitmapOwnsPixels(Handle<ULBitmap> bitmap);

	/// <summary>Lock pixels for reading/writing, returns pointer to pixel buffer.</summary>
	[DllImport(LibUltralight)]
	public static extern byte* ulBitmapLockPixels(Handle<ULBitmap> bitmap);

	/// <summary>Unlock pixels after locking.</summary>
	[DllImport(LibUltralight)]
	public static extern void ulBitmapUnlockPixels(Handle<ULBitmap> bitmap);

	/// <summary>Get raw pixel buffer</summary>
	/// <remarks>you should only call this if Bitmap is already locked.</remarks>
	[DllImport(LibUltralight)]
	public static extern byte* ulBitmapRawPixels(Handle<ULBitmap> bitmap);

	/// <summary>Whether or not this bitmap is empty.</summary>
	[GeneratedDllImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulBitmapIsEmpty(Handle<ULBitmap> bitmap);

	/// <summary>Reset bitmap pixels to 0.</summary>
	[DllImport(LibUltralight)]
	public static extern void ulBitmapErase(Handle<ULBitmap> bitmap);

	/// <summary>Write bitmap to a PNG on disk.</summary>
	[GeneratedDllImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulBitmapWritePNG(Handle<ULBitmap> bitmap, [MarshalUsing(typeof(UTF8Marshaller))] string path);

	/// <summary>This converts a BGRA bitmap to RGBA bitmap and vice-versa by swapping the red and blue channels.</summary>
	[DllImport(LibUltralight)]
	public static extern void ulBitmapSwapRedBlueChannels(Handle<ULBitmap> bitmap);
}

#pragma warning disable CS0659
public unsafe class ULBitmap : INativeContainer<ULBitmap>, INativeContainerInterface<ULBitmap>, ICloneable, IEquatable<ULBitmap>
#pragma warning restore CS0659
{
	private ULBitmap() { }

	public ULBitmap(uint width, uint height, ULBitmapFormat format) => Handle = Methods.ulCreateBitmap(width, height, format);
	public ULBitmap(uint width, uint height, ULBitmapFormat format, uint rowBytes, byte* pixels, uint size, bool shouldCopy) => Handle = Methods.ulCreateBitmapFromPixels(width, height, format, rowBytes, pixels, size, shouldCopy);

	public uint Width
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var returnValue = Methods.ulBitmapGetWidth(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}
	public uint Height
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var returnValue = Methods.ulBitmapGetHeight(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}

	public ULBitmapFormat Format
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var returnValue = Methods.ulBitmapGetFormat(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}
	public uint Bpp
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var returnValue = Methods.ulBitmapGetBpp(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}
	public uint RowBytes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var returnValue = Methods.ulBitmapGetRowBytes(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}
	public nuint Size
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var returnValue = Methods.ulBitmapGetSize(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}

	public bool OwnsPixels
	{
		get
		{
			var returnValue = Methods.ulBitmapOwnsPixels(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte* LockPixels()
	{
		var returnValue = Methods.ulBitmapLockPixels(Handle);
		GC.KeepAlive(this);
		return returnValue;
	}
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void UnlockPixels() => Methods.ulBitmapUnlockPixels(Handle);

	public byte* RawPixels
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var returnValue = Methods.ulBitmapRawPixels(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}
	public bool IsEmpty
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			var returnValue = Methods.ulBitmapIsEmpty(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}
	public void Erase()
	{
		Methods.ulBitmapErase(Handle);
		GC.KeepAlive(this);
	}

	public bool WritePng(string path)
	{
		var returnValue = Methods.ulBitmapWritePNG(Handle, path);
		GC.KeepAlive(this);
		return returnValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SwapRedBlueChannels()
	{
		Methods.ulBitmapSwapRedBlueChannels(Handle);
		GC.KeepAlive(this);
	}

	public override void Dispose()
	{
		if (!IsDisposed && Owns) Methods.ulDestroyBitmap(Handle);
		base.Dispose();
	}

	public ULBitmap Clone() => ULBitmap.FromHandle(Methods.ulCreateBitmapFromCopy(Handle), true);
	object ICloneable.Clone() => Clone();

	public static bool ReferenceEquals(ULBitmap? objA, ULBitmap? objB) => objA is not null ? (objB is not null ? objA._ptr == objB._ptr : false) : objB is null;

	public bool Equals(ULBitmap? other)
	{
		if (other is null) return false;
		if (ReferenceEquals(this, other)) return true;
		uint height = Height;
		uint width = Width;
		uint bpp = Bpp;
		if (width != other.Width || height != other.Height || Format != other.Format || bpp != other.Bpp || IsEmpty != other.IsEmpty) return false;

		int rowLength = (int)(bpp * width);

		nuint rowBytes = RowBytes;
		nuint rowBytesOther = other.RowBytes;
		byte* pixels = (byte*)LockPixels();
		byte* pixelsOther = (byte*)other.LockPixels();

		bool seqEq = true;

		for (nuint y = 0; y < height; y++)
		{
			if (!new ReadOnlySpan<byte>(pixels + (rowBytes * y), rowLength).SequenceEqual(new ReadOnlySpan<byte>(pixelsOther + (rowBytesOther * y), rowLength)))
			{
				seqEq = false;
				break;
			}
		}

		UnlockPixels();
		other.UnlockPixels();

		return seqEq;
	}
	public override bool Equals(object? other) => other is ULBitmap bitmap ? Equals(bitmap) : false;

	public static ULBitmap FromHandle(Handle<ULBitmap> handle, bool dispose) => new() { Handle = handle, Owns = dispose };
}
