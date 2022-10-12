using System.Runtime.InteropServices;

namespace UltralightNet
{
	/// <returns>userData</returns>
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate void* ULSurfaceDefinitionCreateCallback(uint width, uint height);
}
