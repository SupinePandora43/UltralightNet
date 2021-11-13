using System;
using JetBrains.Annotations;

namespace Supine.UltralightSharp {

  [PublicAPI]
  public unsafe delegate void PixelBufferWorkerCallback(void* pixels);

  namespace Safe {

    [PublicAPI]
    public delegate void PixelBufferWorkerCallback(IntPtr pixels);

  }

}