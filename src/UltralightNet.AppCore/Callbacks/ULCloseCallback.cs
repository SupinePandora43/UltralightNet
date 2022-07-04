using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ULCloseCallback__PInvoke__(IntPtr userData, IntPtr window);

public delegate void ULCloseCallback(IntPtr userData, ULWindow window);
