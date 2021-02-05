using System;
using System.Runtime.InteropServices;

namespace Ultralight
{
	public class Config
	{
		public readonly IntPtr ptr;

		public Config()
		{
			ptr = Methods.CreateConfig();
		}
		~Config()
		{
			Methods.ulDestroyConfig(ptr);
		}

		public ULString ResourcePath
		{
			set
			{
				Methods.ulConfigSetResourcePath(ptr, value);
			}
		}
		public static implicit operator IntPtr(Config config) => config.ptr;
		public static class Methods
		{
			[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ulCreateConfig", ExactSpelling = true)]
			public static extern IntPtr CreateConfig();

			[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
			public static extern void ulDestroyConfig(IntPtr config);

			[DllImport("Ultralight", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
			public static extern void ulConfigSetResourcePath(IntPtr config, IntPtr resource_path);
		}
	}
}
