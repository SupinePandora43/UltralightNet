using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace UltralightNet.AppCore;

[NativeMarshalling(typeof(Marshaller))]
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

	[StructLayout(LayoutKind.Sequential)]
	[CustomMarshaller(typeof(ULSettings), MarshalMode.ManagedToUnmanagedIn, typeof(Marshaller))]
	internal ref struct Marshaller
	{
		private ULString DeveloperName;
		private ULString AppName;

		private ULString FileSystemPath;

		private byte LoadShadersFromFileSystem;

		private byte ForceCPURenderer;

		public void FromManaged(ULSettings settings)
		{
			DeveloperName = new(settings.DeveloperName.AsSpan());
			AppName = new(settings.AppName.AsSpan());
			FileSystemPath = new(settings.FileSystemPath.AsSpan());
			LoadShadersFromFileSystem = Unsafe.As<bool, byte>(ref settings.LoadShadersFromFileSystem);
			ForceCPURenderer = Unsafe.As<bool, byte>(ref settings.ForceCPURenderer);
		}

		public readonly Marshaller ToUnmanaged() => this;

		public void Free()
		{
			DeveloperName.Dispose();
			AppName.Dispose();
			FileSystemPath.Dispose();
		}
	}
}
