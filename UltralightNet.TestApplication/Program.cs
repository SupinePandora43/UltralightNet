using UltralightNet;
using UltralightNet.AppCore;

namespace UltralightNetTestApplication
{
	class Program
	{
		static void Main()
		{
			ULConfig config = new();
			config.ResourcePath = "./resources/";
			config.UseGpu = false;

			Renderer renderer = new(config);

			//Session session = Session.DefaultSession(renderer);
			Session session = new(renderer, false, (ULString)"asd");

			AppCore.EnableDefaultLogger((ULString)"./log.txt");
			AppCore.EnablePlatformFileSystem((ULString)"./");
			AppCore.EnablePlatformFontLoader();

			//Ultralight.View view = new(renderer.Ptr, 512, 512, false, session.Ptr);
			//IntPtr view = ulCreateView(renderer.Ptr, 256, 256, 0, IntPtr.Zero, 0);
			View view = new(renderer, 512, 512, false, session, false);

		}
	}
}
