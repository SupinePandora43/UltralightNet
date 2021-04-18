using System;
using System.Runtime.InteropServices;
using System.Threading;
using UltralightNet.AppCore;
using Xunit;

namespace UltralightNet.Test
{
	public class RendererTest
	{
		private Renderer renderer;

		private bool getFileSize(int handle, out long size)
		{
			Console.WriteLine($"get_file_size({handle})");
			size = "<html><body><p>text</p></body></html>".Length;
			return true;
		}
		private bool getFileMimeType(string path, out string result)
		{
			Console.WriteLine($"get_file_mime_type({path})");
			result = "text/html";
			return false;
		}
		private long readFromFile(int handle, out string data, long length)
		{
			Console.WriteLine($"readFromFile({handle}, out data, {length})");
			data = "<html><body><p>text</p></body></html>";
			return data.Length;
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
			ULFileSystemGetFileSizeCallback get_file_size = getFileSize;
			ULFileSystemGetFileMimeTypeCallback get_file_mime_type = getFileMimeType;
			ULFileSystemReadFromFileCallback read_from_file = readFromFile;
			ULPlatform.SetFileSystem(new()
			{
				file_exists = (path) => {
					Console.WriteLine($"file_exists({path})");
					return false;
				},
				get_file_size = get_file_size,
				get_file_mime_type = get_file_mime_type,
				open_file = (string path, bool open_for_writing) =>
				{
					Console.WriteLine($"open_file({path}, {open_for_writing})");
					return -1;
				},
				close_file = (handle) =>
				{
					Console.WriteLine($"close_file({handle})");
				},
				read_from_file = read_from_file
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

			renderer.Dispose();
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
			view.URL = "file:///app.html";
			// todo test using document.body.innerHTML
		}

		private void EventTest()
		{
			View view = new(renderer, 256, 256, false, Session.DefaultSession(renderer), true);

			view.FireMouseEvent(new(ULMouseEvent.ULMouseEventType.MouseDown, 100, 100, ULMouseEvent.Button.Left));
		}
	}
}
