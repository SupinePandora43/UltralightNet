using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Supine.UltralightSharp.Enums;

namespace Supine.UltralightSharp {

  [PublicAPI]
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  public unsafe delegate void ChangeCursorCallback([NativeTypeName("void *")] void* userData, [NativeTypeName("ULView")] View* caller, Cursor cursor);

  namespace Safe {

    [PublicAPI]
    public delegate void ChangeCursorCallback(IntPtr userData, View caller, Cursor cursor);

  }

}