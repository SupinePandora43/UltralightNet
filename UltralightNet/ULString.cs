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

		/// <summary>Get length in UTF-16 characters.</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial uint ulStringGetLength(IntPtr str);

		/// <summary>Whether this string is empty or not.</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial bool ulStringIsEmpty(IntPtr str);

		/// <summary>Replaces the contents of 'str' with the contents of 'new_str'</summary>
		[DllImport("Ultralight")]
		public static extern void ulStringAssignString(IntPtr str, IntPtr newStr);

		[GeneratedDllImport("Ultralight", CharSet = CharSet.Ansi)]
		public static partial void ulStringAssignCString(IntPtr str, [MarshalAs(UnmanagedType.LPUTF8Str)] string c_str);
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ULString16
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public string data_;
		public uint length_;
	}

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
	public class ULString : IDisposable, ICloneable, IEquatable<ULString>
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
	{
		public IntPtr Ptr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private set;
		}

		public ULString16 ULString16
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Marshal.PtrToStructure<ULString16>(Ptr);
		}

		public ULString(IntPtr ptr, bool dispose = false)
		{
			Ptr = ptr;
			IsDisposed = !dispose;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ULString(string str = null) => Ptr = Methods.ulCreateStringUTF16(str ?? "", str is null ? 0 : (uint)str.Length);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string GetData() => Methods.ulStringGetData(Ptr);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint GetLength() => Methods.ulStringGetLength(Ptr);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsEmpty() => Methods.ulStringIsEmpty(Ptr);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Assign(ULString newStr) => Methods.ulStringAssignString(Ptr, newStr.Ptr);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Assign(string newStr) => Assign(new ULString(newStr));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString() => GetData();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		~ULString() => Dispose();

		public bool IsDisposed
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private set;
		}

		public void Dispose()
		{
			if (IsDisposed) return;
			Methods.ulDestroyString(Ptr);

			IsDisposed = true;
			GC.SuppressFinalize(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public object Clone() => new ULString(GetData());

#nullable enable

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ReferenceEquals(ULString? objA, ULString? objB) => ((objA is null) && (objB is null)) ? true : (((objA is null) || (objB is null)) ? false : objA.Ptr == objB.Ptr);

#nullable restore

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(ULString other) => other is not null && (Ptr == other.Ptr || GetData() == other.GetData());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object obj) => Equals(obj as ULString);

#nullable enable

		public static bool operator ==(ULString? a, ULString? b)
		{
			if ((a is null) && (b is null)) return true;
			else if ((a is null) || (b is null)) return false;
			return a.Equals(b);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(ULString? a, ULString? b) => !(a == b);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator ULString(string str) => new(str);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator IntPtr(ULString ulString) => ulString.Ptr;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator string(ULString ulString) => ulString.GetData();
	}
}
