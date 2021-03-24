using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate nuint ULSurfaceDefinitionGetSizeCallback(IntPtr user_data);
}
