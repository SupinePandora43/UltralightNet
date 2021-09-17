using System;
using System.IO;
using System.Reflection;
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
		/// Preload Ultralight binaries on OSX/MacOS
		/// </summary>
		/// <remarks>UltralightCore, WebCore, Ultralight</remarks>
		public static void Preload()
		{
#if !NETFRAMEWORK
#if NET5_0_OR_GREATER
			bool isLinux = OperatingSystem.IsLinux();
			bool isOSX = OperatingSystem.IsMacOS();
#else
			bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
			bool isOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
#endif
			if (isLinux || isOSX)
			{
				ReadOnlySpan<string> libsLinux = new[] { "libgstreamer-full-1.0.so", "libUltralightCore.so", "libWebCore.so", "libUltralight.so" };
				ReadOnlySpan<string> libsOSX = new[] { "libgstreamer-full-1.0.dylib", "libUltralightCore.dylib", "libWebCore.dylib", "libUltralight.dylib" };

				string absoluteAssemblyLocationDir = Path.GetDirectoryName(typeof(Methods).Assembly.Location);
				string absoluteRuntimeNativesDir = Path.Combine(absoluteAssemblyLocationDir, "runtimes", "osx-x64", "native");

#if !NETSTANDARD
				Assembly assembly = typeof(Methods).Assembly;
				DllImportSearchPath searchPath =
					DllImportSearchPath.UseDllDirectoryForDependencies |
					DllImportSearchPath.AssemblyDirectory |
					DllImportSearchPath.ApplicationDirectory;
#endif
				foreach (string lib in (isLinux ? libsLinux : libsOSX))
				{
					string absoluteRuntimeNative = Path.Combine(absoluteRuntimeNativesDir, lib);
					if (File.Exists(absoluteRuntimeNative))
					{
						NativeLibrary.Load(absoluteRuntimeNative
#if !NETSTANDARD
							, assembly, searchPath
#endif
							);
						continue;
					}
					else
					{
						string absoluteAssemblyLocation = Path.Combine(absoluteAssemblyLocationDir, lib);
						if (File.Exists(absoluteAssemblyLocation))
						{
							NativeLibrary.Load(absoluteAssemblyLocation
#if !NETSTANDARD
								, assembly, searchPath
#endif
								);
						}
						else
							try
							{
								NativeLibrary.Load(lib
#if !NETSTANDARD
									, assembly, searchPath
#endif
									); // last hope (will not work)
							}
							catch (DllNotFoundException)
							{
#if DEBUG
								Console.WriteLine($"UltralightNet: failed to load {lib}");
#endif
							} // will cause DllNotFoundException somewhere else
					}
				}
			}
#endif
		}

#if NETSTANDARD
		private static partial class NativeLibrary
		{
			public static IntPtr Load(string libraryPath) => dlopen(libraryPath, 0x002); // RTLD_NOW

			// LPUTF8Str = 48
			[GeneratedDllImport("libdl")]
			private static partial IntPtr dlopen([MarshalAs(48)] string path, int mode);
		}
#endif
	}
}
