using System;
using System.Runtime.InteropServices;
using UltralightNet.LowStuff;

namespace UltralightNet.AppCore;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ULCloseCallback__PInvoke__(IntPtr userData, Handle<ULWindow> window);

public delegate void ULCloseCallback(IntPtr userData, ULWindow window);
