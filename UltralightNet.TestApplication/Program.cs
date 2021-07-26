using System.IO;
using UltralightNet;
using UltralightNet.AppCore;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UltralightNetTestApplication
{
	class Program
	{
		static void Main()
		{
			ULPlatform.SetLogger(new ULLogger(){LogMessage=(level, message)=>Console.WriteLine(message)});
			
			ULFileSystem fs = new();

			Dictionary<int, FileStream> files = new();
			int last = 0;

			string dir = Path.GetDirectoryName(typeof(Program).Assembly.Location);
			Console.WriteLine(dir);
			fs.FileExists = (path) =>
			{
				Console.WriteLine($"FileExists {path}");
				return File.Exists(Path.Combine(dir, path));
			};

			fs.OpenFile = (path, for_writing) =>
			{
				Console.WriteLine($"OpenFile {path}");
				FileStream f = File.Open(Path.Combine(dir, path), FileMode.Open, for_writing ? FileAccess.ReadWrite : FileAccess.Read);
				int id = last++;
				files[id] = f;
				return id;
			};

			fs.GetFileSize = (int id, out long result) =>
			{
				Console.WriteLine($"GetFileSize {id}");
				result = files[id].Length;
				return true;
			};

			fs.CloseFile = (id) =>
			{
				Console.WriteLine($"CloseFile {id}");
				files[id].Close();
				files[id] = null;
			};

			fs.GetFileMimeType = (string path, out string result) =>
			{
				Console.WriteLine($"GetFileMimeType {path}");
				result = "";
				return true;
			};

			fs.ReadFromFile = (id, data, length) =>
			{
				Console.WriteLine($"ReadFromFile {id} {data.Length} {length}");
				return (long)files[id].Read(data); ;
			};

			//AppCoreMethods.ulEnablePlatformFileSystem(Path.GetDirectoryName(typeof(Program).Assembly.Location));
			ULPlatform.SetFileSystem(fs);

			AppCoreMethods.ulEnablePlatformFontLoader();

			ULConfig cfg = new();

			Renderer renderer = new(cfg);

			View view = new(renderer, 512, 512);
		}
	}
}
