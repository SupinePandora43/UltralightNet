using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UltralightNet.UL
{
	public sealed unsafe class ULString : ICloneable, IEquatable<ULString>
	{
		public readonly C_String* ptr;

		public ULString(C_String* ptr) => this.ptr = ptr;
		public ULString(IntPtr ptr) : this((C_String*)ptr) { }

		public ULString(string str)
		{
			IntPtr strPtr = Marshal.StringToHGlobalUni(str);
			ptr = Methods.ulCreateStringUTF16((ushort*)strPtr, (UIntPtr)str.Length);
			Marshal.FreeHGlobal(strPtr);
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
			if (ulString.ptr == null) return true;
			if (ulString.ptr == default) return true;
			if (Methods.ulStringIsEmpty(ulString.ptr)) return true;
			return string.IsNullOrEmpty(ulString.ToString());
		}
		public static explicit operator ULString(string str) => new ULString(str);
		public static implicit operator C_String*(ULString ulString) => ulString.ptr;
		public static implicit operator IntPtr(ULString ulString) => (IntPtr)ulString.ptr;
		public static implicit operator string(ULString ulString) => ulString.ToString();
	}
