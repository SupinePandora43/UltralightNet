using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	/// <summary>
	/// known as "stride"
	/// </summary>
	/// <example>width * 4</example>
	/// <param name="user_data"></param>
	/// <returns></returns>
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate uint ULSurfaceDefinitionGetRowBytesCallback(IntPtr user_data);
}
