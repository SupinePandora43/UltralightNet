using System.Runtime.InteropServices;

namespace UltralightNet;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate bool ULFileSystemFileExistsCallback__PInvoke__(ULString* path);
public delegate bool ULFileSystemFileExistsCallback(string path);
