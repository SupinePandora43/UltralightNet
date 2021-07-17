using UltralightNet;
using UltralightNet.AppCore;
using System;
using System.Runtime.InteropServices;

namespace UltralightNetTestApplication
{
	class Program
	{
		static void Main()
		{
			//ULConfig config = new();
			//Renderer renderer = new(config);

			//Session session = Session.DefaultSession(renderer);
			//Session session = new(renderer, false, "asd");

			//AppCoreMethods.ulEnableDefaultLogger("./log.txt");
			//AppCoreMethods.ulEnablePlatformFileSystem("./");
			//AppCoreMethods.ulEnablePlatformFontLoader();

			//Ultralight.View view = new(renderer.Ptr, 512, 512, false, session.Ptr);
			//IntPtr view = ulCreateView(renderer.Ptr, 256, 256, 0, IntPtr.Zero, 0);
			//View view = new(renderer, 512, 512);


			unsafe {
				ULString* str = (ULString*)Methods.ulCreateString("лол");
				Console.WriteLine(Marshal.PtrToStringUni((IntPtr)str->data, (int)str->length));
			}
		}
	}
}
