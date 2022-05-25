using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet;

public unsafe delegate void DestroyBufferCallback(void* userData, void* data);

public unsafe readonly ref struct ULBuffer
{
	public static ULBuffer* Create(void* data, nuint size, DestroyBufferCallback destroyCallback, void* userData = null) => data is null && destroyCallback is not null ? throw new ArgumentException("Callback will not be called, if data is null", nameof(data)) : Create(data, size, destroyCallback is not null ? (delegate*<void*, void*, void>)Marshal.GetFunctionPointerForDelegate(destroyCallback) : null, userData);
	public static ULBuffer* Create(void* data, nuint size, delegate*<void*, void*, void> destroyCallback = null, void* userData = null)
	{
		return data is null && destroyCallback is not null ? throw new ArgumentException("Callback will not be called, if data is null", nameof(data)) : ulCreateBuffer(data, size, userData, destroyCallback);

		[DllImport(Methods.LibUltralight)]
		static extern ULBuffer* ulCreateBuffer(void* data, nuint size, void* userData, delegate*<void*, void*, void> destroyCallback);
	}
	public static ULBuffer* CreateFrom(void* data, nuint size)
	{
		return ulCreateBufferFromCopy(data, size);

		[DllImport(Methods.LibUltralight)]
		static extern ULBuffer* ulCreateBufferFromCopy(void* data, nuint size);
	}
	public static ULBuffer* CreateFrom<T>(Span<T> span) where T : unmanaged
	{
		fixed (T* t = span)
			return CreateFrom(t, (nuint)span.Length);
	}

	public void Destroy()
	{
		delegate*<ref byte, void*> a = &Unsafe.AsPointer<byte>;
		delegate*<in ULBuffer, ULBuffer*> b = (delegate*<in ULBuffer, ULBuffer*>)a;
		ulDestroyBuffer(b(this));

		[DllImport(Methods.LibUltralight)]
		static extern void* ulDestroyBuffer(ULBuffer* buffer);
	}

	public void* Data
	{
		get
		{
			delegate*<ref byte, void*> a = &Unsafe.AsPointer<byte>;
			delegate*<in ULBuffer, ULBuffer*> b = (delegate*<in ULBuffer, ULBuffer*>)a;
			return ulBufferGetData(b(this));

			[DllImport(Methods.LibUltralight)]
			static extern void* ulBufferGetData(ULBuffer* buffer);
		}
	}
	public nuint Size
	{
		get
		{
			delegate*<ref byte, void*> a = &Unsafe.AsPointer<byte>;
			delegate*<in ULBuffer, ULBuffer*> b = (delegate*<in ULBuffer, ULBuffer*>)a;
			return ulBufferGetSize(b(this));

			[DllImport(Methods.LibUltralight)]
			static extern nuint ulBufferGetSize(ULBuffer* buffer);
		}
	}
	public void* UserData
	{
		get
		{
			delegate*<ref byte, void*> a = &Unsafe.AsPointer<byte>;
			delegate*<in ULBuffer, ULBuffer*> b = (delegate*<in ULBuffer, ULBuffer*>)a;
			return ulBufferGetUserData(b(this));

			[DllImport(Methods.LibUltralight)]
			static extern void* ulBufferGetUserData(ULBuffer* buffer);
		}
	}
	public bool OwnsData
	{
		get
		{
			delegate*<ref byte, void*> a = &Unsafe.AsPointer<byte>;
			delegate*<in ULBuffer, ULBuffer*> b = (delegate*<in ULBuffer, ULBuffer*>)a;
			return ulBufferOwnsData(b(this)) != 0;

			[DllImport(Methods.LibUltralight)]
			static extern byte ulBufferOwnsData(ULBuffer* buffer);
		}
	}
}
