using System;
using System.Runtime.InteropServices;

namespace Ultralight
{
    public class Ultralight
    {
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
