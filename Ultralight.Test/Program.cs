namespace Ultralight.Test
{
	public static class Program
	{
		static void Main(string[] args)
		{
			//var cfg = Ultralight.CreateConfig();
			//var str = Ultralight.CreateString("./resources");
			//Ultralight.ConfigSetResourcePath(ref cfg, str);

			//Console.WriteLine(Ultralight.GetVersionStringSafe());
			//Console.WriteLine(Ultralight.GetVersionStringMarshal());

			Config cfg = new()
			{
				ResourcePath = (ULString)"./resources"
			};

			Renderer renderer = new(cfg);
		}
	}
}
