using System;
using System.Threading.Tasks;
using Xunit;

namespace UltralightNet.Test
{
	public class ULStringTest
	{
		[Theory]
		[InlineData("")]
		[InlineData(null)]
		public void IsEmpty(string value)
		{
			ULString ulString = new(value);
			Assert.True(ulString.IsEmpty());
		}

		[Theory]
		[InlineData(0)]
		[InlineData(100000)]
		public void GetLength(uint strLength)
		{
			Random random = new(0);
			char[] chars = new char[strLength];
			Parallel.For(0, strLength, (index) =>
			{
				chars[index] = (char)random.Next();
			});
			string @string = new(chars);
			ULString ulString = new(@string);
			Assert.Equal(ulString.GetLength(), strLength);
		}
	}
}
