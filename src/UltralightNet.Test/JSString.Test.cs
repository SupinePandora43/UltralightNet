using System.Collections.Generic;
using Xunit;

namespace UltralightNet.Test;

public unsafe class JSStringTest
{
	public static IEnumerable<object[]> GetTestStrings()
	{
		yield return new object[] { (JSString)"TEST" };
		yield return new object[] { (JSString)"ТЕСТ" };
		yield return new object[] { (JSString)"" };
	}

	[Theory]
	[MemberData(nameof(GetTestStrings))]
	public void Clone(JSString s)
	{
		string m = (string)s;
		var r = s.Clone();
		Assert.Equal(s, r);
		Assert.Equal(m, (string)r);
		Assert.True(JSString.ReferenceEquals(s, r));
		s.Dispose();
		Assert.Equal(m, (string)r);
		r.Dispose();
	}

	[Theory]
	[MemberData(nameof(GetTestStrings))]
	public void Equality(JSString s1)
	{
		JSString s2 = new((string)s1);
		Assert.True(s1.Equals(s2));
		Assert.True(s1.Equals((object)s2));
		Assert.True(s1 == s2);
		Assert.False(s1 != s2);
		Assert.False(s1 == null);
		Assert.False(s1!.Equals((JSString?)null));
		Assert.False(s1!.Equals((object?)null));
		Assert.Equal(s1, s2);
		Assert.Equal((object)s1!, (object)s2);
		Assert.False(JSString.ReferenceEquals(s1!, s2));
		Assert.False(s1 == null);
	}
}
