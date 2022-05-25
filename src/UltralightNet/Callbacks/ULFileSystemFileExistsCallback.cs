using System.Runtime.InteropServices;

namespace UltralightNet;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate byte ULFileSystemFileExistsCallback__PInvoke__(ULString* path);
public delegate bool ULFileSystemFileExistsCallback(in string path);
