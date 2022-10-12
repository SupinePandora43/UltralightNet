using System;
using System.Runtime.InteropServices;
using UltralightNet.LowStuff;

namespace UltralightNet.AppCore;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ULResizeCallback__PInvoke__(IntPtr userData, Handle<ULWindow> window, uint width, uint height);

public delegate void ULResizeCallback(IntPtr userData, ULWindow window, uint width, uint height);
