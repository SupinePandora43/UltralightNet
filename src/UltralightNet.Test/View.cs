using System;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace UltralightNet.Test;

[Collection("Renderer")]
[Trait("Category", "Renderer")]
public sealed class ViewTest
{
	private Renderer Renderer { get; }
	public ViewTest(RendererFixture fixture) => Renderer = fixture.Renderer;

	[Fact]
	[Trait("Network", "Required")]
	public void NetworkTest()
	{
		using var view = Renderer.CreateView(512, 512);

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

		var sw = Stopwatch.StartNew();

		while (view.URL == "")
		{
			if (sw.Elapsed > TimeSpan.FromSeconds(10)) throw new TimeoutException("Couldn't load page in 10 seconds.");

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
	public void HTML()
	{
		using var view = Renderer.CreateView(512, 512);
		view.HTML = "<html />";
	}
	[Fact]
	public void JSTest()
	{
		using var view = Renderer.CreateView(2, 2);
		Assert.Equal("3", view.EvaluateScript("1+2", out string exception));
		Assert.True(string.IsNullOrEmpty(exception));

		bool called = false;
		view.OnAddConsoleMessage += (_, _, _, _, _, _) => called = true;
		view.EvaluateScript("console.log(123)", out _);

		Assert.True(called);
	}
	[Fact]
	public void EventTest()
	{
		using var view = Renderer.CreateView(256, 256);
		using var keyEvent = ULKeyEvent.Create(ULKeyEventType.Char, ULKeyEventModifiers.ShiftKey, 0, 0, "A", "A", false, false, false);
		view.FireKeyEvent(keyEvent);
		view.FireMouseEvent(new ULMouseEvent() { Type = ULMouseEventType.MouseDown, X = 100, Y = 100, Button = ULMouseEventButton.Left });
		view.FireScrollEvent(new() { Type = ULScrollEventType.ByPage, DeltaX = 23, DeltaY = 123 });
	}
	[Fact]
	public void InspectorView()
	{
		using var view = Renderer.CreateView(256, 256);
		view.OnCreateInspectorView = (bool isLocal, string inspectedUrl) => throw new NotImplementedException(); // TODO

		var inspectorView = view.CreateLocalInspectorView();
		Assert.NotNull(inspectorView);
	}
}
