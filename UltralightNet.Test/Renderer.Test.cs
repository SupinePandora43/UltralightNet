using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UltralightNet.AppCore;
using Xunit;
using Xunit.Abstractions;

namespace UltralightNet.Test
{
	public class RendererTest
	{
		private Renderer renderer;

		private readonly ULViewConfig viewConfig = new()
		{
			IsAccelerated = false,
			IsTransparent = false
		};

		private readonly Dictionary<int, FileStream> handles = new();

		private bool getFileSize(int handle, out long size)
		{
			Console.WriteLine($"get_file_size({handle})");
			//size = "<html><body><p>123</p></body></html>".Length;
			size = handles[handle].Length;
			return true;
		}
		private bool getFileMimeType(string path, out string result)
		{
			Console.WriteLine($"get_file_mime_type({path})");
			result = "text/html";
			return true;
		}
		private long readFromFile(int handle, Span<byte> data, long length)
		{
			Console.WriteLine($"readFromFile({handle}, Span<byte> data, {length})");
			//Assert.Equal("<html><body><p>123</p></body></html>".Length, length);
			//data = "<html><body><p>123</p></body></html>";
			return handles[handle].Read(data);
		}
		[Fact]
		public void TestRenderer()
		{
			//return;
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return;
			AppCoreMethods.ulEnablePlatformFontLoader();
			AppCoreMethods.ulEnablePlatformFileSystem("./");
			ULPlatform.Logger = new ULLogger()
			{
				LogMessage = (level, message) =>
				{
					Console.WriteLine(message);
				}
			};/*
			AppCoreMethods.ulEnableDefaultLogger("./ullog.txt");
			*/

			WebRequest request = WebRequest.CreateHttp("https://raw.githubusercontent.com/SupinePandora43/UltralightNet/gh-pages/index.html");
			WebResponse response = request.GetResponse();



			File.WriteAllText("test.html", new StreamReader(response.GetResponseStream()).ReadToEnd(), Encoding.UTF8);

			ULFileSystemGetFileSizeCallback get_file_size = getFileSize;
			ULFileSystemGetFileMimeTypeCallback get_file_mime_type = getFileMimeType;
			ULFileSystemReadFromFileCallback read_from_file = readFromFile;
			/*ULPlatform.FileSystem = new ULFileSystem()
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
			};*/

			ULConfig config = new();
			renderer = ULPlatform.CreateRenderer(config);

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
			Session session = renderer.DefaultSession;
			Assert.Equal("default", session.Name);

			session = renderer.CreateSession(false, "myses1");
			Assert.Equal("myses1", session.Name);
			Assert.False(session.IsPersistent);

			session = renderer.CreateSession(true, "myses2");
			Assert.Equal("myses2", session.Name);
			Assert.True(session.IsPersistent);
		}

		private void GenericTest()
		{
			View view = renderer.CreateView(512, 512, viewConfig);

			Assert.Equal(512u, view.Width);
			Assert.Equal(512u, view.Height);

			view.URL = "https://github.com/";

			bool OnChangeTitle = false;
			bool OnChangeURL = false;

			view.OnChangeTitle += (title) =>
			{
				Assert.Contains("GitHub", title);
				OnChangeTitle = true;
			};

			view.OnChangeURL += (url) =>
			{
				Assert.Equal("https://github.com/", url);
				OnChangeURL = true;
			};

			while (view.URL == "")
			{
				renderer.Update();
				Thread.Sleep(100);
			}

			renderer.Render();

			Assert.Equal("https://github.com/", view.URL);
			Assert.Contains("GitHub", view.Title);
			Assert.True(OnChangeTitle);
			Assert.True(OnChangeURL);
		}

		private void JSTest()
		{
			View view = renderer.CreateView(2, 2, viewConfig);
			Assert.Equal("3", view.EvaluateScript("1+2", out string exception));
			Assert.True(string.IsNullOrEmpty(exception));
			view.Dispose();
		}

		private void HTMLTest()
		{
			View view = renderer.CreateView(512, 512, viewConfig);
			view.HTML = "<html />";
			view.Dispose();
		}

		private void FSTest()
		{
			View view = renderer.CreateView(256, 256, viewConfig);
			view.URL = "file:///test.html";

			view.OnAddConsoleMessage += (source, level, message, line_number, column_number, source_id) =>
			{
				Console.WriteLine($"{source_id} {level}: {line_number}, {column_number} ({source}) - {message}");
			};

			bool loaded = false;

			view.OnFinishLoading += (frame_id, is_main_frame, url) =>
			{
				loaded = true;
			};

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
			View view = renderer.CreateView(256, 256, viewConfig);
			Console.WriteLine("KeyEvent");
			view.FireKeyEvent(new(ULKeyEventType.Char, ULKeyEventModifiers.ShiftKey, 0, 0, "A", "A", false, false, false));
			Console.WriteLine("MouseEvent");
			view.FireMouseEvent(new(ULMouseEvent.ULMouseEventType.MouseDown, 100, 100, ULMouseEvent.Button.Left));
			Console.WriteLine("ScrollEvent");
			view.FireScrollEvent(new() { type = ULScrollEvent.ScrollType.ByPage, deltaX = 23, deltaY = 123 });
		}

		private void MemoryTest()
		{
			Console.WriteLine("LogMemoryUsage");
			renderer.LogMemoryUsage();
			Console.WriteLine("PurgeMemory");
			renderer.PurgeMemory();
			Console.WriteLine("LogMemoryUsage");
			renderer.LogMemoryUsage();
		}
	}
}
