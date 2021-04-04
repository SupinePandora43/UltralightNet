using System;
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
		public static partial IntPtr ulCreateStringUTF8([MarshalAs(UnmanagedType.LPUTF8Str)] string str, uint len);

		/// <summary>Create string from UTF-16 buffer.</summary>
		[GeneratedDllImport("Ultralight", CharSet = CharSet.Unicode)]
		public static partial IntPtr ulCreateStringUTF16([MarshalAs(UnmanagedType.LPWStr)] string str, uint len);

		/// <summary>Create string from copy of existing string.</summary>
		[DllImport("Ultralight")]
		[Obsolete("missing")]
		public static extern IntPtr ulCreateStringFromCopy(IntPtr str);

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

		[GeneratedDllImport("Ultralight", CharSet = CharSet.Ansi)]
		public static partial void ulStringAssignCString(IntPtr str, [MarshalAs(UnmanagedType.LPUTF8Str)] string c_str);
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

		public static ICustomMarshaler GetInstance(string cookie) => instance;

		public int GetNativeDataSize() => 16;

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

		/// <summary>
		/// Creates <see cref="string"/> from ULString pointer
		/// </summary>
		/// <param name="ptr">ULString pointer</param>
		/// <returns></returns>
		public static string NativeToManaged(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero) return null;
			ULStringPTR result = Marshal.PtrToStructure<ULStringPTR>(ptr);
			return Marshal.PtrToStringUni(result.data_, (int)result.length_);
		}

		/// <summary>
		/// Frees ULString
		/// </summary>
		/// <param name="ptr">ULString pointer</param>
		public static void CleanUpNative(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero) return;
			Marshal.DestroyStructure<ULStringSTR>(ptr);
			//Marshal.FreeHGlobal(ptr);
		}

		#endregion Code
	}
}
