using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ULUpdateCallback(IntPtr userData);
