using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace UltralightNet.Test;

public unsafe class ULStringTest
{
	public const string Str = "abc123абв@.";

	public static IEnumerable<object[]> GetTestStrings()
	{
		yield return new object[] { string.Empty };
		yield return new object[] { new string('Ж', 340) }; // Stack
		yield return new object[] { new string('Ж', 341) }; // Heap
		yield return new object[] { new string('Ж', 8192) }; // Heap stress
	}

	[Theory]
	[MemberData(nameof(GetTestStrings))]
	public void SameAsNative(string str)
	{
		using ULStringToNativeMarshaller marshaller = new(str);
		var ulString = Methods.ulCreateStringUTF16(str, (nuint)str.Length);

		Assert.Equal(ulString->length, marshaller.Value->length);
		for (uint i = 0; i < (uint)ulString->length; i++)
		{
			Assert.Equal(ulString->data[i], marshaller.Value->data[i]);
		}

		Methods.ulDestroyString(ulString);
	}

	[Theory]
	[MemberData(nameof(GetTestStrings))]
	public void ByteCount(string str)
	{
		Assert.True(((str.Length + 1) * 3 + 1) >= Encoding.UTF8.GetByteCount(str) + 1);
		Assert.True(((str.Length + 1) * 3 + 1) >= Encoding.UTF8.GetMaxByteCount(str.Length) + 1);
	}

	[Theory]
	[MemberData(nameof(GetTestStrings))]
	public void Assignment(string str)
	{
		using ULStringToNativeMarshaller output_marshaller = new();
		using ULStringToNativeMarshaller input_marshaller = new(str);

		byte* m = output_marshaller.Opaque.data;

		Assert.NotEqual((nuint)output_marshaller.Opaque.data, (nuint)input_marshaller.Opaque.data);

		Methods.ulStringAssignString(output_marshaller.Value, input_marshaller.Value);

		Assert.NotEqual((nuint)output_marshaller.Opaque.data, (nuint)input_marshaller.Opaque.data);

		Assert.Equal(input_marshaller.Opaque.length, output_marshaller.Opaque.length);
		for (nuint i = 0; i < output_marshaller.Opaque.length; i++) Assert.Equal(input_marshaller.Opaque.data[i], output_marshaller.Opaque.data[i]);

		Assert.Equal(0, output_marshaller.Opaque.data[output_marshaller.Opaque.length]);

		if (str is not "") Assert.NotEqual((nuint)m, (nuint)output_marshaller.Opaque.data);
	}
}