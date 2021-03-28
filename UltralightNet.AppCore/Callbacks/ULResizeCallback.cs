using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULResizeCallback(IntPtr user_data, uint width, uint height);
}
