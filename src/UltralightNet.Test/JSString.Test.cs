using System.Collections.Generic;
using UltralightNet.JavaScript;

namespace UltralightNet.Test;

[Trait("Category", "JS")]
public unsafe class JSStringTest
{
	const string TestString = "лорум ипсум, что-то там...";
	static ReadOnlySpan<byte> TestStringUTF8 => "лорум ипсум, что-то там...\0"u8;
	public static IEnumerable<object[]> InvalidStrings()
	{
		yield return new object[] { "" };
		yield return new object[] { null! };
	}

	[Fact]
	public void CreateFromCharSpan()
	{
		using var str = JSString.CreateFromUTF16(TestString.AsSpan());
		Assert.Equal(TestString.Length, (int)str.Length);
		Assert.NotEqual((nuint)0, (nuint)str.UTF16DataRaw);
		Assert.True(TestString.AsSpan().SequenceEqual(str.UTF16Data));
		Assert.Equal(TestString, str.ToString());
	}
	[Theory]
	[MemberData(nameof(InvalidStrings))]
	public void CreateFromEmptyCharSpan(string? testString)
	{
		using var str = JSString.CreateFromUTF16(testString.AsSpan());
		Assert.Equal((nuint)0, str.Length);
		Assert.Equal((nuint)0, (nuint)str.UTF16DataRaw);
		Assert.Equal(string.Empty, str.ToString());
	}
	[Fact]
	public void CreateFromByteSpan()
	{
		using var str = JSString.CreateFromUTF8NullTerminated(TestStringUTF8);
		Assert.Equal(TestString.Length, (int)str.Length);
		Assert.Equal(TestString, str.ToString());

		Assert.True(str.EqualsNullTerminatedUTF8(TestStringUTF8));

		Span<byte> bytes = stackalloc byte[(int)str.MaximumUTF8CStringSize];
		Assert.True(bytes.Length >= TestStringUTF8.Length);
		bytes.Fill(byte.MaxValue);
		var written = str.GetUTF8(bytes);
		Assert.Equal(TestStringUTF8.Length, (int)written);
		bytes[(int)written] = 0;
		bytes = bytes[..(int)written];
		Assert.True(TestStringUTF8.SequenceEqual(bytes));

		Assert.Throws<ArgumentException>("utf8", () => JSString.CreateFromUTF8NullTerminated(TestStringUTF8[..^1]));
		Assert.Throws<ArgumentException>("utf8", () => JSString.CreateFromUTF8NullTerminated(ReadOnlySpan<byte>.Empty));
	}

	[Fact]
	public void EqualityTests()
	{
		using var str = JSString.CreateFromUTF8NullTerminated(TestStringUTF8);
		using var str2 = JSString.CreateFromUTF8NullTerminated(TestStringUTF8);
		Assert.Equal(TestString, str.ToString());
		Assert.True(str.Equals(TestString));
		Assert.True(str.Equals(str));
		Assert.True(str == str2);

		Assert.False(str.Equals((string?)null));
		Assert.False(str.Equals((JSString?)null));
		Assert.True(str != null);

		Assert.Throws<ArgumentException>("utf8", () => str.EqualsNullTerminatedUTF8(TestStringUTF8[..^1]));
		Assert.Throws<ArgumentException>("utf8", () => str.EqualsNullTerminatedUTF8(ReadOnlySpan<byte>.Empty));
	}
}
