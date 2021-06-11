using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public struct ULGPUDriver
	{
		public ULGPUDriverCreateTextureCallback CreateTexture { set => _CreateTexture = (id, bitmap) => value(id, new ULBitmap(bitmap)); }
		public ULGPUDriverUpdateTextureCallback UpdateTexture { set => _UpdateTexture = (id, bitmap) => value(id, new ULBitmap(bitmap)); }

		public ULGPUDriverBeginSynchronizeCallback BeginSynchronize;
		public ULGPUDriverEndSynchronizeCallback EndSynchronize;
		public ULGPUDriverNextTextureIdCallback NextTextureId;
		public ULGPUDriverCreateTextureCallback__PInvoke__ _CreateTexture;
		public ULGPUDriverUpdateTextureCallback__PInvoke__ _UpdateTexture;
		public ULGPUDriverDestroyTextureCallback DestroyTexture;
		public ULGPUDriverNextRenderBufferIdCallback NextRenderBufferId;
		public ULGPUDriverCreateRenderBufferCallback CreateRenderBuffer;
		public ULGPUDriverDestroyRenderBufferCallback DestroyRenderBuffer;
		public ULGPUDriverNextGeometryIdCallback NextGeometryId;
		public ULGPUDriverCreateGeometryCallback CreateGeometry;
		public ULGPUDriverUpdateGeometryCallback UpdateGeometry;
		public ULGPUDriverDestroyGeometryCallback DestroyGeometry;
		public ULGPUDriverUpdateCommandListCallback UpdateCommandList;
	}

	/// <summary>
	/// ULGPUDriver with delegate* types
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct _ULGPUDriver
	{
		public delegate* unmanaged[Cdecl]<void> BeginSynchronize;
		public delegate* unmanaged[Cdecl]<void> EndSynchronize;
		public delegate* unmanaged[Cdecl]<uint> NextTextureId;
		public delegate* unmanaged[Cdecl]<uint, void*, void> CreateTexture;
		public delegate* unmanaged[Cdecl]<uint, void*, void> UpdateTexture;
		public delegate* unmanaged[Cdecl]<uint, void> DestroyTexture;
		public delegate* unmanaged[Cdecl]<uint> NextRenderBufferId;
		public delegate* unmanaged[Cdecl]<uint, ULRenderBuffer, void> CreateRenderBuffer;
		public delegate* unmanaged[Cdecl]<uint, void> DestroyRenderBuffer;
		public delegate* unmanaged[Cdecl]<uint> NextGeometryId;
		public delegate* unmanaged[Cdecl]<uint, ULVertexBuffer, ULIndexBuffer, void> CreateGeometry;
		public delegate* unmanaged[Cdecl]<uint, ULVertexBuffer, ULIndexBuffer, void> UpdateGeometry;
		public delegate* unmanaged[Cdecl]<uint, void> DestroyGeometry;
		public delegate* unmanaged[Cdecl]<ULCommandList, void> UpdateCommandList;
	}
}
