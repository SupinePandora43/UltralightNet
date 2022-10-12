using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate uint ULSurfaceDefinitionGetWidthCallback(void* userData);
}
