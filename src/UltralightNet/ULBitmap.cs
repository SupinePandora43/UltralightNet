using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet;

public static partial class Methods
{
	/// <summary>Create empty bitmap.</summary>
	[DllImport(LibUltralight)]
	public static extern IntPtr ulCreateEmptyBitmap();

	/// <summary>Create bitmap with certain dimensions and pixel format.</summary>
	[DllImport(LibUltralight)]
	public static extern IntPtr ulCreateBitmap(uint width, uint height, ULBitmapFormat format);

	/// <summary>Create bitmap from existing pixel buffer. @see Bitmap for help using this function.</summary>
	[GeneratedDllImport(LibUltralight)]
	public static unsafe partial IntPtr ulCreateBitmapFromPixels(uint width, uint height, ULBitmapFormat format, uint rowBytes, void* pixels, nuint size, [MarshalAs(UnmanagedType.I1)] bool shouldCopy);

	/// <summary>Create bitmap from copy.</summary>
	[DllImport(LibUltralight)]
	public static extern IntPtr ulCreateBitmapFromCopy(IntPtr existingBitmap);

	/// <summary>Destroy a bitmap (you should only destroy Bitmaps you have explicitly created via one of the creation functions above.</summary>
	[DllImport(LibUltralight)]
	public static extern void ulDestroyBitmap(IntPtr bitmap);

	/// <summary>Get the width in pixels.</summary>
	[DllImport(LibUltralight)]
	public static extern uint ulBitmapGetWidth(IntPtr bitmap);

	/// <summary>Get the height in pixels.</summary>
	[DllImport(LibUltralight)]
	public static extern uint ulBitmapGetHeight(IntPtr bitmap);

	/// <summary>Get the pixel format.</summary>
	[DllImport(LibUltralight)]
	public static extern ULBitmapFormat ulBitmapGetFormat(IntPtr bitmap);

	/// <summary>Get the bytes per pixel.</summary>
	[DllImport(LibUltralight)]
	public static extern uint ulBitmapGetBpp(IntPtr bitmap);

	/// <summary>Get the number of bytes per row.</summary>
	[DllImport(LibUltralight)]
	public static extern uint ulBitmapGetRowBytes(IntPtr bitmap);

	/// <summary>
	/// Get the size in bytes of the underlying pixel buffer.
	/// </summary>
	[DllImport(LibUltralight)]
	public static extern nuint ulBitmapGetSize(IntPtr bitmap);

	/// <summary>Whether or not this bitmap owns its own pixel buffer.</summary>
	[GeneratedDllImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulBitmapOwnsPixels(IntPtr bitmap);

	/// <summary>Lock pixels for reading/writing, returns pointer to pixel buffer.</summary>
	[DllImport(LibUltralight)]
	public static extern IntPtr ulBitmapLockPixels(IntPtr bitmap);

	/// <summary>Unlock pixels after locking.</summary>
	[DllImport(LibUltralight)]
	public static extern void ulBitmapUnlockPixels(IntPtr bitmap);

	/// <summary>Get raw pixel buffer</summary>
	/// <remarks>you should only call this if Bitmap is already locked.</remarks>
	[DllImport(LibUltralight)]
	public static extern IntPtr ulBitmapRawPixels(IntPtr bitmap);

	/// <summary>Whether or not this bitmap is empty.</summary>
	[GeneratedDllImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulBitmapIsEmpty(IntPtr bitmap);

	/// <summary>Reset bitmap pixels to 0.</summary>
	[DllImport(LibUltralight)]
	public static extern void ulBitmapErase(IntPtr bitmap);

	/// <summary>Write bitmap to a PNG on disk.</summary>
	[GeneratedDllImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulBitmapWritePNG(IntPtr bitmap, [MarshalUsing(typeof(UTF8Marshaller))] string path);

	/// <summary>This converts a BGRA bitmap to RGBA bitmap and vice-versa by swapping the red and blue channels.</summary>
	[DllImport(LibUltralight)]
	public static extern void ulBitmapSwapRedBlueChannels(IntPtr bitmap);
}
#pragma warning disable CS0659
public unsafe class ULBitmap : IDisposable, ICloneable, IEquatable<ULBitmap>
#pragma warning restore CS0659
{
	private IntPtr _ptr;
	public IntPtr Ptr
	{
		get
		{
			static void Throw() => throw new ObjectDisposedException(nameof(ULBitmap));
			if (IsDisposed) Throw();
			ULPlatform.CheckThread();
			return _ptr;
		}
		init => _ptr = value;
	}
	public bool IsDisposed { get; private set; } = false;
	private readonly bool dispose = true;

	public ULBitmap(IntPtr ptr, bool dispose = false)
	{
		Ptr = ptr;
		this.dispose = dispose;
	}

	public ULBitmap(uint width, uint height, ULBitmapFormat format) => Ptr = Methods.ulCreateBitmap(width, height, format);
	public ULBitmap(uint width, uint height, ULBitmapFormat format, uint row_bytes, void* pixels, uint size, bool should_copy) => Ptr = Methods.ulCreateBitmapFromPixels(width, height, format, row_bytes, pixels, size, should_copy);

	public uint Width
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Methods.ulBitmapGetWidth(Ptr);
	}
	public uint Height
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Methods.ulBitmapGetHeight(Ptr);
	}

	public ULBitmapFormat Format
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Methods.ulBitmapGetFormat(Ptr);
	}
	public uint Bpp
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Methods.ulBitmapGetBpp(Ptr);
	}
	public uint RowBytes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Methods.ulBitmapGetRowBytes(Ptr);
	}
	public nuint Size
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Methods.ulBitmapGetSize(Ptr);
	}

	public bool OwnsPixels => Methods.ulBitmapOwnsPixels(Ptr);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IntPtr LockPixels() => Methods.ulBitmapLockPixels(Ptr);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void UnlockPixels() => Methods.ulBitmapUnlockPixels(Ptr);

	public IntPtr RawPixels
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Methods.ulBitmapRawPixels(Ptr);
	}
	public bool IsEmpty
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Methods.ulBitmapIsEmpty(Ptr);
	}
	public void Erase() => Methods.ulBitmapErase(Ptr);

	public bool WritePng(string path) => Methods.ulBitmapWritePNG(Ptr, path);
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SwapRedBlueChannels() => Methods.ulBitmapSwapRedBlueChannels(Ptr);

	~ULBitmap() => Dispose();

	public void Dispose()
	{
		if (IsDisposed || !dispose) return;
		Methods.ulDestroyBitmap(Ptr);

		IsDisposed = true;
		GC.SuppressFinalize(this);
	}

	public ULBitmap Clone() => new ULBitmap(Methods.ulCreateBitmapFromCopy(Ptr), true);
	object ICloneable.Clone() => Clone();

	public static bool ReferenceEquals(ULBitmap? objA, ULBitmap? objB) => objA is not null ? (objB is not null ? objA.Ptr == objB.Ptr : false) : objB is null;

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
}
