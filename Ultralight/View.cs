using System;
using System.Runtime.InteropServices;

namespace Ultralight
{
	public static partial class Methods
	{
		[DllImport("Ultralight")]
		public static extern IntPtr ulCreateView(IntPtr renderer, uint width, uint height, bool transparent, IntPtr session, bool force_cpu_renderer);

		[DllImport("Ultralight")]
		public static extern void ulViewLoadURL(IntPtr view, IntPtr urlString);
	}
	public class View
	{
		public readonly IntPtr ptr;

		public View(IntPtr renderer, uint width, uint height, bool transparent, IntPtr session, bool force_cpu_renderer = false)
		{
			ptr = Methods.ulCreateView(renderer, width, height, transparent, session, force_cpu_renderer);
		}

		public void LoadURL(IntPtr ulStringPtr)
		{
			Methods.ulViewLoadURL(ptr, ulStringPtr);
		}
		public void LoadURL(ULString ulString)
		{
			LoadURL(ulString.ptr);
		}
		public void LoadURL(string str)
		{
			LoadURL((ULString)str);
		}
	}
}
