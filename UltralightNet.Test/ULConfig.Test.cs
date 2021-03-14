using Xunit;

namespace UltralightNet.Test
{
	public class ULConfigTest
	{
		[Fact]
		public void ResourcePath()
		{
			ULConfig config = new();
			config.ResourcePath = "./resources";
			Assert.Equal("./resources", config.ResourcePath);
		}
	}
}
