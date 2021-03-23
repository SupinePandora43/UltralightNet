using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate uint ULSurfaceDefinitionGetHeightCallback(IntPtr user_data);
}
