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

		[DllImport("AppCore")]
		public static extern void ulSettingsSetDeveloperName(IntPtr settings, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string name);

		[DllImport("AppCore")]
		public static extern void ulSettingsSetAppName(IntPtr settings, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string name);

		[DllImport("AppCore")]
		public static extern void ulSettingsSetFileSystemPath(IntPtr settings, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string path);

		[GeneratedDllImport("AppCore")]
		public static extern void ulSettingsSetLoadShadersFromFileSystem(IntPtr settings, bool enabled);

		[GeneratedDllImport("AppCore")]
		public static extern void ulSettingsSetForceCPURenderer(IntPtr settings, bool force_cpu);
	}

	public struct ULSettings_C
	{
		public ULStringMarshaler.ULStringPTR developer_name;
		public ULStringMarshaler.ULStringPTR app_name;
		public ULStringMarshaler.ULStringPTR file_system_path;
		public bool load_shaders_from_file_system;
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

		public ULSettings_C ULSettings_C => Marshal.PtrToStructure<ULSettings_C>(Ptr);

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
