namespace UltralightNet
{
	public struct ULSurfaceDefinition
	{
		public ULSurfaceDefinitionCreateCallback create;
		public ULSurfaceDefinitionDestroyCallback destroy;
		public ULSurfaceDefinitionGetWidthCallback get_width;
		public ULSurfaceDefinitionGetHeightCallback get_height;
		public ULSurfaceDefinitionGetRowBytesCallback get_row_bytes;
		public ULSurfaceDefinitionGetSizeCallback get_size;
		public ULSurfaceDefinitionLockPixelsCallback lock_pixels;
		public ULSurfaceDefinitionUnlockPixelsCallback unlock_pixels;
		public ULSurfaceDefinitionResizeCallback resize;
	}
}
