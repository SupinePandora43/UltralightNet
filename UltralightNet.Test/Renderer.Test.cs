using System;
using Xunit;

namespace UltralightNet.Test
{
	public class RendererTest
	{
		[Fact]
		public void RendererEmptyTest()
		{
			ULConfig config = new();
			Renderer renderer = new(config, false);

			renderer.Update();
			renderer.Render();
		}

		[Fact]
		public void RendererPurgeMemoryTest()
		{
			ULConfig config = new();
			Renderer renderer = new(config, false);

			renderer.PurgeMemory();
		}

		[Fact]
		public void RendererLogMemoryUsageTest()
		{
			ULConfig config = new();
			Renderer renderer = new(config, false);

			renderer.LogMemoryUsage();
		}

		[Fact]
		public void RendererDisposeTest()
		{
			ULConfig config = new();
			Renderer renderer = new(config, false);

			//Assert.False(renderer.IsDisposed);
			renderer.Dispose();
			//Assert.True(renderer.IsDisposed);
		}

		[Fact]
		public void RendererFromIntPtrTest()
		{
			IntPtr ptr = Methods.ulCreateRenderer(new ULConfig().Ptr);
			Renderer renderer = new(ptr);

			Assert.Equal(ptr, renderer.Ptr);
		}
#pragma warning disable IDE0059 // Unnecessary assignment of a value
		[Fact]
		public void RendererFinalizeTest()
		{
			ULConfig config = new();
			Renderer renderer = new(config, false);

			renderer = null;
			GC.WaitForPendingFinalizers();
		}
#pragma warning restore IDE0059 // Unnecessary assignment of a value
	}
}
