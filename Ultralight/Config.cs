using System;
using System.Runtime.InteropServices;

namespace Ultralight
{
	public class Config : IDisposable
	{
		public readonly IntPtr ptr;

		public Config()
		{
			Console.WriteLine("Config()");
			ptr = Methods.CreateConfig();
		}
		~Config()
		{
			Console.WriteLine("~Config()");
		}

		public ULString ResourcePath
		{
			set
			{
				Methods.ulConfigSetResourcePath(ptr, value);
			}
		}

		public void Dispose()
		{
			Console.WriteLine("Config.Dispose()");
			Methods.ulDestroyConfig(ptr);
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
