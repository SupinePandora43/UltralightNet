using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Supine.UltralightSharp {

  [PublicAPI]
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  [return: NativeTypeName("unsigned int")]
  public unsafe delegate uint SurfaceDefinitionGetRowBytesCallback([NativeTypeName("void *")] void* userData);

  namespace Safe {

    [PublicAPI]
    public delegate uint SurfaceDefinitionGetRowBytesCallback(IntPtr userData);

  }

}