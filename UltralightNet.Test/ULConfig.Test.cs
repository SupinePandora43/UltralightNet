using System;
using Xunit;

namespace UltralightNet.Test
{
	public class ULConfigTest
	{
		private ULConfig config = new();

		[Fact]
		public void ResourcePathTest()
		{
			config.ResourcePath = "./resources";
			Assert.Equal("./resources", config.ResourcePath);
		}
		[Fact]
		public void CachePathTest()
		{
			config.CachePath = "./cache";
			Assert.Equal("./cache", config.CachePath);
		}
		[Fact]
		public void UseGpuTest()
		{
			config.UseGpu = true;
			Assert.True(config.UseGpu);
			config.UseGpu = false;
			Assert.False(config.UseGpu);
		}
		[Fact]
		public void DeviceScaleTest()
		{
			config.DeviceScale = 1.0;
			Assert.Equal(1.0, config.DeviceScale);
			config.DeviceScale = 2.0;
			Assert.Equal(2.0, config.DeviceScale);
		}
		[Fact]
		public void FaceWindingTest()
		{
			config.FaceWinding = ULFaceWinding.Clockwise;
			Assert.Equal(ULFaceWinding.Clockwise, config.FaceWinding);
			config.FaceWinding = ULFaceWinding.CounterClockwise;
			Assert.Equal(ULFaceWinding.CounterClockwise, config.FaceWinding);
		}
		[Fact]
		public void EnableImagesTest()
		{
			config.EnableImages = true;
			Assert.True(config.EnableImages);
			config.EnableImages = false;
			Assert.False(config.EnableImages);
		}
		[Fact]
		public void EnableJavaScriptTest()
		{
			config.EnableJavaScript = true;
			Assert.True(config.EnableJavaScript);
			config.EnableJavaScript = false;
			Assert.False(config.EnableJavaScript);
		}
		[Fact]
		public void FontHintingTest()
		{
			config.FontHinting = ULFontHinting.Smooth;
			Assert.Equal(ULFontHinting.Smooth, config.FontHinting);
			config.FontHinting = ULFontHinting.Normal;
			Assert.Equal(ULFontHinting.Normal, config.FontHinting);
			config.FontHinting = ULFontHinting.Monochrome;
			Assert.Equal(ULFontHinting.Monochrome, config.FontHinting);
		}
		[Fact]
		public void FontGammaTest()
		{
			config.FontGamma = 1.8;
			Assert.Equal(1.8, config.FontGamma);
			config.FontGamma = 2.2;
			Assert.Equal(2.2, config.FontGamma);
		}
		[Fact]
		public void FontFamilyStandardTest()
		{
			config.FontFamilyStandard = "Times New Roman";
			Assert.Equal("Times New Roman", config.FontFamilyStandard);
			config.FontFamilyStandard = "Not Times New Roman";
			Assert.Equal("Not Times New Roman", config.FontFamilyStandard);
		}
		[Fact]
		public void FontFamilyFixedTest()
		{
			config.FontFamilyFixed = "Courier New";
			Assert.Equal("Courier New", config.FontFamilyFixed);
			config.FontFamilyFixed = "Not Courier New";
			Assert.Equal("Not Courier New", config.FontFamilyFixed);
		}
		[Fact]
		public void FontFamilySerifTest()
		{
			config.FontFamilySerif = "Times New Roman";
			Assert.Equal("Times New Roman", config.FontFamilySerif);
			config.FontFamilySerif = "Not Times New Roman";
			Assert.Equal("Not Times New Roman", config.FontFamilySerif);
		}
		[Fact]
		public void FontFamilySansSerifTest()
		{
			config.FontFamilySansSerif = "Arial";
			Assert.Equal("Arial", config.FontFamilySansSerif);
			config.FontFamilySansSerif = "Not Arial";
			Assert.Equal("Not Arial", config.FontFamilySansSerif);
		}
		[Fact]
		public void UserAgentTest()
		{
			config.UserAgent = "Chad Ultralight";
			Assert.Equal("Chad Ultralight", config.UserAgent);
			config.UserAgent = "Silly chrome";
			Assert.Equal("Silly chrome", config.UserAgent);
		}
		[Fact]
		public void UserStylesheetTest()
		{
			config.UserStylesheet = "style";
			Assert.Equal("style", config.UserStylesheet);
			config.UserStylesheet = "not style";
			Assert.Equal("not style", config.UserStylesheet);
		}
		[Fact]
		public void ForceRepaintTest()
		{
			config.ForceRepaint = true;
			Assert.True(config.ForceRepaint);
			config.ForceRepaint = false;
			Assert.False(config.ForceRepaint);
		}
		[Fact]
		public void AnimationTimerDelayTest()
		{
			config.AnimationTimerDelay = 1.0 / 60.0;
			Assert.Equal(1.0 / 60.0, config.AnimationTimerDelay);
			config.AnimationTimerDelay = 1.0 / 120.0;
			Assert.Equal(1.0 / 120.0, config.AnimationTimerDelay);
		}
		[Fact]
		public void ScrollTimerDelayTest()
		{
			config.ScrollTimerDelay = 1.0 / 60.0;
			Assert.Equal(1.0 / 60.0, config.ScrollTimerDelay);
			config.ScrollTimerDelay = 1.0 / 120.0;
			Assert.Equal(1.0 / 120.0, config.ScrollTimerDelay);
		}
		[Fact]
		public void RecycleDelayTest()
		{
			config.RecycleDelay = 4.0;
			Assert.Equal(4.0, config.RecycleDelay);
			config.RecycleDelay = 1.0;
			Assert.Equal(1.0, config.RecycleDelay);
		}
		[Fact]
		public void MemoryCacheSizeTest()
		{
			config.MemoryCacheSize = 64u * 1024u * 1024u;
			Assert.Equal(64u * 1024u * 1024u, config.MemoryCacheSize);
			config.MemoryCacheSize = 128u * 1024u * 1024u;
			Assert.Equal(128u * 1024u * 1024u, config.MemoryCacheSize);
		}
		[Fact]
		public void PageCacheSizeTest()
		{
			config.PageCacheSize = 0u;
			Assert.Equal(0u, config.PageCacheSize);
			config.PageCacheSize = 5u;
			Assert.Equal(5u, config.PageCacheSize);
		}
		[Fact]
		public void OverrideRAMSize()
		{
			config.OverrideRAMSize = 0u;
			Assert.Equal(0u, config.OverrideRAMSize);
			config.OverrideRAMSize = 1024u;
			Assert.Equal(1024u, config.OverrideRAMSize);
		}
		[Fact]
		public void MinLargeHeapSizeTest()
		{
			config.MinLargeHeapSize = 32u * 1024u * 1024u;
			Assert.Equal(32u * 1024u * 1024u, config.MinLargeHeapSize);
			config.MinLargeHeapSize = 8u * 1024u * 1024u;
			Assert.Equal(8u * 1024u * 1024u, config.MinLargeHeapSize);
		}
		[Fact]
		public void MinSmallHeapSizeTest()
		{
			config.MinSmallHeapSize = 1u * 1024u * 1024u;
			Assert.Equal(1u * 1024u * 1024u, config.MinSmallHeapSize);
			config.MinSmallHeapSize = 512u * 1024u;
			Assert.Equal(512u * 1024u, config.MinSmallHeapSize);
		}
		[Fact]
		public void DisposeTest()
		{
			config.Dispose();
			config.Dispose();
		}
		[Fact]
		public void FinalizeTest()
		{
			config = null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}
	}
}
