using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using LibraryImportAttribute = System.Runtime.InteropServices.GeneratedDllImportAttribute;

namespace UltralightNet;

public static unsafe partial class Methods
{
	/// <summary>Create string from null-terminated ASCII C-string.</summary>
	[LibraryImport(LibUltralight)]
	public static partial ULString* ulCreateString([MarshalUsing(typeof(UTF8Marshaller))] in string str);

	/// <summary>Create string from UTF-8 buffer.</summary>
	[LibraryImport(LibUltralight)]
	[Obsolete("Unexpected behaviour")]
	public static partial ULString* ulCreateStringUTF8(
		byte* data,
		nuint len
	);

	/// <summary>Create string from UTF-16 buffer.</summary>
	[LibraryImport(LibUltralight)]
	public static partial ULString* ulCreateStringUTF16([MarshalAs(UnmanagedType.LPWStr)] string str, nuint len);

	/// <summary>Create string from UTF-16 buffer.</summary>
	[LibraryImport(LibUltralight)]
	public static partial ULString* ulCreateStringUTF16(ushort* str, nuint len);

	// <summary>Create string from copy of existing string.</summary>
	public static ULString* ulCreateStringFromCopy(ULString* str)
	{
		var ulString = (ULString*)(
#if NET6_0_OR_GREATER
			RuntimeInformation.OSArchitecture is Architecture.Arm ?
			NativeMemory.AlignedAlloc((nuint)sizeof(ULString), (nuint)sizeof(ULString)) : // INTEROPTODO: ARM32
#endif
			NativeMemory.Alloc((nuint)sizeof(ULString)));
		*ulString = str->Clone();
		return ulString;
	}

	/// <summary>Destroy string (you should destroy any strings you explicitly Create).</summary>
	[LibraryImport(LibUltralight)]
	public static partial void ulDestroyString(ULString* str);

	/// <summary>Get internal UTF-8 buffer data.</summary>
	[LibraryImport(LibUltralight)]
	public static partial byte* ulStringGetData(ULString* str);

	/// <summary>Get length in UTF-8 characters.</summary>
	[LibraryImport(LibUltralight)]
	public static partial nuint ulStringGetLength(ULString* str);

