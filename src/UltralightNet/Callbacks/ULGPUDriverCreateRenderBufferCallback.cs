using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULGPUDriverCreateRenderBufferCallback(uint renderBufferId, ULRenderBuffer renderBuffer);
}
