using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ULResizeCallback__PInvoke__(IntPtr userData, IntPtr window, uint width, uint height);

public delegate void ULResizeCallback(IntPtr userData, ULWindow window, uint width, uint height);
