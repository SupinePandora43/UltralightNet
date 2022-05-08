using System;
using System.IO;
using System.Threading;

namespace UltralightNet.AppCore.TestApp
{
	class Program
	{
		static void Main()
		{
			AppCoreMethods.ulEnableDefaultLogger("./log.txt");
			AppCoreMethods.ulEnablePlatformFileSystem(Path.GetDirectoryName(typeof(Program).Assembly.Location));

			ULApp app = new(new ULSettings(), new ULConfig(){});
			ULWindow window = new(app.MainMonitor, 512, 512, false, ULWindowFlags.Titled | ULWindowFlags.Resizable);
			//ULWindow window1 = new(app.MainMonitor, 512, 512, false, ULWindowFlags.Titled | ULWindowFlags.Resizable);

			window.Title = "test title";

			ULOverlay overlay = new(window, 512, 512, 0, 0);
			//ULOverlay overlay1 = new(window1, 512, 512, 0, 0);

			//overlay1.View.HTML = "<html><body><p>123</p></body></html>";

			View view = overlay.View;
			view.URL = "https://github.com/SupinePandora43/UltralightNet";

			view.OnFailLoading += (frame_id, is_main_frame, url, description, error_domain, error_code) => throw new Exception("Failed loading");

			bool l = false;

			view.OnFinishLoading += (frame_id, is_main_frame, url) => l = true;

			view.HTML = "<html><body><p>123</p></body></html>";
			//view.URL = "https://vk.com/supinepandora43";
			//view.URL = "https://youtube.com";
			//view.URL = "https://twitter.com/@supinepandora43";
			while(!l) {app.Renderer.Update();Thread.Sleep(20);}

			app.Run();

	
		}
	}
}
