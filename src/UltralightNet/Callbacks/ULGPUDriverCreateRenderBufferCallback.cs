using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULGPUDriverCreateRenderBufferCallback(uint render_buffer_id, ULRenderBuffer buffer);
}
