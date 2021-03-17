using System;
using Xunit;

namespace UltralightNet.Test
{
	public class RendererTest
	{
		internal static Renderer renderer;

		static RendererTest()
		{
			ULConfig config = new();
			renderer = new(config, false);
		}

		[Fact]
		public void RendererEmptyTest()
		{
			renderer.Update();
			renderer.Render();
		}

		[Fact]
		public void RendererPurgeMemoryTest()
		{
			renderer.PurgeMemory();
		}

		[Fact]
		public void RendererLogMemoryUsageTest()
		{
			renderer.LogMemoryUsage();
		}

		[Fact]
		public void RendererDisposeTest()
		{
			//Assert.False(renderer.IsDisposed);
			//renderer.Dispose();
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
		/*[Fact]
		public void RendererFinalizeTest()
		{
			ULConfig config = new();
			Renderer renderer = new(config, false);

			renderer = null;
			GC.WaitForPendingFinalizers();
		}*/
#pragma warning restore IDE0059 // Unnecessary assignment of a value
	}
}
