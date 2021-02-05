using System;

namespace Ultralight.Test
{
    class Program
    {
        static void Main(string[] args)
        {
			var cfg = Ultralight.CreateConfig();
			var str = Ultralight.CreateString("./resources");
			Ultralight.ConfigSetResourcePath(cfg, str);
		}
    }
}
