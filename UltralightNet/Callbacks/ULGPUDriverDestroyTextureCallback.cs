using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULGPUDriverDestroyTextureCallback(uint texture_id);
}
