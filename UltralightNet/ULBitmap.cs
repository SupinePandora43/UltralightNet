using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		/// <summary>Create empty bitmap.</summary>
		[DllImport("Ultralight")]
		public static extern IntPtr ulCreateEmptyBitmap();

		/// <summary>Create bitmap with certain dimensions and pixel format.</summary>
		[DllImport("Ultralight")]
		public static extern IntPtr ulCreateBitmap(uint width, uint height, ULBitmapFormat format);

		/// <summary>Create bitmap from existing pixel buffer. @see Bitmap for help using this function.</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial IntPtr ulCreateBitmapFromPixels(uint width, uint height, ULBitmapFormat format, uint row_bytes, IntPtr pixels, uint size, [MarshalAs(UnmanagedType.I1)] bool should_copy);

		/// <summary>Create bitmap from copy.</summary>
		[DllImport("Ultralight")]
		public static extern IntPtr ulCreateBitmapFromCopy(IntPtr existing_bitmap);

		/// <summary>Destroy a bitmap (you should only destroy Bitmaps you have explicitly created via one of the creation functions above.</summary>
		[DllImport("Ultralight")]
		public static extern void ulDestroyBitmap(IntPtr bitmap);

		/// <summary>Get the width in pixels.</summary>
		[DllImport("Ultralight")]
		public static extern uint ulBitmapGetWidth(IntPtr bitmap);

		/// <summary>Get the height in pixels.</summary>
		[DllImport("Ultralight")]
		public static extern uint ulBitmapGetHeight(IntPtr bitmap);

		/// <summary>Get the pixel format.</summary>
		[DllImport("Ultralight")]
		public static extern ULBitmapFormat ulBitmapGetFormat(IntPtr bitmap);

		/// <summary>Get the bytes per pixel.</summary>
		[DllImport("Ultralight")]
		public static extern uint ulBitmapGetBpp(IntPtr bitmap);

		/// <summary>Get the number of bytes per row.</summary>
		[DllImport("Ultralight")]
		public static extern uint ulBitmapGetRowBytes(IntPtr bitmap);

		/// <summary>
		/// Get the size in bytes of the underlying pixel buffer.
		/// </summary>
		[DllImport("Ultralight")]
		public static extern nuint ulBitmapGetSize(IntPtr bitmap);

		/// <summary>Whether or not this bitmap owns its own pixel buffer.</summary>
		[GeneratedDllImport("Ultralight")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulBitmapOwnsPixels(IntPtr bitmap);

		/// <summary>Lock pixels for reading/writing, returns pointer to pixel buffer.</summary>
		[DllImport("Ultralight")]
		public static extern IntPtr ulBitmapLockPixels(IntPtr bitmap);

		/// <summary>Unlock pixels after locking.</summary>
		[DllImport("Ultralight")]
		public static extern void ulBitmapUnlockPixels(IntPtr bitmap);

		/// <summary>Get raw pixel buffer</summary>
		/// <remarks>you should only call this if Bitmap is already locked.</remarks>
		[DllImport("Ultralight")]
		public static extern IntPtr ulBitmapRawPixels(IntPtr bitmap);

		/// <summary>Whether or not this bitmap is empty.</summary>
		[GeneratedDllImport("Ultralight")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulBitmapIsEmpty(IntPtr bitmap);

		/// <summary>Reset bitmap pixels to 0.</summary>
		[DllImport("Ultralight")]
		public static extern void ulBitmapErase(IntPtr bitmap);

		/// <summary>Write bitmap to a PNG on disk.</summary>
#if NET5_0_OR_GREATER
		[GeneratedDllImport("Ultralight", CharSet = CharSet.Ansi)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulBitmapWritePNG(IntPtr bitmap, [MarshalAs(UnmanagedType.LPUTF8Str)] string path);
#else
		/// <summary>Write bitmap to a PNG on disk.</summary>
		public static partial bool ulBitmapWritePNG(IntPtr bitmap, [MarshalAs(UnmanagedType.LPStr)] string path);
#endif

		/// <summary>This converts a BGRA bitmap to RGBA bitmap and vice-versa by swapping the red and blue channels.</summary>
		[DllImport("Ultralight")]
		public static extern void ulBitmapSwapRedBlueChannels(IntPtr bitmap);
	}
	public class ULBitmap : IDisposable, ICloneable
	{
		public readonly IntPtr Ptr;
		public bool IsDisposed { get; private set; }

		public ULBitmap(IntPtr ptr, bool dispose = false)
		{
			Ptr = ptr;
			IsDisposed = !dispose;
		}

		public ULBitmap(uint width, uint height, ULBitmapFormat format) => Ptr = Methods.ulCreateBitmap(width, height, format);
		public ULBitmap(uint width, uint height, ULBitmapFormat format, uint row_bytes, IntPtr pixels, uint size, bool should_copy) => Ptr = Methods.ulCreateBitmapFromPixels(width, height, format, row_bytes, pixels, size, should_copy);

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

		public ULBitmapFormat Format => Methods.ulBitmapGetFormat(Ptr);
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
			if (IsDisposed) return;
			Methods.ulDestroyBitmap(Ptr);

			IsDisposed = true;
			GC.SuppressFinalize(this);
		}

		public object Clone() => new ULBitmap(Methods.ulCreateBitmapFromCopy(Ptr), true);

		/// <summary>
		/// literally creates <see cref="ULBitmap"/> from <see cref="IntPtr"/> and back, pls don't use
		/// </summary>
		public class Marshaler : ICustomMarshaler
		{
			private static readonly Marshaler instance = new();

			public static ICustomMarshaler GetInstance(string cookie) => instance;

			public void CleanUpManagedData(object ManagedObj) { }

			public void CleanUpNativeData(IntPtr pNativeData) { }

			public int GetNativeDataSize() => 1;

			public IntPtr MarshalManagedToNative(object ManagedObj) => ((ULBitmap)ManagedObj).Ptr;

			public object MarshalNativeToManaged(IntPtr pNativeData) => new ULBitmap(pNativeData);
		}
	}
}
