using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	/// <summary>
	/// Custom rendering backend
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ULGPUDriver : IDisposable
	{
		public ULGPUDriverBeginSynchronizeCallback? BeginSynchronize
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __BeginSynchronize = null });
				else ULPlatform.Handle(ref this, this with { __BeginSynchronize = (delegate* unmanaged[Cdecl]<void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __BeginSynchronize;
				return p is null ? null : () => p();
			}
		}
		public ULGPUDriverEndSynchronizeCallback? EndSynchronize
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __EndSynchronize = null });
				else ULPlatform.Handle(ref this, this with { __EndSynchronize = (delegate* unmanaged[Cdecl]<void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __EndSynchronize;
				return p is null ? null : () => p();
			}
		}
		public ULGPUDriverNextTextureIdCallback? NextTextureId
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __NextTextureId = null });
				else ULPlatform.Handle(ref this, this with { __NextTextureId = (delegate* unmanaged[Cdecl]<uint>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __NextTextureId;
				return p is null ? null : () => p();
			}
		}
		public ULGPUDriverCreateTextureCallback? CreateTexture
		{
			set => _CreateTexture = value is null ? null : (id, bitmap) => value(id, new ULBitmap((IntPtr)bitmap));
			get
			{
				var c = _CreateTexture;
				return c is null ? null : (id, bitmap) => c(id, (void*)bitmap.Ptr);
			}
		}
		public ULGPUDriverCreateTextureCallback__PInvoke__? _CreateTexture
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __CreateTexture = null });
				else ULPlatform.Handle(ref this, this with { __CreateTexture = (delegate* unmanaged[Cdecl]<uint, void*, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __CreateTexture;
				return p is null ? null : (id, bitmap) => p(id, bitmap);
			}
		}
		public ULGPUDriverUpdateTextureCallback? UpdateTexture
		{
			set => _UpdateTexture = value is null ? null : (id, bitmap) => value(id, new ULBitmap((IntPtr)bitmap));
			get
			{
				var c = _UpdateTexture;
				return c is null ? null : (id, bitmap) => c(id, (void*)bitmap.Ptr);
			}
		}
		public ULGPUDriverUpdateTextureCallback__PInvoke__? _UpdateTexture
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __UpdateTexture = null });
				else ULPlatform.Handle(ref this, this with { __UpdateTexture = (delegate* unmanaged[Cdecl]<uint, void*, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __UpdateTexture;
				return p is null ? null : (id, bitmap) => p(id, bitmap);
			}
		}
		public ULGPUDriverDestroyTextureCallback? DestroyTexture
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __DestroyTexture = null });
				else ULPlatform.Handle(ref this, this with { __DestroyTexture = (delegate* unmanaged[Cdecl]<uint, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __DestroyTexture;
				return p is null ? null : (id) => p(id);
			}
		}
		public ULGPUDriverNextRenderBufferIdCallback? NextRenderBufferId
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __NextRenderBufferId = null });
				else ULPlatform.Handle(ref this, this with { __NextRenderBufferId = (delegate* unmanaged[Cdecl]<uint>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __NextRenderBufferId;
				return p is null ? null : () => p();
			}
		}
		public ULGPUDriverCreateRenderBufferCallback? CreateRenderBuffer
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __CreateRenderBuffer = null });
				else ULPlatform.Handle(ref this, this with { __CreateRenderBuffer = (delegate* unmanaged[Cdecl]<uint, ULRenderBuffer, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __CreateRenderBuffer;
				return p is null ? null : (id, renderBuffer) => p(id, renderBuffer);
			}
		}
		public ULGPUDriverDestroyRenderBufferCallback? DestroyRenderBuffer
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __DestroyRenderBuffer = null });
				else ULPlatform.Handle(ref this, this with { __DestroyRenderBuffer = (delegate* unmanaged[Cdecl]<uint, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __DestroyRenderBuffer;
				return p is null ? null : (id) => p(id);
			}
		}
		public ULGPUDriverNextGeometryIdCallback? NextGeometryId
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __NextGeometryId = null });
				else ULPlatform.Handle(ref this, this with { __NextGeometryId = (delegate* unmanaged[Cdecl]<uint>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __NextGeometryId;
				return p is null ? null : () => p();
			}
		}
		public ULGPUDriverCreateGeometryCallback? CreateGeometry
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __CreateGeometry = null });
				else ULPlatform.Handle(ref this, this with { __CreateGeometry = (delegate* unmanaged[Cdecl]<uint, ULVertexBuffer, ULIndexBuffer, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __CreateGeometry;
				return p is null ? null : (id, vertexBuffer, indexBuffer) => p(id, vertexBuffer, indexBuffer);
			}
		}
		public ULGPUDriverUpdateGeometryCallback? UpdateGeometry
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __UpdateGeometry = null });
				else ULPlatform.Handle(ref this, this with { __UpdateGeometry = (delegate* unmanaged[Cdecl]<uint, ULVertexBuffer, ULIndexBuffer, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __UpdateGeometry;
				return p is null ? null : (id, vertexBuffer, indexBuffer) => p(id, vertexBuffer, indexBuffer);
			}
		}
		public ULGPUDriverDestroyGeometryCallback? DestroyGeometry
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __DestroyGeometry = null });
				else ULPlatform.Handle(ref this, this with { __DestroyGeometry = (delegate* unmanaged[Cdecl]<uint, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __DestroyGeometry;
				return p is null ? null : (id) => p(id);
			}
		}
		public ULGPUDriverUpdateCommandListCallback? UpdateCommandList
		{
			set
			{
				if (value is null) ULPlatform.Handle(ref this, this with { __UpdateCommandList = null });
				else ULPlatform.Handle(ref this, this with { __UpdateCommandList = (delegate* unmanaged[Cdecl]<ULCommandList, void>)Marshal.GetFunctionPointerForDelegate(value) }, value);
			}
			get
			{
				var p = __UpdateCommandList;
				return p is null ? null : (list) => p(list);
			}
		}

		public delegate* unmanaged[Cdecl]<void> __BeginSynchronize;
		public delegate* unmanaged[Cdecl]<void> __EndSynchronize;
		public delegate* unmanaged[Cdecl]<uint> __NextTextureId;
		public delegate* unmanaged[Cdecl]<uint, void*, void> __CreateTexture;
		public delegate* unmanaged[Cdecl]<uint, void*, void> __UpdateTexture;
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
	}
}
