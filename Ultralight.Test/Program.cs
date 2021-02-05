using System;

namespace Ultralight.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			//var cfg = Ultralight.CreateConfig();
			//var str = Ultralight.CreateString("./resources");
			//Ultralight.ConfigSetResourcePath(ref cfg, str);

			//Console.WriteLine(Ultralight.GetVersionStringSafe());
			//Console.WriteLine(Ultralight.GetVersionStringMarshal());

			var cfg = new Config() { ResourcePath = (ULString)"./resources"};
		}
	}
}
