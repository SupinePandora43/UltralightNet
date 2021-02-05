using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Ultralight
{
	public class ULString : ICloneable, IEquatable<ULString>
	{
		public readonly IntPtr ptr;

		public ULString(IntPtr ptr)
		{
			this.ptr = ptr;
		}
		public ULString(string str)
		{
			ptr = Methods.ulCreateStringUTF16(str, (UIntPtr)str.Length);
		}
		/// <remarks>
		///	Ultralight handles strings in unicode (UTF16)
		/// </remarks>
		/// <seealso cref="CreateUTF16(string)"/>
		public static ULString CreateAnsi(string str)
		{
			return new ULString(Methods.ulCreateString(str));
		}
		/// <summary>
		/// do not use
		/// </summary>
		/// <remarks>
		/// doesn't work as excepted on my machine
		/// </remarks>
		/// <seealso cref="CreateUTF16(string)"/>
		public static ULString CreateUTF8(string str)
		{
			return new ULString(Methods.ulCreateStringUTF8(str, (UIntPtr)str.Length));
		}
		/// <summary>
		/// Creates Unicode (UTF16) ULString
		/// </summary>
		public static ULString CreateUTF16(string str)
		{
			return new ULString(Methods.ulCreateStringUTF16(str, (UIntPtr)str.Length));
		}

		public void AssingULString(ULString ulString)
		{
			Methods.ulStringAssignString(ptr, ulString.ptr);
		}
		public void AssingString(string str)
		{
			Methods.ulStringAssignCString(ptr, str);
		}

		public override string ToString()
		{
			return Marshal.PtrToStringUni(Methods.ulStringGetData(ptr), (int)Methods.ulStringGetLength(ptr));
		}

		public object Clone()
		{
			return new ULString(Methods.ulCreateStringFromCopy(ptr));
		}

		public bool Equals(ULString? other)
		{
			if (ptr == other?.ptr) return true;

			return ToString().Equals(other?.ToString());
		}
		public static bool ReferenceEquals(ULString? objA, ULString? objB)
		{
			if (objA == objB) return true;
			if (objA == null || objB == null) return false;
			return objA.ptr == objB.ptr;
		}
		~ULString()
		{
			Methods.ulDestroyString(ptr);
		}
		public static bool IsNullOrEmpty([NotNullWhen(false)] ULString? ulString)
		{
			if (ulString == null) return true;
			if (ulString.ptr == IntPtr.Zero) return true;
			if (ulString.ptr == default) return true;
			if (Methods.ulStringIsEmpty(ulString.ptr)) return true;
			return string.IsNullOrEmpty(ulString.ToString());
		}
		public static explicit operator ULString(string str) => new ULString(str);
		public static implicit operator IntPtr(ULString ulString) => ulString.ptr;
		public static implicit operator string(ULString ulString) => ulString.ToString();

		public static class Methods
		{
			[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
			public static extern IntPtr ulCreateString([MarshalAs(UnmanagedType.LPStr)] string str);

			[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
			public static extern IntPtr ulCreateStringUTF8([MarshalAs(UnmanagedType.LPUTF8Str)] string str, UIntPtr len);

			[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
			public static extern IntPtr ulCreateStringUTF16([MarshalAs(UnmanagedType.LPWStr)] string str, UIntPtr len);

			[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
			public static extern IntPtr ulCreateStringFromCopy(IntPtr str);

			[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
			public static extern void ulDestroyString(IntPtr str);

			[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
			public static extern IntPtr ulStringGetData(IntPtr str);

			[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
			public static extern UIntPtr ulStringGetLength(IntPtr str);

			[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
			public static extern bool ulStringIsEmpty(IntPtr str);

			[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
			public static extern void ulStringAssignString(IntPtr str, IntPtr new_str);

			[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
			public static extern void ulStringAssignCString(IntPtr str, [MarshalAs(UnmanagedType.LPUTF8Str)] string c_str);

		}
	}
}
