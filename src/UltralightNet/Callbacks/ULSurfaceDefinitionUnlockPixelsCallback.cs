using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void ULSurfaceDefinitionUnlockPixelsCallback(void* userData);
}
