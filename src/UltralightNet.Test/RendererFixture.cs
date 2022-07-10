using System;
using UltralightNet.AppCore;
using Xunit;

namespace UltralightNet.Test;

public class RendererFixture : IDisposable
{
	public Renderer Renderer { get; private set; }

	public RendererFixture()
	{
		AppCoreMethods.ulEnablePlatformFontLoader();
		AppCoreMethods.ulEnablePlatformFileSystem("./");
		AppCoreMethods.ulEnableDefaultLogger("./ullog.txt");

		Renderer = ULPlatform.CreateRenderer();
	}

	public void Dispose()
	{
		Renderer.Dispose();
		Renderer.Dispose();
		Renderer = null!;
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();
	}
}

[CollectionDefinition("Renderer", DisableParallelization = true)]
public class RendererCollection : ICollectionFixture<RendererFixture> { }
