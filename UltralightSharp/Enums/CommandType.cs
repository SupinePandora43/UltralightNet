using JetBrains.Annotations;

namespace Supine.UltralightSharp.Enums {

  [PublicAPI]
  [NativeTypeName("ULCommandType")]
  public enum CommandType : byte {

    ClearRenderBuffer = 0,

    DrawGeometry,

  }

}