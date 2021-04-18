using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore
{
	public static partial class AppCoreMethods
	{
		[DllImport("AppCore")]
		public static extern IntPtr ulCreateSettings();

		[DllImport("AppCore")]
		public static extern void ulDestroySettings(IntPtr settings);

		[GeneratedDllImport("AppCore")]
		public static partial void ulSettingsSetDeveloperName(IntPtr settings, [MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))] string name);

		[GeneratedDllImport("AppCore")]
		public static partial void ulSettingsSetAppName(IntPtr settings, [MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))] string name);

		[GeneratedDllImport("AppCore")]
		public static partial void ulSettingsSetFileSystemPath(IntPtr settings, [MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))] string path);

		[GeneratedDllImport("AppCore")]
		public static partial void ulSettingsSetLoadShadersFromFileSystem(IntPtr settings, [MarshalAs(UnmanagedType.I1)] bool enabled);

		[GeneratedDllImport("AppCore")]
		public static partial void ulSettingsSetForceCPURenderer(IntPtr settings, [MarshalAs(UnmanagedType.I1)] bool force_cpu);
	}

	public struct ULSettings_C
	{
		public ULStringMarshaler.ULStringPTR developer_name;
		public ULStringMarshaler.ULStringPTR app_name;
		public ULStringMarshaler.ULStringPTR file_system_path;
		[MarshalAs(UnmanagedType.I1)]
		public bool load_shaders_from_file_system;
		[MarshalAs(UnmanagedType.I1)]
		public bool force_cpu_renderer;
	}

	public class ULSettings : IDisposable
	{
		public IntPtr Ptr { get; private set; }

		public ULSettings(IntPtr ptr, bool dispose = false)
		{
			Ptr = ptr;
			IsDisposed = !dispose;
		}

		public ULSettings(bool dispose = true)
		{
			IsDisposed = !dispose;
			Ptr = AppCoreMethods.ulCreateSettings();
		}
#if NET5_0_OR_GREATER || NET_451 || NETSTANDARD2_0
		public ULSettings_C ULSettings_C => Marshal.PtrToStructure<ULSettings_C>(Ptr);
#else
		public ULSettings_C ULSettings_C => (ULSettings_C)Marshal.PtrToStructure(Ptr, typeof(ULSettings_C));
#endif
		public string DeveloperName
		{
			get => ULSettings_C.developer_name.ToManaged();
			set => AppCoreMethods.ulSettingsSetDeveloperName(Ptr, value);
		}
		public string AppName
		{
			get => ULSettings_C.app_name.ToManaged();
			set => AppCoreMethods.ulSettingsSetAppName(Ptr, value);
		}
		public string FileSystemPath
		{
			get => ULSettings_C.file_system_path.ToManaged();
			set => AppCoreMethods.ulSettingsSetFileSystemPath(Ptr, value);
		}
		public bool LoadShadersFromFileSystem
		{
			get => ULSettings_C.load_shaders_from_file_system;
			set => AppCoreMethods.ulSettingsSetLoadShadersFromFileSystem(Ptr, value);
		}
		public bool ForceCPURenderer
		{
			get => ULSettings_C.force_cpu_renderer;
			set => AppCoreMethods.ulSettingsSetForceCPURenderer(Ptr, value);
		}

		public bool IsDisposed { get; private set; }
		~ULSettings() => Dispose();
		public void Dispose()
		{
			if (IsDisposed) return;

			AppCoreMethods.ulDestroySettings(Ptr);

			IsDisposed = true;
			GC.SuppressFinalize(this);
		}
	}
}
