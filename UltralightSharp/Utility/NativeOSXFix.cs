#if !NETFRAMEWORK && NETCOREAPP3_0_OR_GREATER
using System;
#if NET5_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace ImpromptuNinjas.UltralightSharp.Utility
{
	public static class NativeOSXFix
	{
		private static readonly Lazy<IntPtr> LazyLoadedLibUltralightCore = new Lazy<IntPtr>(() => LoadLib("UltralightCore"), LazyThreadSafetyMode.ExecutionAndPublication);
		private static readonly Lazy<IntPtr> LazyLoadedLibUltralight = new Lazy<IntPtr>(() => LoadLib("Ultralight"), LazyThreadSafetyMode.ExecutionAndPublication);
		private static readonly Lazy<IntPtr> LazyLoadedLibAppCore = new Lazy<IntPtr>(() => LoadLib("AppCore"), LazyThreadSafetyMode.ExecutionAndPublication);
		private static readonly Lazy<IntPtr> LazyLoadedLibWebCore = new Lazy<IntPtr>(() => LoadLib("WebCore"), LazyThreadSafetyMode.ExecutionAndPublication);
		private static readonly Lazy<IntPtr> LazyLoadedIcudata = new Lazy<IntPtr>(() => LoadLib("icudata"), LazyThreadSafetyMode.ExecutionAndPublication);
		private static readonly Lazy<IntPtr> LazyLoadedIcuuc = new Lazy<IntPtr>(() => LoadLib("icuuc"), LazyThreadSafetyMode.ExecutionAndPublication);
		private static readonly Lazy<IntPtr> LazyLoadedIcui18n = new Lazy<IntPtr>(() => LoadLib("icui18n"), LazyThreadSafetyMode.ExecutionAndPublication);
#if NET5_0_OR_GREATER
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#endif
		private static IntPtr LoadLib(string lib)
		{
			string libFullName = $"lib{lib}.dylib";
			string AssmeblyPath = Path.GetDirectoryName(typeof(NativeOSXFix).Assembly.Location) ?? throw new DllNotFoundException("failed to find myself");
			string libFullPath = Path.Combine(AssmeblyPath, libFullName);
			if (File.Exists(libFullPath))
			{
				return NativeLibrary.Load(libFullPath);
			}
			else
			{
				libFullPath = Path.Combine(AssmeblyPath, "runtimes", "osx-x64", libFullName);
				if (File.Exists(libFullPath))
					return NativeLibrary.Load(libFullPath);
			}
			throw new DllNotFoundException($"{lib} was not found");
		}
#if NET5_0_OR_GREATER
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#endif
		public static void Init()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				if (LazyLoadedLibUltralightCore.Value == default)
					throw new DllNotFoundException("UltralightCore");
				if (LazyLoadedLibWebCore.Value == default)
					throw new DllNotFoundException("WebCore");
				if (LazyLoadedLibUltralight.Value == default)
					throw new DllNotFoundException("Ultralight");
				if (LazyLoadedLibAppCore.Value == default)
					throw new DllNotFoundException("AppCore");
			}
		}
	}
}
#endif
