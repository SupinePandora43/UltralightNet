using System;

namespace UltralightNet.Test
{
    class Program
    {
        static void Main()
        {
			ULString uLString = new("тестим :D");
			Console.WriteLine(uLString.IsEmpty());
			Console.WriteLine(uLString.GetData());
		}
    }
}
