using System;
using JetBrains.Annotations;

namespace Supine.UltralightSharp.Enums {

  [PublicAPI]
  [Flags]
  public enum JsClassAttribute : uint {

    None = 0,

    NoAutomaticPrototype = 1 << 1,

  }

}