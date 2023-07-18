using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void ULCloseCallback__PInvoke__(IntPtr userData, void* window);

public delegate void ULCloseCallback(IntPtr userData, ULWindow window);
