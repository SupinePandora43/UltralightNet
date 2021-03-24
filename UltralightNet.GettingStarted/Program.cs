using System;
using System.Threading;
using UltralightNet;
using UltralightNet.AppCore;

namespace UltralightNet.GettingStarted
{
	class Program
	{
		static void Main(string[] args)
		{
			// Set Logger
			ULPlatform.SetLogger(new ULLogger()
			{
				log_message = (level, message) =>
				{
					Console.WriteLine($"({level}): {message}");
				}
			});

			// Set Font Loader
			AppCoreMethods.ulEnablePlatformFontLoader();

			// Create config, used for specifying resources folder (used for URL loading)
			ULConfig config = new()
			{
				ResourcePath = "./resources"
			};

			// Create Renderer
			Renderer renderer = new(config);

			// Create View
			// Session.DefaultSession - get default renderer session (used for saving cookies etc)
			View view = new(renderer, 512, 512, false, Session.DefaultSession(renderer), false);

			// Load URL

			bool loaded = false;

			view.SetFinishLoadingCallback((user_data, caller, frame_id, is_main_frame, url) =>
			{
				loaded = true;
			});

			view.URL = "https://github.com";

			// Update Renderer until page is loaded
			while (!loaded)
			{
				renderer.Update();
				// sleep | give ultralight time to process network etc.
				Thread.Sleep(10);
			}

			// Render
			renderer.Render();

			// Get Surface
			ULSurface surface = view.Surface;

			// Get Bitmap
			ULBitmap bitmap = surface.Bitmap;

			// Swap Red and Blue channels
			bitmap.SwapRedBlueChannels();

			// save bitmap to png file
			bitmap.WritePng("./github.png");
		}
	}
}
