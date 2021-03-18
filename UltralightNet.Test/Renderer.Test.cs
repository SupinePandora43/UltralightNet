using System;
using System.IO;
using Xunit;

namespace UltralightNet.Test
{
	public class RendererTest
	{
		internal static ULConfig config;
		internal static Renderer renderer;
		
		static RendererTest()
		{
			AppCore.AppCore.EnablePlatformFontLoader();
			config = new();
			renderer = new(config);
		}

		[Fact]
		public void RendererEmptyTest()
		{
			renderer.Update();
			renderer.Render();
		}

		[Fact]
		public void RendererPurgeMemoryTest()
		{
			renderer.PurgeMemory();
		}

		[Fact]
		public void RendererLogMemoryUsageTest()
		{
			renderer.LogMemoryUsage();
		}

		[Fact]
		public void RendererDisposeTest()
		{
			ULConfig config = new();
			Renderer renderer = new(config);

			Assert.False(renderer.IsDisposed);
			renderer.Dispose();
			Assert.True(renderer.IsDisposed);
		}

		[Fact]
		public void RendererFromIntPtrTest()
		{
			IntPtr ptr = Methods.ulCreateRenderer(new ULConfig().Ptr);
			Renderer renderer = new(ptr);

			Assert.Equal(ptr, renderer.Ptr);
		}

#pragma warning disable IDE0059 // Unnecessary assignment of a value
		[Fact]
		[Trait("Priority", "last")]
		public void RendererFinalizeTest()
		{
			ULConfig config = new();
			Renderer renderer = new(config);

			renderer = null;
			GC.WaitForPendingFinalizers();
		}
#pragma warning restore IDE0059 // Unnecessary assignment of a value

		[Fact]
		public void ViewWidthTest()
		{
			View view = new(renderer, 512, 512, false, Session.DefaultSession(renderer), true);
			Assert.Equal(512u, view.Width);
		}

		[Fact]
		public void ViewHeightTest()
		{
			View view = new(renderer, 512, 512, false, Session.DefaultSession(renderer), true);
			Assert.Equal(512u, view.Height);
		}

		[Fact]
		public void ViewIsLoadingTest()
		{
			View view = new(renderer, 512, 512, false, Session.DefaultSession(renderer), true);
			Assert.True(view.IsLoading);
		}

		[Fact]
		public void ViewEvaluateScriptTest()
		{
			View view = new(renderer, 512, 512, false, Session.DefaultSession(renderer), true);
			ULString result = view.EvaluateScript((ULString)"1+2", out _);
			File.WriteAllText("./test.txt", result.ToString());
		}
	}
}
