using System.Runtime.InteropServices;

namespace UltralightNet;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate ULString* ULFileSystemGetFileMimeTypeCallback__PInvoke__(ULString* path);
public delegate string ULFileSystemGetFileMimeTypeCallback(in string path);
