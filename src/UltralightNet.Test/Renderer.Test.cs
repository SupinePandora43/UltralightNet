using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Xunit;

namespace UltralightNet.Test;

[Collection("Renderer")]
[Trait("Category", "Renderer")]
public class RendererTest
{
	private Renderer Renderer;

	public RendererTest(RendererFixture fixture) => Renderer = fixture.Renderer;

	private ULViewConfig ViewConfig => new()
	{
		IsAccelerated = false,
		IsTransparent = false
	};

	// [Fact]
	public void TestRenderer()
	{
		SessionTest();

		GenericTest();

		JSTest();

		HTMLTest();

		// TODO: fix
		// FSTest();

		EventTest();

		MemoryTest();
	}

	[Fact]
	private void SessionTest()
	{
		using Session session = Renderer.DefaultSession;
		Assert.Equal("default", session.Name);

		using Session session1 = Renderer.CreateSession(false, "myses1");
		Assert.Equal("myses1", session1.Name);
		Assert.False(session1.IsPersistent);

		using Session session2 = Renderer.CreateSession(true, "myses2");
		Assert.Equal("myses2", session2.Name);
		Assert.True(session2.IsPersistent);
	}

	[Fact]
	[Trait("Network", "Required")]
	[Trait("Category", "Renderer")]
	private void GenericTest()
	{
		CancellationToken token = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;

		using View view = Renderer.CreateView(512, 512, ViewConfig);

		Assert.Equal(512u, view.Width);
		Assert.Equal(512u, view.Height);

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

		view.URL = "https://github.com/";

		while (view.URL == "")
		{
			if (token.IsCancellationRequested) throw new TimeoutException("Couldn't load page in 10 seconds.");

			Renderer.Update();
			Thread.Sleep(100);
		}

		Renderer.Render();

		Assert.Equal("https://github.com/", view.URL);
		Assert.Contains("GitHub", view.Title);
		Assert.True(OnChangeTitle);
		Assert.True(OnChangeURL);
	}

	[Fact]
	private void JSTest()
	{
		using View view = Renderer.CreateView(2, 2, ViewConfig);
		Console.WriteLine("EVal");
		Assert.Equal("3", view.EvaluateScript("1+2", out string exception));
		Assert.True(string.IsNullOrEmpty(exception));
	}

	[Fact]
	private void HTMLTest()
	{
		using View view = Renderer.CreateView(512, 512, ViewConfig);
		view.HTML = "<html />";
	}

	private void FSTest()
	{
		using View view = Renderer.CreateView(256, 256, ViewConfig);
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
			Renderer.Update();
			Thread.Sleep(10);
		}

		for (uint i = 0; i < 100; i++)
		{
			Renderer.Update();
			Thread.Sleep(10);
		}

		Renderer.Render();
		view.Surface!.Bitmap.WritePng("test_FS.png");
	}

	[Fact]
	public void EventTest()
	{
		using View view = Renderer.CreateView(256, 256, ViewConfig);
		Console.WriteLine("KeyEvent");
		view.FireKeyEvent(new(ULKeyEventType.Char, ULKeyEventModifiers.ShiftKey, 0, 0, "A", "A", false, false, false));
		Console.WriteLine("MouseEvent");
		view.FireMouseEvent(new ULMouseEvent() { Type = ULMouseEventType.MouseDown, X = 100, Y = 100, Button = ULMouseEventButton.Left });
		Console.WriteLine("ScrollEvent");
		view.FireScrollEvent(new() { Type = ULScrollEventType.ByPage, DeltaX = 23, DeltaY = 123 });
	}

	[Fact]
	public void MemoryTest()
	{
		Renderer.LogMemoryUsage();
		Renderer.PurgeMemory();
		Renderer.LogMemoryUsage();
	}

	/*[Fact]
	public unsafe void Gio()
	{
		nint gio = default;
		try
		{
			gio = NativeLibrary.Load(OperatingSystem.IsWindows() ? "gio-2.0-0.dll" : "gio-2.0");

			delegate* unmanaged[Cdecl]<byte*, byte*, nuint, int*, byte*> g_content_type_guess = (delegate* unmanaged[Cdecl]<byte*, byte*, nuint, int*, byte*>)NativeLibrary.GetExport(gio, "g_content_type_guess");
			delegate* unmanaged[Cdecl]<byte*, byte*> g_content_type_get_mime_type = (delegate* unmanaged[Cdecl]<byte*, byte*>)NativeLibrary.GetExport(gio, "g_content_type_get_mime_type");

			ULString* file = new ULString("resources/mediaControls.css".AsSpan()).Allocate();

			/*Stream? s = file->ToString() switch
			{
				"resources/cacert.pem" => Resources.Cacertpem,
				"resources/icudt67l.dat" => Resources.Icudt67ldat,
				"resources/mediaControls.css" => Resources.MediaControlscss,
				"resources/mediaControls.js" => Resources.MediaControlsjs,
				"resources/mediaControlsLocalizedStrings.js" => Resources.MediaControlsLocalizedStringsjs,
				_ => null
			};

			Stream? s = Resources.MediaControlscss;

			if (s is not UnmanagedMemoryStream unmanagedMemoryStream) throw new FileNotFoundException();

			//fixed (char* p = "resources/mediaControlsLocalizedStrings.js")
			int resultUncertain = 100;
			byte* content_type = g_content_type_guess(file->data, (byte*)unmanagedMemoryStream.PositionPointer, checked((nuint)unmanagedMemoryStream.Length), &resultUncertain);
			if (content_type is null) throw new FormatException();
			byte* mime = g_content_type_get_mime_type(content_type);
			// NativeMemory.Free(content_type);

			var str = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(mime));

			GC.KeepAlive(unmanagedMemoryStream);
		}
		finally
		{
			if (gio is not 0) NativeLibrary.Free(gio);
		}
	}*/
}
