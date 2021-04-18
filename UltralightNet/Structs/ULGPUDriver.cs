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
}
