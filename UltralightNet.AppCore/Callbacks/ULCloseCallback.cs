using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULCloseCallback__PInvoke__(IntPtr user_data, IntPtr window);

	public delegate void ULCloseCallback(IntPtr user_data, ULWindow window);
}
