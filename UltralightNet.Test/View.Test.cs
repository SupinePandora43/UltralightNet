using Xunit;

namespace UltralightNet.Test
{
	public class ViewTest
	{
		private readonly View view;

		static ViewTest()
		{
			AppCore.AppCore.EnablePlatformFontLoader();
		}

		public ViewTest()
		{
			view = new View(RendererTest.renderer, 512, 512, false, Session.DefaultSession(RendererTest.renderer), true);
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
