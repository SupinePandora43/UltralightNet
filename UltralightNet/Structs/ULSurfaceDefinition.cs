namespace UltralightNet
{
	public struct ULSurfaceDefinition
	{
		public ULSurfaceDefinitionCreateCallback Create;
		public ULSurfaceDefinitionDestroyCallback Destroy;
		public ULSurfaceDefinitionGetWidthCallback GetWidth;
		public ULSurfaceDefinitionGetHeightCallback GetHeight;
		public ULSurfaceDefinitionGetRowBytesCallback GetRowBytes;
		public ULSurfaceDefinitionGetSizeCallback GetSize;
		public ULSurfaceDefinitionLockPixelsCallback LockPixels;
		public ULSurfaceDefinitionUnlockPixelsCallback UnlockPixels;
		public ULSurfaceDefinitionResizeCallback Resize;
	}
}
