using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULSurfaceDefinitionResizeCallback(void* userData, uint width, uint height);
}
