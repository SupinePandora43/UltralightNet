using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace UltralightNet.Generator
{
    public static class Program
    {
		private const string UltralightApi = "https://github.com/ultralight-ux/Ultralight-API/archive/master.zip";

		public static void Main(string[] args)
		{
			#region download headers
			Directory.Delete("./Ultralight-API-master", true);
			WebRequest request = WebRequest.CreateHttp(UltralightApi);
			WebResponse response = request.GetResponse();

			Stream responseStream = response.GetResponseStream();
			ZipArchive archive = new ZipArchive(responseStream, ZipArchiveMode.Read);

			archive.ExtractToDirectory("./");
			#endregion
			ClangSharpUltralightGenerator.Generate();
			return;
			ConsoleDriver.Run(new UltralightLibrary());
		}
	}
	internal class UltralightLibrary : ILibrary
	{
		public override void Postprocess(Driver driver, ASTContext ctx)
		{
			
		}

		public override void Preprocess(Driver driver, ASTContext ctx)
		{
			
		}

		public override void Setup(Driver driver)
		{
			DriverOptions options = driver.Options;
			options.GeneratorKind = GeneratorKind.CSharp;

			var module = options.AddModule("Ultralight");
			module.IncludeDirs.Add("./Ultralight-API-master");
			module.Headers.Add("Ultralight/CAPI.h");
			module.Headers.Add("JavaScriptCore/JavaScript.h");
		}

		public override void SetupPasses(Driver driver)
		{
			
		}
	}
}
