using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	/// <returns>user_data</returns>
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate IntPtr ULSurfaceDefinitionCreateCallback(uint width, uint height);
}
