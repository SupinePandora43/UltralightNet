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
		[GeneratedDllImport("Ultralight")]
		public static partial uint ulStringGetLength(IntPtr str);

		/// <summary>Whether this string is empty or not.</summary>
		[GeneratedDllImport("Ultralight")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulStringIsEmpty(IntPtr str);

		/// <summary>Replaces the contents of 'str' with the contents of 'new_str'</summary>
		[DllImport("Ultralight")]
		public static extern void ulStringAssignString(IntPtr str, IntPtr newStr);

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

	public struct ULStringGeneratedDllImportMarshaler
	{
		public ULStringMarshaler.ULStringPTR ptr;

		public ULStringGeneratedDllImportMarshaler(string str)
		{
			ptr = new()
			{
				data_ = Marshal.StringToCoTaskMemUni(str),
				length_ = (nuint)str.Length
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToManaged()
		{
			return Marshal.PtrToStringUni(ptr.data_, (int)ptr.length_);
		}

		public unsafe ULStringMarshaler.ULStringPTR* Value
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				fixed (ULStringMarshaler.ULStringPTR* valuePtr = &ptr)
					return valuePtr;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => ptr = *value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void FreeNative()
		{
			Marshal.FreeCoTaskMem(ptr.data_);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ULString
	{
		public ushort* data;
		public nuint length;

		public static unsafe string NativeToManaged(ULString* ulString)
		{
			return new((char*)ulString->data, 0, (int)ulString->length);
		}
	}

	public class ULStringMarshaler : ICustomMarshaler
	{
		#region Structures
		[StructLayout(LayoutKind.Sequential)]
		private struct ULStringSTR
		{
			[MarshalAs(UnmanagedType.LPWStr)]
			public string data_;
			public nuint length_;
		}
		[StructLayout(LayoutKind.Sequential)]
		[BlittableType]
		public struct ULStringPTR
		{
			public IntPtr data_;
			public nuint length_;

			public static ULStringPTR ManagedToNative(string str)
			{
				if (str is null) str = "";
				IntPtr data = Marshal.StringToHGlobalUni(str);
				return new() { data_ = data, length_ = (nuint)str.Length };
			}
			public string ToManaged()
			{
				return Marshal.PtrToStringUni(data_, (int)length_);
			}
			public static void CleanUpNative(ULStringPTR ulStringPTR)
			{
				Marshal.FreeHGlobal(ulStringPTR.data_);
			}
		}
		#endregion Structures

		private static readonly ULStringMarshaler instance = new();

		public static ICustomMarshaler GetInstance(string _) => instance;

		public int GetNativeDataSize() => 16; // idk what this means

		public void CleanUpManagedData(object ManagedObj) { }
		public void CleanUpNativeData(IntPtr ptr) => CleanUpNative(ptr);

		public IntPtr MarshalManagedToNative(object ManagedObj) => ManagedToNative(ManagedObj as string);
		public object MarshalNativeToManaged(IntPtr ptr) => NativeToManaged(ptr);

		#region Code

		/// <summary>
		/// Creates ULString from <see cref="string"/>
		/// </summary>
		/// <param name="managedString">Unicode text</param>
		/// <returns>ULString pointer</returns>
		/// <remarks>you <b>MUST</b> call <see cref="CleanUpNativeData(IntPtr)"/> after you done</remarks>
		public static IntPtr ManagedToNative(string managedString)
		{
			if (managedString is null) return IntPtr.Zero;

			IntPtr ptr = Marshal.AllocHGlobal(11);
			ULStringSTR nativeStruct = new()
			{
				data_ = managedString,
				length_ = (uint)managedString.Length
			};
			Marshal.StructureToPtr(
				nativeStruct,
				ptr,
				false
			);
			return ptr;
		}

		public static void ManagedToNative(string managedString, IntPtr ptr)
		{
			if (managedString is null || ptr == IntPtr.Zero) return;

			ULStringSTR nativeStruct = new()
			{
				data_ = managedString,
				length_ = (uint)managedString.Length
			};
			Marshal.StructureToPtr(
				nativeStruct,
				ptr,
				false
			);
		}

		/// <summary>
		/// Creates <see cref="string"/> from ULString pointer
		/// </summary>
		/// <param name="ptr">ULString pointer</param>
		/// <returns></returns>
		public static string NativeToManaged(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero) return null;
#if NET5_0_OR_GREATER || NET451 || NETSTANDARD2_0
			ULStringPTR result = Marshal.PtrToStructure<ULStringPTR>(ptr);
#else
			ULStringPTR result = (ULStringPTR)Marshal.PtrToStructure(ptr, typeof(ULStringPTR));
#endif
			return Marshal.PtrToStringUni(result.data_, (int)result.length_);
		}

		/// <summary>
		/// Frees ULString
		/// </summary>
		/// <param name="ptr">ULString pointer</param>
		public static void CleanUpNative(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero) return;
#if NET5_0_OR_GREATER || NET451 || NETSTANDARD2_0
			Marshal.DestroyStructure<ULStringSTR>(ptr);
#else
			Marshal.DestroyStructure(ptr, typeof(ULStringSTR));
#endif
			//Marshal.FreeHGlobal(ptr);
		}

		#endregion Code
	}
}
