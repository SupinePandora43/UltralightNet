using System;
using System.Runtime.InteropServices;
using System.Threading;
using Xunit;

namespace UltralightNet.Test
{
	public class RendererTest
	{
		[Fact]
		public void TestRenderer()
		{
			AppCore.AppCore.EnablePlatformFontLoader();

			ULConfig config = new()
			{
				ResourcePath = "./resources"
			};
			Renderer renderer = new(config);

			View view = new View(renderer, 512, 512, false, Session.DefaultSession(renderer), true);

			view.URL = "https://github.com";

			uint att = 0;

			while (view.URL == "")
			{
				renderer.Update();
				Thread.Sleep(100);
				att++;
			}

			Assert.Equal("https://github.com/", view.URL);
		}
	}
}
