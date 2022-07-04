using System.IO;
using System.Reflection;

namespace UltralightNet;

public static class Resources
{
	private static readonly Assembly assembly = typeof(Resources).Assembly;

	public static Stream? Cacertpem => assembly.GetManifestResourceStream("UltralightNet.resources.cacert.pem");
	public static Stream? Icudt67ldat => assembly.GetManifestResourceStream("UltralightNet.resources.icudt67l.dat");
	public static Stream? MediaControlscss => assembly.GetManifestResourceStream("UltralightNet.resources.mediaControls.css");
	public static Stream? MediaControlsjs => assembly.GetManifestResourceStream("UltralightNet.resources.mediaControls.js");
	public static Stream? MediaControlsLocalizedStringsjs => assembly.GetManifestResourceStream("UltralightNet.resources.mediaControlsLocalizedStrings.js");
}
