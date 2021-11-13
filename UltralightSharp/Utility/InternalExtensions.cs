using System;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;

namespace Supine.UltralightSharp {

  [PublicAPI]
  internal static class InternalExtensions {

    internal static string GetLocalCodeBaseDirectory(this Assembly asm)
      => Path.GetDirectoryName(asm.Location)
        ?? throw new PlatformNotSupportedException();

    internal static Assembly GetAssembly(this Type type)
      => type.Assembly;

  }

}