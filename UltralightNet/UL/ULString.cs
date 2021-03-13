using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "<Pending>")]
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
		public static extern IntPtr ulCreateStringFromCopy(IntPtr str);

		/// <summary>Destroy string (you should destroy any strings you explicitly Create).</summary>
		[DllImport("Ultralight")]
		public static extern void ulDestroyString(IntPtr str);

		/// <summary>Get internal UTF-16 buffer data.</summary>
		[GeneratedDllImport("Ultralight", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.LPWStr)]
		public static partial string ulStringGetData(IntPtr str);

		/// <summary>Get length in UTF-16 characters.</summary>
		[DllImport("Ultralight")]
		public static extern uint ulStringGetLength(IntPtr str);

		/// <summary>Whether this string is empty or not.</summary>
		[DllImport("Ultralight")]
		public static extern bool ulStringIsEmpty(IntPtr str);
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ULString16
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public string data_;
		public uint length_;
	}

	public class ULString : IDisposable
	{
		internal IntPtr Ptr { get; private set; }

		public ULString16 ULString16 => Marshal.PtrToStructure<ULString16>(Ptr);

		public ULString(IntPtr ptr) => Ptr = ptr;
		public ULString(string str) => Ptr = Methods.ulCreateStringUTF16(str, (uint)str.Length);

		public bool IsEmpty() => Methods.ulStringIsEmpty(Ptr);
		public string GetData() => Methods.ulStringGetData(Ptr);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString() => GetData();

		#region Disposing

		~ULString() => Dispose();

		public bool IsDisposed { get; private set; } = false;

		public void Dispose()
		{
			Console.WriteLine("disposing");

			if (IsDisposed) return;

			Methods.ulDestroyString(Ptr);

			IsDisposed = true;

			GC.SuppressFinalize(this);
		}

		#endregion Disposing
	}
}
