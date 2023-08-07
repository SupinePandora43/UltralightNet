using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using UltralightNet.LowStuff;

namespace UltralightNet;

public static unsafe partial class Methods
{
	/// <summary>Create empty bitmap.</summary>
	[LibraryImport(LibUltralight)]
	internal static partial ULBitmap ulCreateEmptyBitmap();

	/// <summary>Create bitmap with certain dimensions and pixel format.</summary>
	[LibraryImport(LibUltralight)]
	internal static partial ULBitmap ulCreateBitmap(uint width, uint height, ULBitmapFormat format);

	/// <summary>Create bitmap from existing pixel buffer. @see Bitmap for help using this function.</summary>
	[LibraryImport(LibUltralight)]
	internal static unsafe partial ULBitmap ulCreateBitmapFromPixels(uint width, uint height, ULBitmapFormat format, uint rowBytes, byte* pixels, nuint size, [MarshalAs(UnmanagedType.U1)] bool shouldCopy);

	/// <summary>Create bitmap from copy.</summary>
	[LibraryImport(LibUltralight)]
	internal static partial ULBitmap ulCreateBitmapFromCopy(ULBitmap existingBitmap);

	/// <summary>Destroy a bitmap (you should only destroy Bitmaps you have explicitly created via one of the creation functions above.</summary>
	[LibraryImport(LibUltralight)]
	internal static partial void ulDestroyBitmap(ULBitmap bitmap);

	/// <summary>Get the width in pixels.</summary>
	[LibraryImport(LibUltralight)]
	internal static partial uint ulBitmapGetWidth(ULBitmap bitmap);

	/// <summary>Get the height in pixels.</summary>
	[LibraryImport(LibUltralight)]
	internal static partial uint ulBitmapGetHeight(ULBitmap bitmap);

	/// <summary>Get the pixel format.</summary>
	[LibraryImport(LibUltralight)]
	internal static partial ULBitmapFormat ulBitmapGetFormat(ULBitmap bitmap);

	/// <summary>Get the bytes per pixel.</summary>
	[LibraryImport(LibUltralight)]
	internal static partial uint ulBitmapGetBpp(ULBitmap bitmap);

	/// <summary>Get the number of bytes per row.</summary>
	[LibraryImport(LibUltralight)]
	internal static partial uint ulBitmapGetRowBytes(ULBitmap bitmap);

	/// <summary>
	/// Get the size in bytes of the underlying pixel buffer.
	/// </summary>
	[LibraryImport(LibUltralight)]
	internal static partial nuint ulBitmapGetSize(ULBitmap bitmap);

