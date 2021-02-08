using System;
using System.Runtime.InteropServices;

namespace Ultralight
{
	public class Renderer: IDisposable
	{
		public readonly IntPtr ptr;

		public Renderer(Config config)
		{
			Console.WriteLine("Renderer()");
			ptr = Methods.ulCreateRenderer(config.ptr);
		}
		~Renderer()
		{
			Console.WriteLine("~Renderer()");
		}

		public void Dispose()
		{
			Console.WriteLine("Renderer.Dispose()");
			Methods.ulDestroyRenderer(ptr);
		}
	}
	public static partial class Methods
	{
		[DllImport("Ultralight")]
		public static extern IntPtr ulCreateRenderer(IntPtr config);

		[DllImport("Ultralight")]
		public static extern void ulDestroyRenderer(IntPtr renderer);
	}
}
