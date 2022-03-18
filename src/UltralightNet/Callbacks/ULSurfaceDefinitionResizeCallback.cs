using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULSurfaceDefinitionResizeCallback(IntPtr user_data, uint width, uint height);
}
