using Xunit;

namespace UltralightNet.Test
{
	public class ULConfigTest
	{
		private readonly ULConfig config = new();
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
		//[Fact]
		public void EnableImagesTest()
		{
			config.EnableImages = true;
			Assert.True(config.EnableImages);
			config.EnableImages = false;
			Assert.False(config.EnableImages);
		}
		//[Fact]
		public void EnableJavaScriptTest()
		{
			config.EnableJavaScript = true;
			Assert.True(config.EnableJavaScript);
			config.EnableJavaScript = false;
			Assert.False(config.EnableJavaScript);
		}
		//[Fact]
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
	}
}
