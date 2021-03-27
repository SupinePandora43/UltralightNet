using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ULCloseCallback(IntPtr user_data);
}
