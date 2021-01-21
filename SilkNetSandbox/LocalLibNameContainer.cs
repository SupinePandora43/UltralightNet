using System;
using System.IO;
using Silk.NET.Core.Loader;

internal abstract class LocalLibNameContainer : SearchPathContainer {

  protected static readonly string AssemblyPath
    = typeof(CustomGlEsLibNameContainer).Assembly.Location;

  protected static readonly string AssemblyDirectory
    = Path.GetDirectoryName(AssemblyPath)!;

}