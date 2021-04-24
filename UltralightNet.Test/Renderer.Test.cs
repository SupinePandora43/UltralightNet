using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UltralightNet.AppCore;
using Xunit;

namespace UltralightNet.Test
{
	public class RendererTest
	{
		private Renderer renderer;

		private Dictionary<int, FileStream> handles = new();

		private bool getFileSize(int handle, out long size)
		{
			Console.WriteLine($"get_file_size({handle})");
			//size = "<html><body><p>123</p></body></html>".Length;
			size = handles[handle].Length;
			return true;
		}
		private bool getFileMimeType(IntPtr path, IntPtr result)
		{
			Console.WriteLine($"get_file_mime_type({ULStringMarshaler.NativeToManaged(path)})");
			Methods.ulStringAssignCString(result, "text/html");
			return true;
		}
		private long readFromFile(int handle, out byte[] data, long length)
		{
			Console.WriteLine($"readFromFile({handle}, out data, {length})");
			//Assert.Equal("<html><body><p>123</p></body></html>".Length, length);
			//data = "<html><body><p>123</p></body></html>";
			data = new byte[length];
			return handles[handle].Read(data, 0, (int)length);
		}
		[Fact]
		public void TestRenderer()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return;
			AppCoreMethods.ulEnablePlatformFontLoader();

			ULPlatform.SetLogger(new()
			{
				LogMessage = (level, message) =>
				{
					Console.WriteLine(message);
				}
			});
			AppCoreMethods.ulEnableDefaultLogger("./ullog.txt");

			WebRequest request = WebRequest.CreateHttp("https://raw.githubusercontent.com/SupinePandora43/UltralightNet/gh-pages/index.html");
			WebResponse response = request.GetResponse();



			File.WriteAllText("test.html", new StreamReader(response.GetResponseStream()).ReadToEnd(), Encoding.UTF8);

			ULFileSystemGetFileSizeCallback get_file_size = getFileSize;
			ULFileSystemGetFileMimeTypeCallback get_file_mime_type = getFileMimeType;
			ULFileSystemReadFromFileCallback read_from_file = readFromFile;
			ULPlatform.SetFileSystem(new()
			{
				FileExists = (path) =>
				{
					Console.WriteLine($"file_exists({path})");
					return File.Exists(path);
				},
				GetFileSize = get_file_size,
				GetFileMimeType = get_file_mime_type,
				OpenFile = (path, open_for_writing) =>
				 {
					 Console.WriteLine($"open_file({path}, {open_for_writing})");
					 int handle = new Random().Next(0, 100);
					 handles[handle] = File.OpenRead(path);
					 return handle;
				 },
				CloseFile = (handle) =>
				 {
					 Console.WriteLine($"close_file({handle})");
				 },
				ReadFromFile = read_from_file
			});

			ULConfig config = new()
			{
				ResourcePath = "./resources"
			};
			renderer = new(config);

			SessionTest();

			GenericTest();

			JSTest();

			HTMLTest();

			FSTest();

			EventTest();

			MemoryTest();


			// ~Renderer() => Dispose()

			Console.WriteLine("Disposing");
			renderer.Dispose();
			Console.WriteLine("Disposed");
		}

		private void SessionTest()
		{
			Session session = Session.DefaultSession(renderer);
			Assert.Equal("default", session.Name);
		}

		private void GenericTest()
		{
			View view = new(renderer, 512, 512, false, Session.DefaultSession(renderer), true);

			Assert.Equal(512u, view.Width);
			Assert.Equal(512u, view.Height);

			view.URL = "https://github.com/";

			view.SetChangeTitleCallback((user_data, caller, title) =>
			{
				Assert.Equal(view.Ptr, caller.Ptr);
				Assert.Contains("GitHub", title);
			});

			view.SetChangeURLCallback((user_data, caller, url) =>
			{
				Assert.Equal(view.Ptr, caller.Ptr);
				Assert.Equal("https://github.com/", url);
			});

			while (view.URL == "")
			{
				renderer.Update();
				Thread.Sleep(100);
			}

			renderer.Render();

			view.SetChangeTitleCallback(null);
			view.SetChangeURLCallback(null);

			Assert.Equal("https://github.com/", view.URL);
			Assert.Contains("GitHub", view.Title);
		}

		private void JSTest()
		{
			View view = new(renderer, 2, 2, false, Session.DefaultSession(renderer), true);
			Assert.Equal("3", view.EvaluateScript("1+2", out string exception));
			Assert.True(string.IsNullOrEmpty(exception));
			view.Dispose();
		}

		private void HTMLTest()
		{
			View view = new(renderer, 512, 512, false, Session.DefaultSession(renderer), true);
			view.HTML = "<html />";
			view.Dispose();
		}

		private void FSTest()
		{
			View view = new(renderer, 256, 256, false, Session.DefaultSession(renderer), true);
			view.URL = "file:///test.html";

			view.SetAddConsoleMessageCallback((user_data, caller, source, level, message, line_number, column_number, source_id) =>
			{
				Console.WriteLine($"{source_id} {level}: {line_number}, {column_number} ({source}) - {message}");
			});

			bool loaded = false;

			view.SetFinishLoadingCallback((user_data, caller, frame_id, is_main_frame, url) =>
			{
				loaded = true;
			});

			while (!loaded)
			{
				renderer.Update();
				Thread.Sleep(10);
			}

			for (uint i = 0; i < 100; i++)
			{
				renderer.Update();
				Thread.Sleep(10);
			}

			renderer.Render();
			view.Surface.Bitmap.WritePng("test_FS.png");
		}

		private void EventTest()
		{
			View view = new(renderer, 256, 256, false, Session.DefaultSession(renderer), true);

			view.FireKeyEvent(new(ULKeyEventType.Char, ULKeyEventModifiers.ShiftKey, 0, 0, "A", "A", false, false, false));
			view.FireMouseEvent(new(ULMouseEvent.ULMouseEventType.MouseDown, 100, 100, ULMouseEvent.Button.Left));
			view.FireScrollEvent(new() { type = ULScrollEvent.Type.ByPage, deltaX = 23, deltaY = 123 });
		}

		private void MemoryTest()
		{
			renderer.LogMemoryUsage();
			renderer.PurgeMemory();
			renderer.LogMemoryUsage();
		}
	}
}
