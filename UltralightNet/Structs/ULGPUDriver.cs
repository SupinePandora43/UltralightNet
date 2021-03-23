namespace UltralightNet
{
	public struct ULGPUDriver
	{
		public ULGPUDriverBeginSynchronizeCallback begin_synchronize;
		public ULGPUDriverEndSynchronizeCallback end_synchronize;
		public ULGPUDriverNextTextureIdCallback next_texture_id;
		public ULGPUDriverCreateTextureCallback create_texture;
		public ULGPUDriverUpdateTextureCallback update_texture;
		public ULGPUDriverDestroyTextureCallback destroy_texture;
		public ULGPUDriverNextRenderBufferIdCallback next_render_buffer_id;
		public ULGPUDriverCreateRenderBufferCallback create_render_buffer;
		public ULGPUDriverDestroyRenderBufferCallback destroy_render_buffer;
		public ULGPUDriverNextGeometryIdCallback next_geometry_id;
		public ULGPUDriverCreateGeometryCallback create_geometry;
		public ULGPUDriverUpdateGeometryCallback update_geometry;
		public ULGPUDriverDestroyGeometryCallback destroy_geometry;
		public ULGPUDriverUpdateCommandListCallback update_command_list;
	}
}
