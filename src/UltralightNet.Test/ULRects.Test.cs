using UltralightNet;
using Xunit;

public class ULRectsTests
{
	[Fact]
	public void EqualityF()
	{
		ULRect rect1 = new() { Left = 99, Top = 123, Right = 1215623, Bottom = -12.63f };
		ULRect rect2 = new() { Left = 99, Top = 123, Right = 1215623, Bottom = -12.63f };

		Assert.Equal(rect1, rect2);
		Assert.True(rect1 == rect2);
		Assert.False(rect1 != rect2);
		Assert.True(rect1.Equals(rect2));
	}
	[Fact]
	public void EqualityI()
	{
		ULIntRect rect1 = new() { Left = 99, Top = 123, Right = 1215623, Bottom = -12 };
		ULIntRect rect2 = new() { Left = 99, Top = 123, Right = 1215623, Bottom = -12 };

		Assert.Equal(rect1, rect2);
		Assert.True(rect1 == rect2);
		Assert.False(rect1 != rect2);
		Assert.True(rect1.Equals(rect2));
	}
	[Fact]
	public void ConversionToInt()
	{
		ULRect rect = new() { Left = -10, Top = 10, Right = 17, Bottom = 20 };
		ULIntRect iRect = (ULIntRect)rect;
		Assert.Equal(-10, iRect.Left);
		Assert.Equal(10, iRect.Top);
		Assert.Equal(17, iRect.Right);
		Assert.Equal(20, iRect.Bottom);
	}
	[Fact]
	public void ConversionToFloat()
	{
		ULIntRect iRect = new() { Left = -10, Top = 10, Right = 17, Bottom = 20 };
		ULRect rect = (ULRect)iRect;
		Assert.Equal(-10, rect.Left);
		Assert.Equal(10, rect.Top);
		Assert.Equal(17, rect.Right);
		Assert.Equal(20, rect.Bottom);
	}
}
