using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		/// <summary>Create string from null-terminated ASCII C-string.</summary>
		[GeneratedDllImport("Ultralight")]
		public unsafe static partial ULString* ulCreateString([MarshalAs(UnmanagedType.LPStr)] string str);

		/// <summary>Create string from UTF-8 buffer.</summary>
		[GeneratedDllImport("Ultralight")]
		[Obsolete("Unexpected behaviour")]
		public static unsafe partial ULString* ulCreateStringUTF8(
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
			[MarshalAs(UnmanagedType.LPUTF8Str)]
#else
			[MarshalAs(UnmanagedType.LPStr)]
#endif
			string str,
			nuint len
		);

		/// <summary>Create string from UTF-16 buffer.</summary>
		[GeneratedDllImport("Ultralight")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe partial ULString* ulCreateStringUTF16([MarshalAs(UnmanagedType.LPWStr)] string str, nuint len);

		// <summary>Create string from copy of existing string.</summary>
		[DllImport("Ultralight")]
		public static unsafe extern ULString* ulCreateStringFromCopy(ULString* str);

		/// <summary>Destroy string (you should destroy any strings you explicitly Create).</summary>
		[DllImport("Ultralight")]
		public static unsafe extern void ulDestroyString(ULString* str);

		/// <summary>Get internal UTF-8 buffer data.</summary>
		[DllImport("Ultralight", EntryPoint = "ulStringGetData")]
		public static unsafe extern byte* ulStringGetDataPtr(ULString* str);

		/// <summary>Get length in UTF-8 characters.</summary>
		[DllImport("Ultralight")]
		public static unsafe extern nuint ulStringGetLength(ULString* str);

		/// <summary>Whether this string is empty or not.</summary>
		[GeneratedDllImport("Ultralight")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static unsafe partial bool ulStringIsEmpty(ULString* str);

		/// <summary>Replaces the contents of 'str' with the contents of 'new_str'</summary>
		[DllImport("Ultralight")]
		public static unsafe extern void ulStringAssignString(ULString* str, ULString* newStr);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulStringAssignCString(
			IntPtr str,
			[MarshalAs(
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
				UnmanagedType.LPUTF8Str
#else
				UnmanagedType.LPStr
#endif
			)] string c_str);
	}

	public unsafe ref struct ULStringGeneratedDllImportMarshaler
	{
		public ULString* ulstringPtr;

		[SkipLocalsInit]
		public ULStringGeneratedDllImportMarshaler(string str)
		{
			ulstringPtr = Methods.ulCreateStringUTF16(str, (nuint) str.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[SkipLocalsInit]
		public string ToManaged()
		{
			return Marshal.PtrToStringUTF8((IntPtr)ulstringPtr->data, (int)ulstringPtr->length);
		}

		public unsafe ULString* Value
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			[SkipLocalsInit]
			get => ulstringPtr;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			[SkipLocalsInit]
			set => ulstringPtr = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[SkipLocalsInit]
		public void FreeNative()
		{
			Methods.ulDestroyString(ulstringPtr);
		}
	}

	[BlittableType]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ULString
	{
		public byte* data;
		public nuint length;

		/// <summary>Do not use on pointers</summary>
		public string ToManaged()
		{
			return Marshal.PtrToStringUTF8((IntPtr)data, (int)length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe string NativeToManaged(ULString* ulString)
		{
			return Marshal.PtrToStringUTF8((IntPtr)ulString->data, (int)ulString->length);
		}
	}
}
