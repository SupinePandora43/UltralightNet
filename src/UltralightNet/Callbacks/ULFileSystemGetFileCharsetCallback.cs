using System.Runtime.InteropServices;

namespace UltralightNet;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate ULString* ULFileSystemGetFileCharsetCallback__PInvoke__(ULString* path);
public delegate string ULFileSystemGetFileCharsetCallback(in string path);
