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
		[InlineData(10)]
		[InlineData(1000)]
		[InlineData(10000)]
		[InlineData(100000)]
		public void GetLength(uint strLength)
		{
			Random random = new(0);
			char[] chars = new char[strLength];
			Parallel.For(0, strLength, index =>
			{
				chars[index] = (char)random.Next(char.MinValue, char.MaxValue);
			});
			string @string = new(chars);
			Assert.Equal(strLength, (uint)@string.Length);
			ULString ulString = new(@string);
			Assert.Equal(strLength, ulString.GetLength());
		}

		[Theory]
		[InlineData(null, null)]
		[InlineData("123456", null)]
		[InlineData(null, "123456")]
		[InlineData("123456", "123456")]
		public void AssignUL(string str, string newStr)
		{
			ULString ulString = new(str);
			ULString newULString = new(newStr);

			ulString.Assign(newULString);

			Assert.Equal(newULString, ulString);
			Assert.Equal(newULString.GetData(), ulString.GetData());
			Assert.Equal(newULString.IsEmpty(), ulString.IsEmpty());
		}

		[Theory]
		[InlineData(null, null)]
		[InlineData("123456", null)]
		[InlineData(null, "123456")]
		[InlineData("123456", "123456")]
		public void AssignCStr(string str, string newStr)
		{
			ULString ulString = new(str);

			ulString.Assign(newStr);

			Assert.Equal(newStr ?? "", ulString.GetData());
			Assert.Equal(string.IsNullOrEmpty(newStr), ulString.IsEmpty());
		}
	}
}
