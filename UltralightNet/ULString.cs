using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		/// <summary>Create string from null-terminated ASCII C-string.</summary>
		[GeneratedDllImport("Ultralight", CharSet = CharSet.Ansi)]
		public static partial IntPtr ulCreateString([MarshalAs(UnmanagedType.LPStr)] string str);

		/// <summary>Create string from UTF-8 buffer.</summary>
		[GeneratedDllImport("Ultralight")]
		[Obsolete("Unexpected behaviour")]
		public static partial IntPtr ulCreateStringUTF8(
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
			[MarshalAs(UnmanagedType.LPUTF8Str)]
#else
			[MarshalAs(UnmanagedType.LPStr)]
#endif
			string str,
			uint len
		);

		/// <summary>Create string from UTF-16 buffer.</summary>
		[GeneratedDllImport("Ultralight", CharSet = CharSet.Unicode)]
		public static partial IntPtr ulCreateStringUTF16([MarshalAs(UnmanagedType.LPWStr)] string str, uint len);

		// <summary>Create string from copy of existing string.</summary>
		// [DllImport("Ultralight")]
		// [Obsolete("missing")]
		// public static extern IntPtr ulCreateStringFromCopy(IntPtr str);
		public static IntPtr ulCreateStringFromCopy() => throw new EntryPointNotFoundException();

		/// <summary>Destroy string (you should destroy any strings you explicitly Create).</summary>
		[DllImport("Ultralight")]
		public static extern void ulDestroyString(IntPtr str);

		/// <summary>Get internal UTF-16 buffer data.</summary>
		[GeneratedDllImport("Ultralight", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static partial string ulStringGetData(IntPtr str);

		/// <summary>Get internal UTF-16 buffer data.</summary>
		[DllImport("Ultralight", EntryPoint = "ulStringGetData")]
		public static extern IntPtr ulStringGetDataPtr(IntPtr str);

		/// <summary>Get length in UTF-16 characters.</summary>
		[DllImport("Ultralight")]
		public static extern uint ulStringGetLength(IntPtr str);

		/// <summary>Whether this string is empty or not.</summary>
		[GeneratedDllImport("Ultralight")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulStringIsEmpty(IntPtr str);

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
		public ULString ulstring;

		public ULStringGeneratedDllImportMarshaler(string str)
		{
			ulstring = new() { data = (ushort*)Marshal.StringToHGlobalUni(str), length = (nuint)str.Length };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToManaged()
		{
			return Marshal.PtrToStringUni((IntPtr)ulstring.data, (int)ulstring.length);
		}

		public unsafe ULString* Value
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				fixed (ULString* ulStringPtr = &ulstring)
				{
					return ulStringPtr;
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => ulstring = *value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FreeNative()
		{
			Marshal.FreeHGlobal((IntPtr)ulstring.data);
		}
	}

	[BlittableType]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ULString
	{
		public ushort* data;
		public nuint length;

		/// <summary>Do not use on pointers</summary>
		public string ToManaged()
		{
			return new((char*)data, 0, (int)length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe string NativeToManaged(ULString* ulString)
		{
			return new((char*)ulString->data, 0, (int)ulString->length);
		}
	}
}
