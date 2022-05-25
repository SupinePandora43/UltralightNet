using System.Runtime.InteropServices;

namespace UltralightNet;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate ULBuffer* ULFileSystemOpenFileCallback__PInvoke__(ULString* path);
public delegate byte[]? ULFileSystemOpenFileCallback(in string path);
