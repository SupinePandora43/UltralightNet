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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

[DebuggerDisplay("{ToManaged(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public unsafe struct ULString : IDisposable, ICloneable
{
	public byte* data;
	public nuint length;

	public ULString(ReadOnlySpan<char> chars)
	{
		data = (byte*)NativeMemory.Alloc((length = (nuint)Encoding.UTF8.GetByteCount(chars)) + 1);
		nuint written = (nuint)Encoding.UTF8.GetBytes(chars, new Span<byte>(data, (int)length)); // INTEROPTODO: INT64
		data[length = written] = 0;
	}
	public ULString(ReadOnlySpan<byte> chars)
	{
		data = (byte*)NativeMemory.Alloc((length = (nuint)chars.Length) + 1);
		chars.CopyTo(new Span<byte>(data, (int)length));
	}

	public void Dispose()
	{
		if (data is null) return;
		NativeMemory.Free(data);
		data = null;
		length = 0;
	}

	public void Assign(ULString newStr)
	{
		if (data is not null) data = (byte*)NativeMemory.Realloc(data, newStr.length + 1);
		else data = (byte*)NativeMemory.Alloc(newStr.length + 1);

		length = newStr.length;

		Buffer.MemoryCopy(data, newStr.data, length, length);

		data[length] = 0; // just to make sure
	}
	public void Assign(ULString* newStr)
	{
		if (data is not null) data = (byte*)NativeMemory.Realloc(data, newStr->length + 1);
		else data = (byte*)NativeMemory.Alloc(newStr->length + 1);

		length = newStr->length;

		Buffer.MemoryCopy(data, newStr->data, length, length);

		data[length] = 0; // just to make sure
	}

	/// <remarks>it doesn't copy</remarks>
	public readonly ULString* Allocate()
	{
		ULString* str = (ULString*)NativeMemory.Alloc((nuint)sizeof(ULString*));
		str->data = data;
		str->length = length;
		return str;
	}

	public readonly ULString Clone()
	{
		Unsafe.SkipInit(out ULString clone);

		clone.data = (byte*)NativeMemory.Alloc(length + 1);
		clone.length = length;

		Buffer.MemoryCopy(data, clone.data, length, length);

		clone.data[length] = 0; // just to make sure

		return clone;
	}
	readonly object ICloneable.Clone() => Clone();

	public static explicit operator string(ULString str) => // INTEROPTODO: INT64
#if NETSTANDARD2_1 || NETCOREAPP1_1_OR_GREATER
		Marshal.PtrToStringUTF8((IntPtr)str.data, (int)str.length);
#elif NETSTANDARD2_0
		Encoding.UTF8.GetString(str.data, (int)str.length);
#elif NETFRAMEWORK
		new string((sbyte*)str.data, 0, (int)str.length);
#else
		Marshal.PtrToStringUTF8((IntPtr)str.data, (int)str.length);
#endif

	internal static string NativeToManaged(ULString* str) => str is null ? string.Empty : // INTEROPTODO: INT64
#if NETSTANDARD2_1 || NETCOREAPP1_1_OR_GREATER
		Marshal.PtrToStringUTF8((IntPtr)str->data, (int)str->length);
#elif NETSTANDARD2_0
		Encoding.UTF8.GetString(str->data, (int)str->length);
#elif NETFRAMEWORK
		new string((sbyte*)str->data, 0, (int)str->length);
#else
		Marshal.PtrToStringUTF8((IntPtr)str->data, (int)str->length);
#endif

	// INTEROPTODO: CUSTOMTYPEMARSHALLER
	[StructLayout(LayoutKind.Auto)]
	public unsafe ref struct ToNative
	{
		public ULString ulString;

		public ToNative(string str)
		{
			ulString = new(str);
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
