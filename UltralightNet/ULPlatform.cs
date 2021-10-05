using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		/// <see cref="ULPlatform"/>

		[DllImport("Ultralight", EntryPoint = "ulPlatformSetLogger")]
		public static extern void ulPlatformSetLogger(ULLogger logger);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "<Pending>")]
	public static class ULPlatform
	{
		private static readonly List<GCHandle> handles = new();
		internal static readonly Dictionary<ULLogger, GCHandle> loggerHandles = new(1);

		private static void Handle(GCHandle handle)
		{
			handles.Add(handle);
		}

		/// <summary>
		/// Frees structures passed to methods
		/// </summary>
		public static void Free()
		{
			foreach (GCHandle handle in handles)
			{
				handle.Free();
			}
			handles.Clear();
		}

		private static bool isLoggerSet = false;
		private static ULLogger _logger;
		public static ULLogger Logger
		{
			get => _logger;
			set
			{
				unsafe
				{
					if (value.__LogMessage is null)
					{
						isLoggerSet = false;
						_logger = value;
						Methods.ulPlatformSetLogger(value);
						return;
					}
				}
				if (!isLoggerSet)
				{
					isLoggerSet = true;
					_logger = value;
					Methods.ulPlatformSetLogger(value);
				}
				else
				{
					_logger = value;
					Methods.ulPlatformSetLogger(value);
				}
			}
		}

		[DllImport("Ultralight", EntryPoint = "ulPlatformSetFileSystem")]
		public static extern void SetFileSystem(_ULFileSystem file_system);
		[DllImport("Ultralight", EntryPoint = "ulPlatformSetGPUDriver")]
		public static extern void SetGPUDriver(_ULGPUDriver gpu_driver);
		[DllImport("Ultralight", EntryPoint = "ulPlatformSetSurfaceDefinition")]
		public static extern void SetSurfaceDefinition(_ULSurfaceDefinition surface_definition);
		[DllImport("Ultralight", EntryPoint = "ulPlatformSetClipboard")]
		public static extern void SetClipboard(_ULClipboard clipboard);

		[DllImport("Ultralight", EntryPoint = "ulPlatformSetFileSystem")]
		private static extern void SetFileSystem__PInvoke__(ULFileSystem file_system);
		[DllImport("Ultralight", EntryPoint = "ulPlatformSetGPUDriver")]
		private static extern void SetGPUDriver__PInvoke__(ULGPUDriver gpu_driver);
		[DllImport("Ultralight", EntryPoint = "ulPlatformSetSurfaceDefinition")]
		private static extern void SetSurfaceDefinition__PInvoke__(ULSurfaceDefinition surface_definition);
		[DllImport("Ultralight", EntryPoint = "ulPlatformSetClipboard")]
		private static extern void SetClipboard__PInvoke__(ULClipboard clipboard);

		public static void SetFileSystem(ULFileSystem fs)
		{
			Handle(GCHandle.Alloc(fs, GCHandleType.Normal));
			SetFileSystem__PInvoke__(fs);
		}
		public static void SetGPUDriver(ULGPUDriver gpu_driver)
		{
			Handle(GCHandle.Alloc(gpu_driver, GCHandleType.Normal));
			SetGPUDriver__PInvoke__(gpu_driver);
		}
		public static void SetSurfaceDefinition(ULSurfaceDefinition surface_definition)
		{
			Handle(GCHandle.Alloc(surface_definition, GCHandleType.Normal));
			SetSurfaceDefinition__PInvoke__(surface_definition);
		}
		public static void SetClipboard(ULClipboard clipboard)
		{
			Handle(GCHandle.Alloc(clipboard, GCHandleType.Normal));
			SetClipboard__PInvoke__(clipboard);
		}
	}
}
