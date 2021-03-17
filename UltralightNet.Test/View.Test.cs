using Xunit;

namespace UltralightNet.Test
{
	public class ViewTest
	{
		private readonly Renderer renderer;
		private readonly View view;

		public ViewTest()
		{
			renderer = new(new ULConfig());
			AppCore.AppCore.EnablePlatformFontLoader();
			view = new View(renderer, 512, 512, false, Session.DefaultSession(renderer), true);
		}

		[Fact]
		public void WidthTest()
		{
			Assert.Equal(512u, view.Width);
		}

		[Fact]
		public void HeightTest()
		{
			Assert.Equal(512u, view.Height);
		}
	}
}