	/// <summary>Whether this string is empty or not.</summary>
	[LibraryImport(LibUltralight)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool ulStringIsEmpty(ULString* str);

	/// <summary>Replaces the contents of 'str' with the contents of 'new_str'</summary>
	[LibraryImport(LibUltralight)]
	public static partial void ulStringAssignString(ULString* str, ULString* newStr);

	[LibraryImport(LibUltralight)]
	public static partial void ulStringAssignCString(ULString* str, byte* c_str);

	public static void Deallocate(this ref ULString str)
	{
		str.Dispose();
		NativeMemory.Free(Unsafe.AsPointer(ref str));
	}
}

[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public unsafe struct ULString : IDisposable, ICloneable, IEquatable<ULString>
{
	public byte* data;
	public nuint length;

	public ULString(ReadOnlySpan<char> chars)
	{
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1
		data = (byte*)NativeMemory.Alloc((length = (nuint)Encoding.UTF8.GetByteCount(chars)) + 1);
		nuint written = (nuint)Encoding.UTF8.GetBytes(chars, new Span<byte>(data, checked((int)length)));
#else
		nuint written = 0;
		if (chars.Length is not 0)
			fixed (char* charPtr = chars)
				written = (nuint)Encoding.UTF8.GetBytes(charPtr, chars.Length, data = (byte*)NativeMemory.Alloc((length = (nuint)Encoding.UTF8.GetByteCount(charPtr, chars.Length)) + 1), checked((int)length));
		else data = (byte*)NativeMemory.Alloc((length = 0) + 1);
#endif
		Debug.Assert(written == length);
		data[length] = 0;
	}
	public ULString(ReadOnlySpan<byte> chars)
	{
		data = (byte*)NativeMemory.Alloc((length = (nuint)chars.Length) + 1);
		chars.CopyTo(new Span<byte>(data, (int)length));
		data[length] = 0;
	}
	public ULString()
	{
		(data = (byte*)NativeMemory.Alloc(1))
			[length = 0] = 0;
	}

	public void Dispose()
	{
		if (data is not null)
		{
			NativeMemory.Free(data);
			data = null;
			length = 0; // why? string shouldn't even be accessed after disposal! i just do this to notice problems
		}
	}

	public void Assign(ReadOnlySpan<char> newStr)
	{
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1
		data = (byte*)NativeMemory.Alloc((length = (nuint)Encoding.UTF8.GetByteCount(newStr)) + 1);
		nuint written = (nuint)Encoding.UTF8.GetBytes(newStr, new Span<byte>(data, checked((int)length)));
#else
		nuint written = 0;
		if (newStr.Length is not 0)
			fixed (char* charPtr = newStr)
				written = (nuint)Encoding.UTF8.GetBytes(charPtr, newStr.Length,
				data = data is not null ?
					(byte*)NativeMemory.Realloc(data, (length = (nuint)Encoding.UTF8.GetByteCount(charPtr, newStr.Length)) + 1) :
					(byte*)NativeMemory.Alloc((length = (nuint)Encoding.UTF8.GetByteCount(charPtr, newStr.Length)) + 1), checked((int)length));
		else
			data = data is not null ?
				(byte*)NativeMemory.Realloc(data, (length = 0) + 1) :
				(byte*)NativeMemory.Alloc((length = 0) + 1);
#endif
		Debug.Assert(written == length);
		data[length] = 0;
	}
	public void Assign(ReadOnlySpan<byte> newStr)
	{
		if (data is not null) data = (byte*)NativeMemory.Realloc(data, (length = (nuint)newStr.Length) + 1);
		else data = (byte*)NativeMemory.Alloc((length = (nuint)newStr.Length) + 1);

		newStr.CopyTo(new Span<byte>(data, newStr.Length));

		data[length] = 0;
	}
	public void Assign(ULString newStr)
	{
		if (data is not null) data = (byte*)NativeMemory.Realloc(data, newStr.length + 1);
		else data = (byte*)NativeMemory.Alloc(newStr.length + 1);

		length = newStr.length;

		Buffer.MemoryCopy(data, newStr.data, length, length);

		data[length] = 0;
	}
	public void Assign(ULString* newStr)
	{
		if (data is not null) data = (byte*)NativeMemory.Realloc(data, newStr->length + 1);
		else data = (byte*)NativeMemory.Alloc(newStr->length + 1);

		length = newStr->length;

		Buffer.MemoryCopy(data, newStr->data, length, length);

		data[length] = 0;
	}

	/// <remarks>it doesn't copy</remarks>
	public readonly ULString* Allocate() // TODO: implement "ULString* Create(ROS<char/byte>)" that will not require member fields
	{
		ULString* str = (ULString*)NativeMemory.Alloc((nuint)sizeof(ULString));
		str->data = data;
		str->length = length;
		return str;
	}

	public readonly ULString Clone()
	{
		ULString clone = new()
		{
			data = (byte*)NativeMemory.Alloc(length + 1),
			length = length
		};

		Buffer.MemoryCopy(data, clone.data, length, length);

		clone.data[length] = 0;

		return clone;
	}
	readonly object ICloneable.Clone() => Clone();

	public readonly bool Equals(ULString str)
	{
		if (length != str.length) return false;
		if (data == str.data) return true;
		if (length < (nuint)int.MaxValue) return new ReadOnlySpan<byte>(data, unchecked((int)length)).SequenceEqual(new ReadOnlySpan<byte>(str.data, unchecked((int)length)));
		else for (nuint i = 0; i < length; i++) if (data[i] != str.data[i]) return false;
		return true;
	}
	public override readonly bool Equals(object? other) => other is ULString str ? Equals(str) : false;

#if NET6_0_OR_GREATER
	public override readonly int GetHashCode()
	{
		var hash = new HashCode();
		hash.Add(length);
		hash.AddBytes(new ReadOnlySpan<byte>(data, unchecked((int)Math.Clamp(length, 0, (nuint)int.MaxValue))));
		return hash.ToHashCode();
	}
#endif

	public static explicit operator string(ULString str) => str.data is null || str.length is 0 ? string.Empty :
#if NETSTANDARD2_1 || NETCOREAPP1_1_OR_GREATER
		Marshal.PtrToStringUTF8((IntPtr)str.data, checked((int)str.length));
#elif NETSTANDARD2_0
		Encoding.UTF8.GetString(str.data, checked((int)str.length));
#elif NETFRAMEWORK
		new string((sbyte*)str.data, 0, checked((int)str.length));
#else
		Marshal.PtrToStringUTF8((IntPtr)str.data, checked((int)str.length));
#endif

	public override readonly string ToString() => (string)this;
	public readonly ReadOnlySpan<byte> ToSpan() => new(data, checked((int)length));

	internal static string NativeToManaged(ULString* str) => str is null ? string.Empty :
#if NETSTANDARD2_1 || NETCOREAPP1_1_OR_GREATER
		Marshal.PtrToStringUTF8((IntPtr)str->data, checked((int)str->length));
#elif NETSTANDARD2_0
		Encoding.UTF8.GetString(str->data, checked((int)str->length));
#elif NETFRAMEWORK
		new string((sbyte*)str->data, 0, checked((int)str->length));
#else
		Marshal.PtrToStringUTF8((IntPtr)str->data, checked((int)str->length));
#endif

	// INTEROPTODO: CUSTOMTYPEMARSHALLER
	[StructLayout(LayoutKind.Auto)]
	public unsafe ref struct ToNative
	{
		public ULString ulString;

		public ToNative(string str)
		{
			ulString = new(str.AsSpan());
		}

		public ULString* Value => (ULString*)Unsafe.AsPointer(ref ulString);

		public void FreeNative() => ulString.Dispose();
	}

	public unsafe ref struct ToManaged_
	{
		public ToManaged_(string _) { }

		private ULString ul = default;
		public ULString* Value
		{
			get => null;
			set => ul = *value;
		}

		public string ToManaged() => (string)ul;
	}
}
