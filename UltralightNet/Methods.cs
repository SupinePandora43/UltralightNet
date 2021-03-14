using System.Runtime.InteropServices;

namespace UltralightNet
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "<Pending>")]
	public static partial class Methods
	{
		static Methods() => Preload();

		/// <summary>
		/// Preload OSX Ultralight binaries
		/// </summary>
		/// <remarks>UltralightCore, WebCore, Ultralight</remarks>
		public static void Preload()
		{
			// i don't have iphone/mac, and probably never
			// so it will not work for ios, i'm sure 100%%%
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				NativeLibrary.Load("UltralightCore");
				NativeLibrary.Load("WebCore");
				NativeLibrary.Load("Ultralight");
			}
		}
	}
}
