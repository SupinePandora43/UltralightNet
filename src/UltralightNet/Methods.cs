using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("UltralightNet.AppCore")]
[assembly: DisableRuntimeMarshalling]
[assembly: AssemblyMetadata("IsTrimmable", "True")]

#if RELEASE
[module: SkipLocalsInit]
#endif

namespace UltralightNet;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "<Pending>")]
public static unsafe partial class Methods
{
	public const string LibUltralight = "Ultralight";

	static Methods() => Preload();

	[LibraryImport(LibUltralight)]
	public static partial byte* ulVersionString();

	[LibraryImport(LibUltralight)]
	public static partial uint ulVersionMajor();

	[LibraryImport(LibUltralight)]
	public static partial uint ulVersionMinor();

	[LibraryImport(LibUltralight)]
	public static partial uint ulVersionPatch();

	/// <summary>
	/// Preload Ultralight binaries on OSX/MacOS
	/// </summary>
	/// <remarks>UltralightCore, WebCore, Ultralight</remarks>
	public static void Preload()
	{
#if NET5_0_OR_GREATER
		bool isOSX = OperatingSystem.IsMacOS();
		if (isOSX)
		{
			ReadOnlySpan<string> libsOSX = new[] { "libUltralightCore.dylib", "libWebCore.dylib", "libUltralight.dylib" };

			string? absoluteAssemblyLocationDir = Path.GetDirectoryName(typeof(Methods).Assembly.Location);
			if (string.IsNullOrEmpty(absoluteAssemblyLocationDir)) return;
			string absoluteRuntimeNativesDir = Path.Combine(absoluteAssemblyLocationDir, "runtimes", "osx-x64", "native");

			Assembly assembly = typeof(UltralightNet.Binaries.Binaries).Assembly;
			DllImportSearchPath searchPath =
				DllImportSearchPath.UseDllDirectoryForDependencies |
				DllImportSearchPath.AssemblyDirectory |
				DllImportSearchPath.ApplicationDirectory;

			foreach (string lib in libsOSX)
			{
				if (!NativeLibrary.TryLoad(lib, assembly, searchPath, out nint _))
				{
					string absoluteRuntimeNative = Path.Combine(absoluteRuntimeNativesDir, lib);
					NativeLibrary.TryLoad(absoluteRuntimeNative, assembly, searchPath, out nint _);
				}
			}
		}
#endif
	}
}
