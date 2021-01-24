#if !NETFRAMEWORK && NETCOREAPP3_0_OR_GREATER
using System;
#if NET5_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;

namespace ImpromptuNinjas.UltralightSharp.Utility
{
	internal static class NativeOSXFix
	{
		private static readonly Assembly assembly = typeof(Ultralight).Assembly;
#if NET5_0_OR_GREATER
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#endif
		private static IntPtr LoadLib(string lib)
		{
			IntPtr library = NativeLibrary.Load(lib, assembly, DllImportSearchPath.AssemblyDirectory);
			if (library == default)
			{
				string libFullName = $"lib{lib}.dylib";
				string AssmeblyPath = Path.GetDirectoryName(assembly.Location) ?? throw new DllNotFoundException("failed to find myself");
				library = NativeLibrary.Load(Path.Combine(AssmeblyPath, libFullName));
				if (library == default)
				{
					library = NativeLibrary.Load(Path.Combine(AssmeblyPath, "runtimes", "osx-x64", libFullName));
					if (library == default)
						throw new DllNotFoundException($"failed to find {lib}");
				}
			}
			return library;
		}
#if NET5_0_OR_GREATER
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#endif
		public static void Init()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				if (LoadLib("UltralightCore") == default)
					throw new DllNotFoundException("UltralightCore");
				if (LoadLib("WebCore") == default)
					throw new DllNotFoundException("WebCore");
				if (LoadLib("Ultralight") == default)
					throw new DllNotFoundException("Ultralight");
				if (LoadLib("AppCore") == default)
					throw new DllNotFoundException("AppCore");
			}
		}
	}
}
#endif
