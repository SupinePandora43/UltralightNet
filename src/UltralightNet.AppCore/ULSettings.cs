using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace UltralightNet.AppCore;

public static unsafe partial class AppCoreMethods
{
	[DllImport(LibAppCore)]
	public static extern _ULSettings* ulCreateSettings();

	[DllImport(LibAppCore)]
	public static extern void ulDestroySettings(_ULSettings* settings);

	[LibraryImport(LibAppCore)]
	public static partial void ulSettingsSetDeveloperName(_ULSettings* settings, [MarshalUsing(typeof(ULString))] string name = "MyCompany");

	[LibraryImport(LibAppCore)]
	public static partial void ulSettingsSetAppName(_ULSettings* settings, [MarshalUsing(typeof(ULString))] string name = "MyApp");

	[LibraryImport(LibAppCore)]
	public static partial void ulSettingsSetFileSystemPath(_ULSettings* settings, [MarshalUsing(typeof(ULString))] string path = "./assets/");

	[LibraryImport(LibAppCore)]
	public static partial void ulSettingsSetLoadShadersFromFileSystem(_ULSettings* settings, bool enabled = false);

	[LibraryImport(LibAppCore)]
	public static partial void ulSettingsSetForceCPURenderer(_ULSettings* settings, bool forceCPU = false);
}

[SuppressMessage("Code Rule", "IDE1006: Naming rule violation")]
[CustomMarshaller(typeof(ULSettings), MarshalMode.ManagedToUnmanagedIn, typeof(_ULSettings))]
public struct _ULSettings : IDisposable, IEquatable<_ULSettings>
{
	public ULString DeveloperName;
	public ULString AppName;

	public ULString FileSystemPath;

	public bool LoadShadersFromFileSystem;

	public bool ForceCPURenderer;

	public _ULSettings(ULString developerName, ULString appName, ULString fileSystemPath)
	{
		DeveloperName = developerName;
		AppName = appName;
		FileSystemPath = fileSystemPath;
	}
	public void FromManaged(in ULSettings settings)
	{
		DeveloperName = new(settings.DeveloperName.AsSpan());
		AppName = new(settings.AppName.AsSpan());
		FileSystemPath = new(settings.FileSystemPath.AsSpan());
		LoadShadersFromFileSystem = settings.LoadShadersFromFileSystem;
		ForceCPURenderer = settings.ForceCPURenderer;
	}

	public _ULSettings ToUnmanaged() => this;

	public void Free()
	{
		DeveloperName.Dispose();
		AppName.Dispose();
		FileSystemPath.Dispose();
	}
	void IDisposable.Dispose() => Free();

	public readonly bool Equals(_ULSettings settings) =>
		DeveloperName.Equals(settings.DeveloperName) &&
		AppName.Equals(settings.AppName) &&
		FileSystemPath.Equals(settings.FileSystemPath) &&
		LoadShadersFromFileSystem == settings.LoadShadersFromFileSystem &&
		ForceCPURenderer == settings.ForceCPURenderer;
}

[NativeMarshalling(typeof(_ULSettings))]
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
