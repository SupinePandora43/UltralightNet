using Xunit;

namespace UltralightNet.Test
{
	public class ULConfigTest
	{
		[Fact]
		public void ResourcePathTest()
		{
			ULConfig config = new();
			config.ResourcePath = "./resources";
			Assert.Equal("./resources", config.ResourcePath);
		}
		[Fact]
		public void CachePathTest()
		{
			ULConfig config = new();
			config.CachePath = "./cache";
			Assert.Equal("./cache", config.CachePath);
		}
		[Fact]
		public void UseGpuTest()
		{
			ULConfig config = new();
			config.UseGpu = true;
			Assert.True(config.UseGpu);
			config.UseGpu = false;
			Assert.False(config.UseGpu);
		}
		[Fact]
		public void DeviceScaleTest()
		{
			ULConfig config = new();
			config.DeviceScale = 1.0;
			Assert.Equal(1.0, config.DeviceScale);
			config.DeviceScale = 2.0;
			Assert.Equal(2.0, config.DeviceScale);
		}
		[Fact]
		public void FaceWindingTest()
		{
			ULConfig config = new();
			config.FaceWinding = ULFaceWinding.Clockwise;
			Assert.Equal(ULFaceWinding.Clockwise, config.FaceWinding);
			config.FaceWinding = ULFaceWinding.CounterClockwise;
			Assert.Equal(ULFaceWinding.CounterClockwise, config.FaceWinding);
		}
	}
}
