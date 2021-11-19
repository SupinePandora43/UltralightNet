using System.IO;
using System.Text;

namespace UltralightNetTestApplication
{
	class Program
	{
		private static string ToByteString(string path)
		{
			using var file = File.OpenRead(Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "resources", path));

			using var fileStream = new MemoryStream((int)file.Length);
			file.CopyTo(fileStream);

			StringBuilder bytes = new();
			foreach (var b in fileStream.ToArray())
			{
				bytes.Append(b);
				bytes.Append(',');
			}
			bytes.Remove(bytes.Length - 1, 1); // remove last ","

			return bytes.ToString();
		}
		static void Main()
		{
			string generatedFile =
@"using System;

namespace UltralightNet.Resources {
	public static class BinaryResources {
		public static ReadOnlySpan<byte> CompressedCacert => new byte[] {" + ToByteString("cacert.pem") + @"};
		public static ReadOnlySpan<byte> CompressedIcu => new byte[] {" + ToByteString("icudt67l.dat") + @"};
		public static ReadOnlySpan<byte> CompressedMediaControlsCss => new byte[] {" + ToByteString("mediaControls.css") + @"};
		public static ReadOnlySpan<byte> CompressedMediaControlsJs => new byte[] {" + ToByteString("mediaControls.js") + @"};
		public static ReadOnlySpan<byte> CompressedMediaControlsLocalizedStrings => new byte[] {" + ToByteString("mediaControlsLocalizedStrings.js") + @"};
	}
}
";
			File.WriteAllText("generatedBytes.cs", generatedFile);
		}
	}
}
