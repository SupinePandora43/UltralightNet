using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using UltralightNet.LowStuff;

namespace UltralightNet;

/// <summary>
/// Custom rendering backend
/// </summary>

public unsafe struct ULGPUDriver : IDisposable, IEquatable<ULGPUDriver>
{
	public ULGPUDriverBeginSynchronizeCallback? BeginSynchronize
	{
		set => ULPlatform.Handle(ref this, this with { __BeginSynchronize = value is null ? null : (delegate* unmanaged[Cdecl]<void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __BeginSynchronize;
			return p is null ? null : () => p();
		}
	}
	public ULGPUDriverEndSynchronizeCallback? EndSynchronize
	{
		set => ULPlatform.Handle(ref this, this with { __EndSynchronize = value is null ? null : (delegate* unmanaged[Cdecl]<void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __EndSynchronize;
			return p is null ? null : () => p();
		}
	}
	public ULGPUDriverNextTextureIdCallback? NextTextureId
	{
		set => ULPlatform.Handle(ref this, this with { __NextTextureId = value is null ? null : (delegate* unmanaged[Cdecl]<uint>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __NextTextureId;
			return p is null ? null : () => p();
		}
	}
	public ULGPUDriverCreateTextureCallback? CreateTexture
	{
		set => _CreateTexture = value is null ? null : (id, bitmap) => value(id, ULBitmap.FromHandle(bitmap, false));
		readonly get
		{
			var c = _CreateTexture;
			return c is null ? null : (id, bitmap) => c(id, bitmap.Handle);
		}
	}
	public ULGPUDriverCreateTextureCallback__PInvoke__? _CreateTexture
	{
		set => ULPlatform.Handle(ref this, this with { __CreateTexture = value is null ? null : (delegate* unmanaged[Cdecl]<uint, Handle<ULBitmap>, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __CreateTexture;
			return p is null ? null : (id, bitmap) => p(id, bitmap);
		}
	}
	public ULGPUDriverUpdateTextureCallback? UpdateTexture
	{
		set => _UpdateTexture = value is null ? null : (id, bitmap) => value(id, ULBitmap.FromHandle(bitmap, false));
		readonly get
		{
			var c = _UpdateTexture;
			return c is null ? null : (id, bitmap) => c(id, bitmap.Handle);
		}
	}
	public ULGPUDriverUpdateTextureCallback__PInvoke__? _UpdateTexture
	{
		set => ULPlatform.Handle(ref this, this with { __UpdateTexture = value is null ? null : (delegate* unmanaged[Cdecl]<uint, Handle<ULBitmap>, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __UpdateTexture;
			return p is null ? null : (id, bitmap) => p(id, bitmap);
		}
	}
	public ULGPUDriverDestroyTextureCallback? DestroyTexture
	{
		set => ULPlatform.Handle(ref this, this with { __DestroyTexture = value is null ? null : (delegate* unmanaged[Cdecl]<uint, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __DestroyTexture;
			return p is null ? null : (id) => p(id);
		}
	}
	public ULGPUDriverNextRenderBufferIdCallback? NextRenderBufferId
	{
		set => ULPlatform.Handle(ref this, this with { __NextRenderBufferId = value is null ? null : (delegate* unmanaged[Cdecl]<uint>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __NextRenderBufferId;
			return p is null ? null : () => p();
		}
	}
	public ULGPUDriverCreateRenderBufferCallback? CreateRenderBuffer
	{
		set => ULPlatform.Handle(ref this, this with { __CreateRenderBuffer = value is null ? null : (delegate* unmanaged[Cdecl]<uint, ULRenderBuffer, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __CreateRenderBuffer;
			return p is null ? null : (id, renderBuffer) => p(id, renderBuffer);
		}
	}
	public ULGPUDriverDestroyRenderBufferCallback? DestroyRenderBuffer
	{
		set => ULPlatform.Handle(ref this, this with { __DestroyRenderBuffer = value is null ? null : (delegate* unmanaged[Cdecl]<uint, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __DestroyRenderBuffer;
			return p is null ? null : (id) => p(id);
		}
	}
	public ULGPUDriverNextGeometryIdCallback? NextGeometryId
	{
		set => ULPlatform.Handle(ref this, this with { __NextGeometryId = value is null ? null : (delegate* unmanaged[Cdecl]<uint>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __NextGeometryId;
			return p is null ? null : () => p();
		}
	}
	public ULGPUDriverCreateGeometryCallback? CreateGeometry
	{
		set => ULPlatform.Handle(ref this, this with { __CreateGeometry = value is null ? null : (delegate* unmanaged[Cdecl]<uint, ULVertexBuffer, ULIndexBuffer, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __CreateGeometry;
			return p is null ? null : (id, vertexBuffer, indexBuffer) => p(id, vertexBuffer, indexBuffer);
		}
	}
	public ULGPUDriverUpdateGeometryCallback? UpdateGeometry
	{
		set => ULPlatform.Handle(ref this, this with { __UpdateGeometry = value is null ? null : (delegate* unmanaged[Cdecl]<uint, ULVertexBuffer, ULIndexBuffer, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __UpdateGeometry;
			return p is null ? null : (id, vertexBuffer, indexBuffer) => p(id, vertexBuffer, indexBuffer);
		}
	}
	public ULGPUDriverDestroyGeometryCallback? DestroyGeometry
	{
		set => ULPlatform.Handle(ref this, this with { __DestroyGeometry = value is null ? null : (delegate* unmanaged[Cdecl]<uint, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __DestroyGeometry;
			return p is null ? null : (id) => p(id);
		}
	}
	public ULGPUDriverUpdateCommandListCallback? UpdateCommandList
	{
		set => ULPlatform.Handle(ref this, this with { __UpdateCommandList = value is null ? null : (delegate* unmanaged[Cdecl]<ULCommandList, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
		readonly get
		{
			var p = __UpdateCommandList;
			return p is null ? null : (list) => p(list);
		}
	}

	public delegate* unmanaged[Cdecl]<void> __BeginSynchronize;
	public delegate* unmanaged[Cdecl]<void> __EndSynchronize;
	public delegate* unmanaged[Cdecl]<uint> __NextTextureId;
	public delegate* unmanaged[Cdecl]<uint, Handle<ULBitmap>, void> __CreateTexture;
	public delegate* unmanaged[Cdecl]<uint, Handle<ULBitmap>, void> __UpdateTexture;
	public delegate* unmanaged[Cdecl]<uint, void> __DestroyTexture;
	public delegate* unmanaged[Cdecl]<uint> __NextRenderBufferId;
	public delegate* unmanaged[Cdecl]<uint, ULRenderBuffer, void> __CreateRenderBuffer;
	public delegate* unmanaged[Cdecl]<uint, void> __DestroyRenderBuffer;
	public delegate* unmanaged[Cdecl]<uint> __NextGeometryId;
	public delegate* unmanaged[Cdecl]<uint, ULVertexBuffer, ULIndexBuffer, void> __CreateGeometry;
	public delegate* unmanaged[Cdecl]<uint, ULVertexBuffer, ULIndexBuffer, void> __UpdateGeometry;
	public delegate* unmanaged[Cdecl]<uint, void> __DestroyGeometry;
	public delegate* unmanaged[Cdecl]<ULCommandList, void> __UpdateCommandList;

	public void Dispose()
	{
		ULPlatform.Free(this);
	}

#pragma warning disable CS8909
	public readonly bool Equals(ULGPUDriver driver) =>
		__BeginSynchronize == driver.__BeginSynchronize &&
		__EndSynchronize == driver.__EndSynchronize &&
		__NextTextureId == driver.__NextTextureId &&
		__CreateTexture == driver.__CreateTexture &&
		__UpdateTexture == driver.__UpdateTexture &&
		__DestroyTexture == driver.__DestroyTexture &&
		__NextRenderBufferId == driver.__NextRenderBufferId &&
		__CreateRenderBuffer == driver.__CreateRenderBuffer &&
		__DestroyRenderBuffer == driver.__DestroyRenderBuffer &&
		__NextGeometryId == driver.__NextGeometryId &&
		__CreateGeometry == driver.__CreateGeometry &&
		__UpdateGeometry == driver.__UpdateGeometry &&
		__DestroyGeometry == driver.__DestroyGeometry &&
		__UpdateCommandList == driver.__UpdateCommandList;
#pragma warning restore CS8909
	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is ULGPUDriver driver ? Equals(driver) : false;

	public readonly override int GetHashCode() =>
#if NETSTANDARD2_1 || NETCOREAPP2_1_OR_GREATER
		HashCode.Combine(
			HashCode.Combine((nuint)__BeginSynchronize, (nuint)__EndSynchronize, (nuint)__NextTextureId, (nuint)__CreateTexture),
			HashCode.Combine((nuint)__UpdateTexture, (nuint)__DestroyTexture, (nuint)__NextRenderBufferId, (nuint)__CreateRenderBuffer),
			HashCode.Combine((nuint)__DestroyRenderBuffer, (nuint)__NextGeometryId, (nuint)__CreateGeometry, (nuint)__UpdateGeometry, (nuint)__DestroyGeometry, (nuint)__UpdateCommandList)
		);
#else
		base.GetHashCode();
#endif
}
