using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UltralightNet.JavaScript;
using Xunit;

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
		Assert.NotEqual((nuint)Unsafe.AsPointer(ref MemoryMarshal.GetReference(TestStringUTF8)), (nuint)str.UTF16DataRaw);
		Assert.Equal(TestString, str.ToString());
		// TODO make sure utf8 equals
		Assert.Throws<ArgumentException>("utf8", () => JSString.CreateFromUTF8NullTerminated(TestStringUTF8[..^1]));
	}
}
