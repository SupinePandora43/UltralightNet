using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UltralightNet;

public unsafe delegate void DestroyBufferCallback(void* userData, void* data);

public unsafe struct ULBuffer : IDisposable, IEquatable<ULBuffer> // TODO: INativeContainer
{
	private nuint handle;

	public ULBuffer(void* data, nuint size, DestroyBufferCallback destroyCallback, void* userData = null) : this(data is null && destroyCallback is not null ? throw new ArgumentException("Callback will not be called, if data is null", nameof(data)) : data, size, destroyCallback is not null ? (delegate*<void*, void*, void>)Marshal.GetFunctionPointerForDelegate(destroyCallback) : null, userData) { }
	public ULBuffer(void* data, nuint size, delegate*<void*, void*, void> destroyCallback = null, void* userData = null)
	{
		handle = data is null && destroyCallback is not null ? throw new ArgumentException("Callback will not be called, if data is null", nameof(data)) : ulCreateBuffer(data, size, userData, destroyCallback);

		[DllImport(Methods.LibUltralight)]
		static extern nuint ulCreateBuffer(void* data, nuint size, void* userData, delegate*<void*, void*, void> destroyCallback);
	}
	public ULBuffer(void* data, nuint size)
	{
		handle = ulCreateBufferFromCopy(data, size);

		[DllImport(Methods.LibUltralight)]
		static extern nuint ulCreateBufferFromCopy(void* data, nuint size);
	}
	public static ULBuffer CreateFromSpan<T>(ReadOnlySpan<T> span) where T : unmanaged
	{
		fixed (byte* bytePtr = MemoryMarshal.Cast<T, byte>(span))
			return new(bytePtr, (nuint)span.Length);
	}

	public void Dispose()
	{
		if (handle is 0) return;
		ulDestroyBuffer(handle);
		handle = 0;

		[DllImport(Methods.LibUltralight)]
		static extern void* ulDestroyBuffer(nuint buffer);
	}

	/// <summary>Use <see cref="MemoryMarshal.Cast{TFrom, TTo}(Span{TFrom})" /> to convert it to type of your choice</summary>
	public readonly Span<byte> DataSpan => new(Data, checked((int)Size));

	public readonly byte* Data
	{
		get
		{
			Debug.Assert(handle is not 0);
			return ulBufferGetData(handle);

			[DllImport(Methods.LibUltralight)]
			static extern byte* ulBufferGetData(nuint buffer);
		}
	}
	public readonly nuint Size
	{
		get
		{
			Debug.Assert(handle is not 0);
			return ulBufferGetSize(handle);

			[DllImport(Methods.LibUltralight)]
			static extern nuint ulBufferGetSize(nuint buffer);
		}
	}
	public readonly void* UserData
	{
		get
		{
			Debug.Assert(handle is not 0);
			return ulBufferGetUserData(handle);

			[DllImport(Methods.LibUltralight)]
			static extern void* ulBufferGetUserData(nuint buffer);
		}
	}
	public readonly bool OwnsData
	{
		get
		{
			Debug.Assert(handle is not 0);
			return ulBufferOwnsData(handle) != 0;

			[DllImport(Methods.LibUltralight)]
			static extern byte ulBufferOwnsData(nuint buffer);
		}
	}

	public readonly bool Equals(ULBuffer buffer) // so called "premature" optimization...
	{
		if (handle == buffer.handle) return true;
		if (handle is 0 || buffer.handle is 0) return false;
		nuint size = Size;
		if (size != buffer.Size || OwnsData != buffer.OwnsData) return false;
		byte* data = Data, bufferData = buffer.Data;
		if (data == bufferData) return true;
		if (size < (nuint)int.MaxValue) return new ReadOnlySpan<byte>(data, unchecked((int)size)).SequenceEqual(new ReadOnlySpan<byte>(bufferData, unchecked((int)size)));
		else
			for (nuint i = 0; i < Size; i++)
				if (data[i] != bufferData[i]) return false;
		return true;
	}
}
