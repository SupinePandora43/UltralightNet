using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULGPUDriverCreateGeometryCallback(uint geometry_id, ULVertexBuffer vertices, ULIndexBuffer indices);
}
