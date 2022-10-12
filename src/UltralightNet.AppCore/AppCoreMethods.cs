using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "<Pending>")]
public unsafe static partial class AppCoreMethods
{
	static AppCoreMethods() => Methods.Preload();

	[DllImport("AppCore")]
	public static extern void ulEnablePlatformFontLoader();

	#region ulEnablePlatformFileSystem
	[DllImport("AppCore", EntryPoint = "ulEnablePlatformFileSystem", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
	private static extern void ulEnablePlatformFileSystemActual(ULString* baseDirectory);

	public static void ulEnablePlatformFileSystem(ULString* baseDirectory)
	{
		ulEnablePlatformFileSystemActual(baseDirectory);

		ULPlatform.SetDefaultFileSystem = false;
		ULPlatform.ErrorMissingResources = false;
	}

	public static void ulEnablePlatformFileSystem(string baseDirectory) => ulEnablePlatformFileSystem(baseDirectory.AsSpan());

	public static void ulEnablePlatformFileSystem(ReadOnlySpan<char> baseDirectory)
	{
		using ULString baseDirectoryUL = new(baseDirectory);
		ulEnablePlatformFileSystem(&baseDirectoryUL);
	}

	public static void ulEnablePlatformFileSystem(ReadOnlySpan<byte> baseDirectory)
	{
		using ULString baseDirectoryUL = new(baseDirectory);
		ulEnablePlatformFileSystem(&baseDirectoryUL);
	}
	#endregion ulEnablePlatformFileSystem
	#region ulEnableDefaultLogger
	[DllImport("AppCore", EntryPoint = "ulEnableDefaultLogger", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
	private static extern void ulEnableDefaultLoggerActual(ULString* logPath);

	public static void ulEnableDefaultLogger(ULString* logPath){
		ulEnableDefaultLoggerActual(logPath);

		ULPlatform.SetDefaultLogger = false;
	}

	public static void ulEnableDefaultLogger(string logPath) => ulEnableDefaultLogger(logPath.AsSpan());

	public static void ulEnableDefaultLogger(ReadOnlySpan<char> logPath)
	{
		using ULString logPathUL = new(logPath);
		ulEnableDefaultLogger(&logPathUL);
	}

	public static void ulEnableDefaultLogger(ReadOnlySpan<byte> logPath)
	{
		using ULString logPathUL = new(logPath);
		ulEnableDefaultLogger(&logPathUL);
	}
	#endregion ulEnableDefaultLogger
}
