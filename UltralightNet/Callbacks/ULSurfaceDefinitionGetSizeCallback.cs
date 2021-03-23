using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	// todo: size_t
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate ulong ULSurfaceDefinitionGetSizeCallback(IntPtr user_data);
}
