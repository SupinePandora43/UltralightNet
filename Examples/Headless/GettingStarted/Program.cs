using System;
using System.IO;
using System.Threading;
using UltralightNet;
using UltralightNet.AppCore;

namespace GettingStarted
{
	class Program
	{
		static void Main()
		{
			// Set Font Loader
			AppCoreMethods.ulEnablePlatformFontLoader();

			// Set filesystem (Ultralight requires "resources/icudt67l.dat")
			var path = Path.GetDirectoryName(typeof(Program).Assembly.Location);
			AppCoreMethods.ulEnablePlatformFileSystem(path);

			// Create Renderer
			var cfg = new ULConfig();
			Renderer renderer = ULPlatform.CreateRenderer(cfg);

			// Create View
			View view = renderer.CreateView(1980, 1024);

			// Load URL

			bool loaded = false;

			view.OnFinishLoading += (frameId, isMainFrame, url) =>
			{
				loaded = true;
			};

			view.URL = "https://ultralig.ht";

			// Update Renderer until page is loaded
			while (!loaded)
			{
				renderer.Update();
				// give time to process network etc.
				Thread.Sleep(10);
			}

			// Render
			renderer.Render();

			// Get Surface
			ULSurface surface = view.Surface;

			// Get Bitmap
			ULBitmap bitmap = surface.Bitmap;

			// Save bitmap to png file
			bitmap.WritePng(Path.Combine(path, "OUTPUT.png"));
		}
	}
}
