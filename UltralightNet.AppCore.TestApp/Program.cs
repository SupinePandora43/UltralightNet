using System;
using System.Threading;

namespace UltralightNet.AppCore.TestApp
{
	class Program
	{
		static void Main()
		{
			AppCoreMethods.ulEnableDefaultLogger("./log.txt");

			ULApp app = new(new ULSettings()
			{
				ForceCPURenderer = true
			}, new ULConfig()
			{
				ResourcePath = "./resources",
				UseGpu = false
			});

			ULWindow window = new(app.MainMonitor, 512, 512, false, ULWindowFlags.Titled | ULWindowFlags.Maximizable);

			window.Title = "test title";

			app.Window = window;

			ULOverlay overlay = new(window, 512, 512, 0, 0);

			View view = overlay.View;
			//view.URL = "https://github.com/SupinePandora43/UltralightNet";

			view.SetFailLoadingCallback(
				(user_data, caller, frame_id, is_main_frame, url, description, error_domain, error_code) =>
					throw new Exception("Failed loading")
			);

			view.SetFinishLoadingCallback((user_data, caller, frame_id, is_main_frame, url) => Console.WriteLine("loaded"));

			view.HTML = "<html><body><p>123</p></body></html>";

			ulong counter = 0;

			app.SetUpdateCallback((_) =>
			{
				counter++;

				//if (counter >= 1000) app.Quit();
			});

			app.Run();
		}
	}
}
