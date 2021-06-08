using System.IO;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "<Pending>")]
	public static partial class Methods
	{
		static Methods() => Preload();

		[GeneratedDllImport("Ultralight", CharSet = CharSet.Ansi)]
		[return: MarshalAs(UnmanagedType.LPStr)]
		public static partial string ulVersionString();

		[DllImport("Ultralight")]
		public static extern uint ulVersionMajor();

		[DllImport("Ultralight")]
		public static extern uint ulVersionMinor();

		[DllImport("Ultralight")]
		public static extern uint ulVersionPatch();

		/// <summary>
		/// Preload OSX Ultralight binaries
		/// </summary>
		/// <remarks>UltralightCore, WebCore, Ultralight</remarks>
		public static void Preload()
		{
#if NET5_0_OR_GREATER
			// i don't have iphone/mac, and probably never
			// so it will not work for ios, i'm sure 100%%%
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				string[] libs = new[] { "libUltralightCore.dylib", "libWebCore.dylib", "libUltralight.dylib" };
				foreach(string lib in libs)
				{
					string path = Path.Combine(Path.GetDirectoryName(typeof(Methods).Assembly.Location), "runtimes", "osx-x64", "native", lib);
					if (File.Exists(path))
					{
						NativeLibrary.Load(path);
						continue;
					}
					NativeLibrary.Load(lib);
				}
			}
#endif
		}
	}
}
