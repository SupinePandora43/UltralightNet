using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
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

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct _ULSurfaceDefinition
	{
		public delegate* unmanaged[Cdecl]<uint, uint, void*> Create;
		public delegate* unmanaged[Cdecl]<void*, void> Destroy;
		public delegate* unmanaged[Cdecl]<void*, uint> GetWidth;
		public delegate* unmanaged[Cdecl]<void*, uint> GetHeight;
		public delegate* unmanaged[Cdecl]<void*, uint> GetRowBytes;
		public delegate* unmanaged[Cdecl]<void*, nuint> GetSize;
		public delegate* unmanaged[Cdecl]<void*, void*> LockPixels;
		public delegate* unmanaged[Cdecl]<void*, void> UnlockPixels;
		public delegate* unmanaged[Cdecl]<void*, uint, uint, void> Resize;
	}
}
