#if !NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ImpromptuNinjas.UltralightSharp
{
	internal static partial class Native
	{

		private static readonly Lazy<IntPtr> LazyLoadedLibUltralightCore
		  = new Lazy<IntPtr>(
			() => LoadLib("UltralightCore"),
			LazyThreadSafetyMode.ExecutionAndPublication);

		private static readonly Lazy<IntPtr> LazyLoadedLibUltralight
		  = new Lazy<IntPtr>(
			() => LoadLib("Ultralight"),
			LazyThreadSafetyMode.ExecutionAndPublication);

		private static readonly Lazy<IntPtr> LazyLoadedLibAppCore
		  = new Lazy<IntPtr>(
			() => LoadLib("AppCore"),
			LazyThreadSafetyMode.ExecutionAndPublication);

		private static readonly Lazy<IntPtr> LazyLoadedLibWebCore
		  = new Lazy<IntPtr>(
			() => LoadLib("WebCore"),
			LazyThreadSafetyMode.ExecutionAndPublication);

		// these following 3 are only preloaded by osx, their file names differ per platform

		private static readonly Lazy<IntPtr> LazyLoadedIcudata
		  = new Lazy<IntPtr>(
			() => LoadLib("icudata"),
			LazyThreadSafetyMode.ExecutionAndPublication);

		private static readonly Lazy<IntPtr> LazyLoadedIcuuc
		  = new Lazy<IntPtr>(
			() => LoadLib("icuuc"),
			LazyThreadSafetyMode.ExecutionAndPublication);

		private static readonly Lazy<IntPtr> LazyLoadedIcui18n
		  = new Lazy<IntPtr>(
			() => LoadLib("icui18n"),
			LazyThreadSafetyMode.ExecutionAndPublication);

		private static unsafe IntPtr LoadLib(string libName)
		{
			var asm = typeof(Native).Assembly;
			var baseDir = Path.GetDirectoryName(asm.Location) ?? throw new ArgumentNullException("asm.Location", "can't find base directory");

			IntPtr lib = default;

			string libPath;
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				libPath = Path.Combine(baseDir, $"{libName}.dll");
				if (!TryLoad(libPath, out lib))
					libPath = Path.Combine(baseDir, "runtimes", "win-x64", "native", $"{libName}.dll");
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				libPath = Path.Combine(baseDir, $"lib{libName}.dylib");
				if (!TryLoad(libPath, out lib))
					libPath = Path.Combine(baseDir, "runtimes", "osx-x64", "native", $"lib{libName}.dylib");
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				libPath = Path.Combine(baseDir, $"lib{libName}.so");
				if (!TryLoad(libPath, out lib))
					libPath = Path.Combine(baseDir, "runtimes", $"{(IsMusl() ? "linux-musl-" : "linux-")}{GetProcArchString()}", "native", $"lib{libName}.so");
			}
			else throw new PlatformNotSupportedException();

			if (lib == default)
			{
				lib = NativeLibrary.Load(libPath);
				if (lib == default)
#if !NETFRAMEWORK
					throw new DllNotFoundException(libPath);
#endif
			}

			return lib;
		}

		private static bool TryLoad(string libPath, out IntPtr lib)
		{
			if (File.Exists(libPath))
			{
				try
				{
					lib = NativeLibrary.Load(libPath);
					return true;
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine($"Library loading error: {libPath}\n{ex}");
					lib = default;
					return false;
				}
			}
			else
			{
				lib = default;
				return false;
			}
		}

		public static IntPtr LibUltralightCore => LazyLoadedLibUltralightCore.Value;
		public static IntPtr LibUltralight => LazyLoadedLibUltralight.Value;
		public static IntPtr LibAppCore => LazyLoadedLibAppCore.Value;
		public static IntPtr LibWebCore => LazyLoadedLibWebCore.Value;

		//static Native()
		//  => NativeLibrary.SetDllImportResolver(typeof(Native).Assembly,
		//    (name, assembly, path)
		//      =>
		//    {
		//        switch (name)
		//        {
		//            case "UltralightCore":
		//                Debug.Assert(LibUltralightCore != default);
		//                return LibUltralightCore;
		//            case "Ultralight":
		//                Debug.Assert(LibUltralight != default);
		//                return LibUltralight;
		//            case "AppCore":
		//                Debug.Assert(LibAppCore != default);
		//                return LibAppCore;
		//            case "WebCoreCore":
		//                Debug.Assert(LibWebCore != default);
		//                return LibWebCore;
		//            default:
		//                return default;
		//        }
		//    });

		public static void Init()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				// deal with osx dylib load path ordeal
				if (LibUltralightCore == default)
					throw new PlatformNotSupportedException("Can't preload LibUltralightCore");
				if (LibWebCore == default)
					throw new PlatformNotSupportedException("Can't preload LibWebCore");
				if (LibUltralight == default)
					throw new PlatformNotSupportedException("Can't preload LibUltralight");
				if (LibAppCore == default)
					throw new PlatformNotSupportedException("Can't preload LibAppCore");
			}
			return;
		}
	}
}
#endif
