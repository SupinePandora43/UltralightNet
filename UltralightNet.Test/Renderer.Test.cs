using Xunit;

namespace UltralightNet.Test
{
	public class RendererTest
	{
		[Fact]
		public void RendererEmptyTest()
		{
			ULConfig config = new();
			Renderer renderer = new(config);

			renderer.Update();
			renderer.Render();
		}

		[Fact]
		public void RendererPurgeMemoryTest()
		{
			ULConfig config = new();
			Renderer renderer = new(config);

			renderer.PurgeMemory();
		}

		[Fact]
		public void RendererLogMemoryUsageTest()
		{
			ULConfig config = new();
			Renderer renderer = new(config);

			renderer.LogMemoryUsage();
		}

		[Fact]
		public void RendererDisposeTest()
		{
			ULConfig config = new();
			Renderer renderer = new(config);

			Assert.False(renderer.IsDisposed);
			renderer.Dispose();
			Assert.True(renderer.IsDisposed);
		}
	}
}
