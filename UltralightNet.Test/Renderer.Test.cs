using System.Runtime.InteropServices;
using System.Threading;
using UltralightNet.AppCore;
using Xunit;

namespace UltralightNet.Test
{
	public class RendererTest
	{
		private Renderer renderer;
		[Fact]
		public void TestRenderer()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return;
			AppCoreMethods.ulEnablePlatformFontLoader();

			ULConfig config = new()
			{
				ResourcePath = "./resources"
			};
			renderer = new(config, false);

			SessionTest();

			GenericTest();

			JSTest();

			HTMLTest();
		}

		private void SessionTest()
		{
			Session session = Session.DefaultSession(renderer);
			Assert.Equal("default", session.Name);
		}

		private void GenericTest()
		{
			View view = new(renderer, 512, 512, false, Session.DefaultSession(renderer), true);

			Assert.Equal(512u, view.Width);
			Assert.Equal(512u, view.Height);

			view.URL = "https://github.com/";

			view.SetChangeTitleCallback((user_data, caller, title) =>
			{
				Assert.Equal(view.Ptr, caller.Ptr);
				Assert.Contains("GitHub", title);
			});

			view.SetChangeURLCallback((user_data, caller, url) =>
			{
				Assert.Equal(view.Ptr, caller.Ptr);
				Assert.Equal("https://github.com/", url);
			});

			while (view.URL == "")
			{
				renderer.Update();
				Thread.Sleep(100);
			}

			view.SetChangeTitleCallback(null);
			view.SetChangeURLCallback(null);

			Assert.Equal("https://github.com/", view.URL);
			Assert.Contains("GitHub", view.Title);
		}

		private void JSTest()
		{
			View view = new(renderer, 2, 2, false, Session.DefaultSession(renderer), true);
			Assert.Equal("3", view.EvaluateScript("1+2", out string exception));
			Assert.True(string.IsNullOrEmpty(exception));
		}

		private void HTMLTest()
		{
			View view = new(renderer, 512, 512, false, Session.DefaultSession(renderer), true);
			view.HTML = "<html />";
		}
	}
}
