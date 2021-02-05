using System;
using System.Runtime.InteropServices;

namespace Ultralight
{
    public class Ultralight
    {
		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ulVersionString", ExactSpelling = true)]
		public static unsafe extern IntPtr GetVersionString();

		public static string GetVersionStringSafe()
		{
			return Marshal.PtrToStringUTF8(GetVersionString());
		}

		/// <summary>
		/// DOESN't WORK!!!
		/// </summary>
		/// <returns></returns>
		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ulVersionString", ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.LPUTF8Str)]
		public static extern string GetVersionStringMarshal();

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ulCreateConfig", ExactSpelling = true)]
		public static extern IntPtr CreateConfig();

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ulConfigSetResourcePath", ExactSpelling = true)]
		public static extern void ConfigSetResourcePath(IntPtr config, IntPtr resourcePath);

		[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ulCreateString", ExactSpelling = true)]
		public static extern IntPtr CreateString([MarshalAs(UnmanagedType.LPUTF8Str)] string str);
	}

	public class ULString
	{

	}
}
