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
			char[] chars = new char[strLength];
			Parallel.For(0, strLength, index =>
			{
				chars[index] = 'ы';
			});
			string @string = new(chars);
			Assert.Equal(strLength, (uint)@string.Length);
			ULString ulString = new(@string);
			Assert.Equal(strLength, ulString.GetLength());
		}

		[Theory]
		[InlineData(null, null)]
		[InlineData("123456 юникод", null)]
		[InlineData("123456 юникод", "")]
		[InlineData(null, "123456 юникод")]
		[InlineData("", "123456 юникод")]
		[InlineData("123456 юникод", "123456 юникод")]
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
		[InlineData("123456 юникод", null)]
		[InlineData("123456 юникод", "")]
		[InlineData(null, "123456 юникод")]
		[InlineData("123456 юникод", "123456 юникод")]
		public void AssignCStr(string str, string newStr)
		{
			ULString ulString = new(str);

			ulString.Assign(newStr);

			Assert.Equal(newStr ?? "", ulString.GetData());
			Assert.Equal((newStr ?? "") == "", ulString.IsEmpty());
		}

		[Theory]
		[InlineData("")]
		[InlineData("юникод")]
		public void MarshalStruct(string str)
		{
			ULString uLString = new(str);

			Assert.Equal(str, uLString.ULString16.data_);
			Assert.Equal(str.Length, (int)uLString.ULString16.length_);
		}

		[Fact]
		public void CloneTest()
		{
			ULString ulString1 = new("юникод");
			ULString ulString2 = ulString1.Clone() as ULString;

			Assert.Equal(ulString1, ulString2);
			Assert.True(ulString1 == ulString2);
			Assert.True(ulString1.Equals(ulString2));
			Assert.True(ulString1.Equals(ulString2 as object));
			Assert.False(ulString1 != ulString2);
			Assert.Equal(ulString1.ToString(), ulString2.ToString());
			Assert.Equal(ulString1.ULString16, ulString2.ULString16);
		}

		[Fact]
		public void Conversion()
		{
			ULString ulString = new("юникод");

			Assert.Equal("юникод", ulString);
			Assert.Equal((ULString)"юникод", ulString);
		}
	}
}
