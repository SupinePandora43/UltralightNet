using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULResizeCallback__PInvoke__(IntPtr user_data, IntPtr window, uint width, uint height);

	public delegate void ULResizeCallback(IntPtr user_data, ULWindow window, uint width, uint height);
}
