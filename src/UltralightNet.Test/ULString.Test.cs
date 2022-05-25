using System;
using System.Collections.Generic;
using Xunit;

namespace UltralightNet.Test;

public unsafe class ULStringTest
{
	public static IEnumerable<object[]> GetTestStrings()
	{
		yield return new object[] { string.Empty };
		yield return new object[] { "я тесто я тесто aаaаaаaа!!!123!!!" };
	}

	[Theory]
	[MemberData(nameof(GetTestStrings))]
	public void SameAsNative(string str)
	{
		var reference = Methods.ulCreateStringUTF16(str, (nuint)str.Length);
		using ULString managed = new(str);

		Assert.Equal(reference->length, managed.length);

		for (nuint i = 0; i < reference->length; i++)
		{
			Assert.Equal(reference->data[i], managed.data[i]);
		}

		Methods.ulDestroyString(reference);
	}

	[Theory]
	[MemberData(nameof(GetTestStrings))]
	public void NullTerminator(string str)
	{
		var reference = Methods.ulCreateStringUTF16(str, (nuint)str.Length);
		if (reference->data[reference->length] is not 0) throw new InvalidProgramException("data[length] != 0");

		using ULString managed = new(str);

		Assert.Equal(0, managed.data[managed.length]);

		Methods.ulDestroyString(reference);
	}

	#region ASSIGN
	[Theory]
	[MemberData(nameof(GetTestStrings))]
	public void ManagedAssignManagedToNative(string str)
	{
		using ULString managed = new(str);
		var native = Methods.ulCreateStringUTF16("", 0);

		native->Assign(managed);
		native->Assign(&managed);

		Assert.Equal((ulong)managed.length, (ulong)native->length);

		for (nuint i = 0; i < managed.length; i++)
		{
			Assert.Equal(managed.data[i], native->data[i]);
		}

		Methods.ulDestroyString(native);
	}
	[Theory]
	[MemberData(nameof(GetTestStrings))]
	public void ManagedAssignNativeToManaged(string str)
	{
		var native = Methods.ulCreateStringUTF16(str, 0);
		using ULString managed = new("");

		managed.Assign(native);
		managed.Assign(*native);

		Assert.Equal((ulong)native->length, (ulong)managed.length);

		for (nuint i = 0; i < native->length; i++)
		{
			Assert.Equal(native->data[i], managed.data[i]);
		}

		Methods.ulDestroyString(native);
	}
	[Theory]
	[MemberData(nameof(GetTestStrings))]
	public void UNManagedAssignManagedToNative(string str)
	{
		using ULString managed = new(str);
		var native = Methods.ulCreateStringUTF16("", 0);

		Methods.ulStringAssignString(native, &managed);

		Assert.Equal((ulong)managed.length, (ulong)native->length);

		for (nuint i = 0; i < managed.length; i++)
		{
			Assert.Equal(managed.data[i], native->data[i]);
		}

		Methods.ulDestroyString(native);
	}
	[Theory]
	[MemberData(nameof(GetTestStrings))]
	public void UNManagedAssignNativeToManaged(string str)
	{
		var native = Methods.ulCreateStringUTF16(str, 0);
		using ULString managed = new("");

		Methods.ulStringAssignString(&managed, native);

		Assert.Equal((ulong)native->length, (ulong)managed.length);

		for (nuint i = 0; i < native->length; i++)
		{
			Assert.Equal(native->data[i], managed.data[i]);
		}

		Methods.ulDestroyString(native);
	}
	#endregion ASSIGN

}
