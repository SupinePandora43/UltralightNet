using System;
using System.IO;
using UltralightNet.AppCore;
using Xunit;

namespace UltralightNet.Test;

public sealed class RendererFixture : IDisposable
{
	public Renderer Renderer { get; private set; }

	public RendererFixture()
	{
		string path = Path.GetDirectoryName(typeof(RendererFixture).Assembly.Location) ?? "./";

		AppCoreMethods.ulEnablePlatformFontLoader();
		//AppCoreMethods.ulEnablePlatformFileSystem(path);
		//AppCoreMethods.ulEnableDefaultLogger(Path.Combine(path, "./ullog.txt"));

		Renderer = ULPlatform.CreateRenderer();
	}

	public void Dispose() => Renderer.Dispose(); // we're good boys
}

[CollectionDefinition("Renderer", DisableParallelization = true)]
public class RendererCollection : ICollectionFixture<RendererFixture> { }
