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
		public static unsafe partial IntPtr ulCreateBitmapFromPixels(uint width, uint height, ULBitmapFormat format, uint row_bytes, void* pixels, nuint size, [MarshalAs(UnmanagedType.I1)] bool should_copy);

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
		[GeneratedDllImport("Ultralight")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulBitmapWritePNG(IntPtr bitmap, [MarshalUsing(typeof(UTF8Marshaller))] string path);

		/// <summary>This converts a BGRA bitmap to RGBA bitmap and vice-versa by swapping the red and blue channels.</summary>
		[DllImport("Ultralight")]
		public static extern void ulBitmapSwapRedBlueChannels(IntPtr bitmap);
	}
	public unsafe class ULBitmap : IDisposable, ICloneable
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
	}
}
