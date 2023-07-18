using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void ULResizeCallback__PInvoke__(IntPtr userData, void* window, uint width, uint height);

public delegate void ULResizeCallback(IntPtr userData, ULWindow window, uint width, uint height);
