using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore;

public static unsafe partial class AppCoreMethods
{
	[DllImport(LibAppCore)]
	public static extern _ULSettings* ulCreateSettings();

	[DllImport(LibAppCore)]
	public static extern void ulDestroySettings(_ULSettings* settings);

	[GeneratedDllImport(LibAppCore)]
	public static partial void ulSettingsSetDeveloperName(_ULSettings* settings, [MarshalUsing(typeof(ULString.ToNative))] string name = "MyCompany");

	[GeneratedDllImport(LibAppCore)]
	public static partial void ulSettingsSetAppName(_ULSettings* settings, [MarshalUsing(typeof(ULString.ToNative))] string name = "MyApp");

	[GeneratedDllImport(LibAppCore)]
	public static partial void ulSettingsSetFileSystemPath(_ULSettings* settings, [MarshalUsing(typeof(ULString.ToNative))] string path = "./assets/");

	[GeneratedDllImport(LibAppCore)]
	public static partial void ulSettingsSetLoadShadersFromFileSystem(_ULSettings* settings, [MarshalAs(UnmanagedType.I1)] bool enabled = false);

	[GeneratedDllImport(LibAppCore)]
	public static partial void ulSettingsSetForceCPURenderer(_ULSettings* settings, [MarshalAs(UnmanagedType.I1)] bool forceCPU = false);
}

public struct _ULSettings : IDisposable, IEquatable<_ULSettings>
{
	public ULString DeveloperName;
	public ULString AppName;

	public ULString FileSystemPath;

	private byte _LoadShadersFromFileSystem = 0;
	public bool LoadShadersFromFileSystem { readonly get => Unsafe.As<byte, bool>(ref Unsafe.AsRef(_LoadShadersFromFileSystem)); set => _LoadShadersFromFileSystem = Unsafe.As<bool, byte>(ref value); }

	private byte _ForceCPURenderer = 0;
	public bool ForceCPURenderer { readonly get => Unsafe.As<byte, bool>(ref Unsafe.AsRef(_ForceCPURenderer)); set => _ForceCPURenderer = Unsafe.As<bool, byte>(ref value); }

	public _ULSettings(ULString developerName, ULString appName, ULString fileSystemPath)
	{
		DeveloperName = developerName;
		AppName = appName;
		FileSystemPath = fileSystemPath;
	}
	public _ULSettings(in ULSettings settings)
	{
		DeveloperName = new(settings.DeveloperName.AsSpan());
		AppName = new(settings.AppName.AsSpan());
		FileSystemPath = new(settings.FileSystemPath.AsSpan());
		LoadShadersFromFileSystem = settings.LoadShadersFromFileSystem;
		ForceCPURenderer = settings.ForceCPURenderer;
	}

	public void Dispose()
	{
		DeveloperName.Dispose();
		AppName.Dispose();
		FileSystemPath.Dispose();
	}

	public readonly bool Equals(_ULSettings settings) =>
		DeveloperName.Equals(settings.DeveloperName) &&
		AppName.Equals(settings.AppName) &&
		FileSystemPath.Equals(settings.FileSystemPath) &&
		LoadShadersFromFileSystem == settings.LoadShadersFromFileSystem &&
		ForceCPURenderer == settings.ForceCPURenderer;
}

public struct ULSettings : IEquatable<ULSettings>
{
	public string DeveloperName = "MyCompany";
	public string AppName = "MyApp";
	public string FileSystemPath = "./assets/";
	public bool LoadShadersFromFileSystem = false;
	public bool ForceCPURenderer = false;

	public ULSettings() { }

	public readonly bool Equals(ULSettings settings) =>
		DeveloperName == settings.DeveloperName &&
		AppName == settings.AppName &&
		FileSystemPath == settings.FileSystemPath &&
		LoadShadersFromFileSystem == settings.LoadShadersFromFileSystem &&
		ForceCPURenderer == settings.ForceCPURenderer;
}