	/// <summary>Whether or not this bitmap owns its own pixel buffer.</summary>
	[LibraryImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static partial bool ulBitmapOwnsPixels(ULBitmap bitmap);

	/// <summary>Lock pixels for reading/writing, returns pointer to pixel buffer.</summary>
	[LibraryImport(LibUltralight)]
	internal static partial byte* ulBitmapLockPixels(ULBitmap bitmap);

	/// <summary>Unlock pixels after locking.</summary>
	[LibraryImport(LibUltralight)]
	internal static partial void ulBitmapUnlockPixels(ULBitmap bitmap);

	/// <summary>Get raw pixel buffer</summary>
	/// <remarks>you should only call this if Bitmap is already locked.</remarks>
	[LibraryImport(LibUltralight)]
	internal static partial byte* ulBitmapRawPixels(ULBitmap bitmap);

	/// <summary>Whether or not this bitmap is empty.</summary>
	[LibraryImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static partial bool ulBitmapIsEmpty(ULBitmap bitmap);

	/// <summary>Reset bitmap pixels to 0.</summary>
	[LibraryImport(LibUltralight)]
	internal static partial void ulBitmapErase(ULBitmap bitmap);

	/// <summary>Write bitmap to a PNG on disk.</summary>
	[LibraryImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static partial bool ulBitmapWritePNG(ULBitmap bitmap, [MarshalUsing(typeof(Utf8StringMarshaller))] string path);

	/// <summary>This converts a BGRA bitmap to RGBA bitmap and vice-versa by swapping the red and blue channels.</summary>
	[LibraryImport(LibUltralight)]
	internal static partial void ulBitmapSwapRedBlueChannels(ULBitmap bitmap);
}

#pragma warning disable CS0659
[NativeMarshalling(typeof(Marshaller))]
public sealed unsafe class ULBitmap : NativeContainer, ICloneable, IEquatable<ULBitmap>
#pragma warning restore CS0659
{
	public uint Width => Methods.ulBitmapGetWidth(this);
	public uint Height => Methods.ulBitmapGetHeight(this);

	public ULBitmapFormat Format => Methods.ulBitmapGetFormat(this);
	public uint Bpp => Methods.ulBitmapGetBpp(this);
	public uint RowBytes => Methods.ulBitmapGetRowBytes(this);
	public nuint Size => Methods.ulBitmapGetSize(this);

	public bool OwnsPixels => Methods.ulBitmapOwnsPixels(this);

	public byte* LockPixels() => Methods.ulBitmapLockPixels(this);
	public void UnlockPixels() => Methods.ulBitmapUnlockPixels(this);

	public byte* RawPixels => Methods.ulBitmapRawPixels(this);
	public bool IsEmpty => Methods.ulBitmapIsEmpty(this);
	public void Erase() => Methods.ulBitmapErase(this);

	public bool WritePng(string path) => Methods.ulBitmapWritePNG(this, path);

	public void SwapRedBlueChannels() => Methods.ulBitmapSwapRedBlueChannels(this);

	public override void Dispose()
	{
		if (!IsDisposed && Owns) Methods.ulDestroyBitmap(this);
		base.Dispose();
	}

	public ULBitmap Clone() => Methods.ulCreateBitmapFromCopy(this);
	object ICloneable.Clone() => Clone();

	public static bool ReferenceEquals(ULBitmap? objA, ULBitmap? objB)
	{
		if (objA is null || objB is null) return objA is null && objB is null;
		if (objA.IsDisposed || objB.IsDisposed) return objA.IsDisposed == objB.IsDisposed;
		return objA.Handle == objB.Handle;
	}

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
		byte* pixels = LockPixels();
		byte* pixelsOther = other.LockPixels();

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
	public override bool Equals(object? other) => other is ULBitmap bitmap && Equals(bitmap);

	public static ULBitmap CreateEmpty() => Methods.ulCreateEmptyBitmap();
	public static ULBitmap Create(uint width, uint height, ULBitmapFormat format) => Methods.ulCreateBitmap(width, height, format);
	public static ULBitmap CreateFromPixels(uint width, uint height, ULBitmapFormat format, uint rowBytes, byte* pixels, uint size, bool shouldCopy) => Methods.ulCreateBitmapFromPixels(width, height, format, rowBytes, pixels, size, shouldCopy);


	public static ULBitmap FromHandle(void* handle, bool dispose) => new() { Handle = handle, Owns = dispose };

	[CustomMarshaller(typeof(ULBitmap), MarshalMode.ManagedToUnmanagedIn, typeof(ManagedToUnmanagedIn))]
	[CustomMarshaller(typeof(ULBitmap), MarshalMode.ManagedToUnmanagedOut, typeof(ManagedToUnmanagedOut))]
	internal static class Marshaller
	{
		internal ref struct ManagedToUnmanagedIn
		{
			private ULBitmap bitmap;

			public void FromManaged(ULBitmap bitmap) => this.bitmap = bitmap;
			public readonly void* ToUnmanaged() => bitmap.Handle;
			public readonly void Free() => GC.KeepAlive(bitmap);
		}
		internal ref struct ManagedToUnmanagedOut
		{
			private ULBitmap bitmap;

			public void FromUnmanaged(void* unmanaged) => bitmap = FromHandle(unmanaged, true);
			public readonly ULBitmap ToManaged() => bitmap;
			public readonly void Free() { }
		}
	}
}
