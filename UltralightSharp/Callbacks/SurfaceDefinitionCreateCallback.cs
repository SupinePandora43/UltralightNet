using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Supine.UltralightSharp {

  [PublicAPI]
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  [return: NativeTypeName("void *")]
  public unsafe delegate void* SurfaceDefinitionCreateCallback([NativeTypeName("unsigned int")] uint width, [NativeTypeName("unsigned int")] uint height);

  namespace Safe {

    [PublicAPI]
    public delegate IntPtr SurfaceDefinitionCreateCallback(uint width, uint height);

  }

}