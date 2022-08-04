using System;
using System.Runtime.InteropServices;

namespace UltralightNet;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void DestroyBufferCallback(void* userData, void* data);

public unsafe struct ULBuffer : IDisposable, IEquatable<ULBuffer> // TODO: INativeContainer
{
	private nuint handle;

	public static ULBuffer CreateFromOwnedData(void* data, nuint length, delegate* unmanaged[Cdecl]<void*, void*, void> destroyCallback = null, void* userData = null)
	{
		return data is null && destroyCallback is not null ? throw new ArgumentException("Callback will not be called, if data is null", nameof(data)) : ulCreateBuffer(data, length, userData, destroyCallback);

		[DllImport(Methods.LibUltralight)]
		static extern ULBuffer ulCreateBuffer(void* data, nuint length, void* userData, delegate* unmanaged[Cdecl]<void*, void*, void> destroyCallback);
	}
	public static ULBuffer CreateFromOwnedData(void* data, nuint size, DestroyBufferCallback? destroyCallback, void* userData = null) => CreateFromOwnedData(data is null && destroyCallback is not null ? throw new ArgumentException("Callback will not be called, if data is null", nameof(data)) : data, size, destroyCallback is not null ? (delegate* unmanaged[Cdecl]<void*, void*, void>)Marshal.GetFunctionPointerForDelegate(destroyCallback) : null, userData);
	public static ULBuffer CreateFromOwnedData<T>(ReadOnlySpan<T> data, delegate* unmanaged[Cdecl]<void*, void*, void> destroyCallback = null, void* userData = null) where T : unmanaged
	{
		fixed (T* dataPointer = data)
			return CreateFromOwnedData(dataPointer, unchecked((nuint)data.Length), destroyCallback, userData);
	}
	public static ULBuffer CreateFromOwnedData<T>(ReadOnlySpan<T> data, DestroyBufferCallback? destroyCallback, void* userData = null) where T : unmanaged
	{
		fixed (T* dataPointer = data)
			return CreateFromOwnedData(dataPointer, unchecked((nuint)data.Length), destroyCallback, userData);
	}

	public static ULBuffer CreateFromDataCopy(void* data, nuint length)
	{
		if (data is null && length is not 0) throw new ArgumentNullException(nameof(data));

		return ulCreateBufferFromCopy(data, length);

		[DllImport(Methods.LibUltralight)]
		static extern ULBuffer ulCreateBufferFromCopy(void* data, nuint length);
	}
	public static ULBuffer CreateFromDataCopy<T>(ReadOnlySpan<T> data) where T : unmanaged
	{
		fixed (T* dataPointer = data)
			return CreateFromDataCopy(dataPointer, (nuint)data.Length);
	}

	public void Dispose()
	{
		if (handle is 0) return;
		ulDestroyBuffer(handle);
		handle = 0;

		[DllImport(Methods.LibUltralight)]
		static extern void* ulDestroyBuffer(nuint buffer);
	}

	public readonly bool IsDisposed => handle is 0;
	private readonly nuint Handle => !IsDisposed ? handle : throw new ObjectDisposedException(nameof(ULBuffer));

	/// <summary>Use <see cref="MemoryMarshal.Cast{TFrom, TTo}(Span{TFrom})" /> to convert it to type of your choice</summary>
	public readonly Span<byte> DataSpan => new(Data, checked((int)Size));

	public readonly byte* Data
	{
		get
		{
			return ulBufferGetData(Handle);

			[DllImport(Methods.LibUltralight)]
			static extern byte* ulBufferGetData(nuint buffer);
		}
	}
	public readonly nuint Size
	{
		get
		{
			return ulBufferGetSize(Handle);

			[DllImport(Methods.LibUltralight)]
			static extern nuint ulBufferGetSize(nuint buffer);
		}
	}
	public readonly void* UserData
	{
		get
		{
			return ulBufferGetUserData(Handle);

			[DllImport(Methods.LibUltralight)]
			static extern void* ulBufferGetUserData(nuint buffer);
		}
	}
	public readonly bool OwnsData
	{
		get
		{
			return ulBufferOwnsData(Handle) != 0;

			[DllImport(Methods.LibUltralight)]
			static extern byte ulBufferOwnsData(nuint buffer);
		}
	}

	public readonly bool Equals(ULBuffer buffer) // so called "premature" optimization...
	{
		if (handle == buffer.handle) return true;
		if (handle is 0 || buffer.handle is 0) return false;
		nuint size = Size;
		if (size != buffer.Size) return false;
		byte* data = Data, bufferData = buffer.Data;
		if (data == bufferData) return true;
		if (size < int.MaxValue) return new ReadOnlySpan<byte>(data, unchecked((int)size)).SequenceEqual(new ReadOnlySpan<byte>(bufferData, unchecked((int)size)));
		else
			for (nuint i = 0; i < Size; i++)
				if (data[i] != bufferData[i]) return false;
		return true;
	}

	public readonly override bool Equals(object? obj) => obj is ULBuffer buffer && Equals(buffer);
	public override int GetHashCode() => unchecked((int)handle);

	public static bool operator ==(ULBuffer left, ULBuffer right) => left.Equals(right);
	public static bool operator !=(ULBuffer left, ULBuffer right) => !(left == right);
}
