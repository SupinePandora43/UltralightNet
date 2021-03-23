using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate uint ULSurfaceDefinitionGetWidthCallback(IntPtr user_data);
}
