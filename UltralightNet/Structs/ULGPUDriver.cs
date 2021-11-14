using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ULGPUDriver : IDisposable
	{
		public ULGPUDriverBeginSynchronizeCallback BeginSynchronize
		{
			set
			{
				ULGPUDriverBeginSynchronizeCallback callback = value;
				if (callback is null) __BeginSynchronize = null;
				else
				{
					ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
					__BeginSynchronize = (delegate* unmanaged[Cdecl]<void>)Marshal.GetFunctionPointerForDelegate(callback);
				}
			}
		}
		public ULGPUDriverEndSynchronizeCallback EndSynchronize
		{
			set
			{
				ULGPUDriverEndSynchronizeCallback callback = value;
				if (callback is null) __EndSynchronize = null;
				else
				{
					ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
					__EndSynchronize = (delegate* unmanaged[Cdecl]<void>)Marshal.GetFunctionPointerForDelegate(callback);
				}
			}
		}
		public ULGPUDriverNextTextureIdCallback NextTextureId
		{
			set
			{
				ULGPUDriverNextTextureIdCallback callback = value;
				ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
				__NextTextureId = (delegate* unmanaged[Cdecl]<uint>)Marshal.GetFunctionPointerForDelegate(callback);
			}
		}
		public ULGPUDriverCreateTextureCallback CreateTexture { set => _CreateTexture = (id, bitmap) => value(id, new ULBitmap(bitmap)); }
		public ULGPUDriverCreateTextureCallback__PInvoke__ _CreateTexture
		{
			set
			{
				ULGPUDriverCreateTextureCallback__PInvoke__ callback = value;
				ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
				__CreateTexture = (delegate* unmanaged[Cdecl]<uint, void*, void>)Marshal.GetFunctionPointerForDelegate(callback);
			}
		}
		public ULGPUDriverUpdateTextureCallback UpdateTexture { set => _UpdateTexture = (id, bitmap) => value(id, new ULBitmap(bitmap)); }
		public ULGPUDriverUpdateTextureCallback__PInvoke__ _UpdateTexture
		{
			set
			{
				ULGPUDriverUpdateTextureCallback__PInvoke__ callback = value;
				ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
				__UpdateTexture = (delegate* unmanaged[Cdecl]<uint, void*, void>)Marshal.GetFunctionPointerForDelegate(callback);
			}
		}
		public ULGPUDriverDestroyTextureCallback DestroyTexture
		{
			set
			{
				ULGPUDriverDestroyTextureCallback callback = value;
				ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
				__DestroyTexture = (delegate* unmanaged[Cdecl]<uint, void>)Marshal.GetFunctionPointerForDelegate(callback);
			}
		}
		public ULGPUDriverNextRenderBufferIdCallback NextRenderBufferId
		{
			set
			{
				ULGPUDriverNextRenderBufferIdCallback callback = value;
				ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
				__NextRenderBufferId = (delegate* unmanaged[Cdecl]<uint>)Marshal.GetFunctionPointerForDelegate(callback);
			}
		}
		public ULGPUDriverCreateRenderBufferCallback CreateRenderBuffer
		{
			set
			{
				ULGPUDriverCreateRenderBufferCallback callback = value;
				ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
				__CreateRenderBuffer = (delegate* unmanaged[Cdecl]<uint, ULRenderBuffer, void>)Marshal.GetFunctionPointerForDelegate(callback);
			}
		}
		public ULGPUDriverDestroyRenderBufferCallback DestroyRenderBuffer
		{
			set
			{
				ULGPUDriverDestroyRenderBufferCallback callback = value;
				ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
				__DestroyRenderBuffer = (delegate* unmanaged[Cdecl]<uint, void>)Marshal.GetFunctionPointerForDelegate(callback);
			}
		}
		public ULGPUDriverNextGeometryIdCallback NextGeometryId
		{
			set
			{
				ULGPUDriverNextGeometryIdCallback callback = value;
				ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
				__NextGeometryId = (delegate* unmanaged[Cdecl]<uint>)Marshal.GetFunctionPointerForDelegate(callback);
			}
		}
		public ULGPUDriverCreateGeometryCallback CreateGeometry
		{
			set
			{
				ULGPUDriverCreateGeometryCallback callback = value;
				ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
				__CreateGeometry = (delegate* unmanaged[Cdecl]<uint, ULVertexBuffer, ULIndexBuffer, void>)Marshal.GetFunctionPointerForDelegate(callback);
			}
		}
		public ULGPUDriverUpdateGeometryCallback UpdateGeometry
		{
			set
			{
				ULGPUDriverUpdateGeometryCallback callback = value;
				ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
				__UpdateGeometry = (delegate* unmanaged[Cdecl]<uint, ULVertexBuffer, ULIndexBuffer, void>)Marshal.GetFunctionPointerForDelegate(callback);
			}
		}
		public ULGPUDriverDestroyGeometryCallback DestroyGeometry
		{
			set
			{
				ULGPUDriverDestroyGeometryCallback callback = value;
				ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
				__DestroyGeometry = (delegate* unmanaged[Cdecl]<uint, void>)Marshal.GetFunctionPointerForDelegate(callback);
			}
		}
		public ULGPUDriverUpdateCommandListCallback UpdateCommandList
		{
			set
			{
				ULGPUDriverUpdateCommandListCallback callback = value;
				ULPlatform.Handle(this, GCHandle.Alloc(callback, GCHandleType.Normal));
				__UpdateCommandList = (delegate* unmanaged[Cdecl]<ULCommandList, void>)Marshal.GetFunctionPointerForDelegate(callback);
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
