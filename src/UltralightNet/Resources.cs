using System.IO;
using System.Reflection;

namespace UltralightNet;

public static class Resources
{
	private static Assembly Assembly => typeof(Resources).Assembly;

	public static Stream? Cacertpem => Assembly.GetManifestResourceStream("UltralightNet.resources.cacert.pem");
	public static Stream? Icudt67ldat => Assembly.GetManifestResourceStream("UltralightNet.resources.icudt67l.dat");
	public static Stream? MediaControlscss => Assembly.GetManifestResourceStream("UltralightNet.resources.mediaControls.css");
	public static Stream? MediaControlsjs => Assembly.GetManifestResourceStream("UltralightNet.resources.mediaControls.js");
	public static Stream? MediaControlsLocalizedStringsjs => Assembly.GetManifestResourceStream("UltralightNet.resources.mediaControlsLocalizedStrings.js");
}
