using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate IntPtr ULSurfaceDefinitionLockPixelsCallback(IntPtr user_data);
}
