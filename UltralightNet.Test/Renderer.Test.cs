using System.Runtime.InteropServices;
using System.Threading;
using UltralightNet.AppCore;
using Xunit;

namespace UltralightNet.Test
{
	public class RendererTest
	{
		[Fact]
		public void TestRenderer()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return;
			AppCoreMethods.ulEnablePlatformFontLoader();

			ULConfig config = new()
			{
				ResourcePath = "./resources"
			};
			Renderer renderer = new(config);


			#region Session
			Session session = Session.DefaultSession(renderer);
			Assert.Equal("default", session.Name);
			#endregion

			View view = new(renderer, 512, 512, false, session, true);

			// Width, Height
			Assert.Equal(512u, view.Width);
			Assert.Equal(512u, view.Height);

			// LoadURL
			view.URL = "https://github.com/";

			while (view.URL == "")
			{
				renderer.Update();
				Thread.Sleep(100);
			}

			// GetURL
			Assert.Equal("https://github.com/", view.URL);

			// EvaluateScript
			Assert.Equal("3", view.EvaluateScript("1+2", out string exception));
			Assert.True(string.IsNullOrEmpty(exception));

		}
	}
}
