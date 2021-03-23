namespace UltralightNet
{
	public struct ULGPUDriver
	{
		public ULGPUDriverBeginSynchronizeCallback BeginSynchronize;
		public ULGPUDriverEndSynchronizeCallback EndSynchronize;
		public ULGPUDriverNextTextureIdCallback NextTexture_id;
		public ULGPUDriverCreateTextureCallback CreateTexture;
		public ULGPUDriverUpdateTextureCallback UpdateTexture;
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
