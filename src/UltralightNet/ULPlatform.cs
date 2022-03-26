using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace UltralightNet
{
	public static partial class Methods
	{
		/// <see cref="ULPlatform"/>

		[DllImport("Ultralight", EntryPoint = "ulPlatformSetLogger")]
		public static extern void ulPlatformSetLogger(ULLogger logger);

		[DllImport("Ultralight", EntryPoint = "ulPlatformSetFileSystem")]
		public static extern void ulPlatformSetFileSystem(ULFileSystem file_system);

		[DllImport("Ultralight", EntryPoint = "ulPlatformSetGPUDriver")]
		public static extern void ulPlatformSetGPUDriver(ULGPUDriver gpu_driver);

		[DllImport("Ultralight", EntryPoint = "ulPlatformSetSurfaceDefinition")]
		public static extern void ulPlatformSetSurfaceDefinition(ULSurfaceDefinition surface_definition);

		[DllImport("Ultralight", EntryPoint = "ulPlatformSetClipboard")]
		public static extern void ulPlatformSetClipboard(ULClipboard clipboard);
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "<Pending>")]
	public static class ULPlatform
	{
		static void ULPLatform() => Methods.Preload();

		private static readonly Dictionary<ULLogger, List<GCHandle>> loggerHandles = new(1);
		private static readonly Dictionary<ULFileSystem, List<GCHandle>> filesystemHandles = new(1);
		private static readonly Dictionary<ULGPUDriver, List<GCHandle>> gpudriverHandles = new(1);
		private static readonly Dictionary<ULClipboard, List<GCHandle>> clipboardHandles = new(1);

		internal static void Handle(ULLogger logger, GCHandle handle)
		{
			if (!loggerHandles.ContainsKey(logger)) loggerHandles.Add(logger, new(1));
			loggerHandles[logger].Add(handle);
		}
		internal static void Handle(ULFileSystem filesystem, GCHandle handle)
		{
			if (!filesystemHandles.ContainsKey(filesystem)) filesystemHandles.Add(filesystem, new(6));
			filesystemHandles[filesystem].Add(handle);
		}
		internal static void Handle(ULGPUDriver gpudriver, GCHandle handle)
		{
			if (!gpudriverHandles.ContainsKey(gpudriver)) gpudriverHandles.Add(gpudriver, new(14));
			gpudriverHandles[gpudriver].Add(handle);
		}
		internal static void Handle(ULClipboard clipboard, GCHandle handle)
		{
			if (!clipboardHandles.ContainsKey(clipboard)) clipboardHandles.Add(clipboard, new(3));
			clipboardHandles[clipboard].Add(handle);
		}

		internal static void Free(ULLogger logger)
		{
			if (loggerHandles.ContainsKey(logger))
			{
				foreach (GCHandle handle in loggerHandles[logger]) if (handle.IsAllocated) handle.Free();
				loggerHandles.Remove(logger);
			}
		}
		internal static void Free(ULFileSystem filesystem)
		{
			if (filesystemHandles.ContainsKey(filesystem))
			{
				foreach (GCHandle handle in filesystemHandles[filesystem]) if (handle.IsAllocated) handle.Free();
				filesystemHandles.Remove(filesystem);
			}
		}
		internal static void Free(ULGPUDriver gpudriver)
		{
			if (gpudriverHandles.ContainsKey(gpudriver))
			{
				foreach (GCHandle handle in gpudriverHandles[gpudriver]) if (handle.IsAllocated) handle.Free();
				gpudriverHandles.Remove(gpudriver);
			}
		}
		internal static void Free(ULClipboard clipboard)
		{
			if (clipboardHandles.ContainsKey(clipboard))
			{
				foreach (GCHandle handle in clipboardHandles[clipboard]) if (handle.IsAllocated) handle.Free();
				clipboardHandles.Remove(clipboard);
			}
		}

		/// <summary>
		/// Frees structures passed to methods
		/// </summary>
		public static void Free()
		{
			foreach (List<GCHandle> handles in loggerHandles.Values) foreach (GCHandle handle in handles) if (handle.IsAllocated) handle.Free();
			foreach (List<GCHandle> handles in filesystemHandles.Values) foreach (GCHandle handle in handles) if (handle.IsAllocated) handle.Free();
			foreach (List<GCHandle> handles in gpudriverHandles.Values) foreach (GCHandle handle in handles) if (handle.IsAllocated) handle.Free();
			foreach (List<GCHandle> handles in clipboardHandles.Values) foreach (GCHandle handle in handles) if (handle.IsAllocated) handle.Free();
		}

		public static bool SetDefaultLogger { get; set; } = true;
		public static bool SetDefaultFileSystem { get; set; } = true;

		public static bool ErrorMissingResources { get; set; } = true;
		public static bool ErrorGPUDriverNotSet { get; set; } = true;

		public static bool ErrorWrongThread { get; set; } = true;
		internal static Thread? thread;
		internal static void CheckThread()
		{
			if (thread is null || !ErrorWrongThread) return;
			if (Thread.CurrentThread != thread) throw new InvalidOperationException($"{nameof(ULPlatform.ErrorWrongThread)}: Use of ultralight api from a different thread.");
		}

		private static ULLogger _logger;
		internal static ULFileSystem _filesystem;
		private static ULGPUDriver _gpudriver;
		private static ULClipboard _clipboard;

		internal static bool gpudriverSet = false;
		internal static bool fileSystemSet = false;

		public static ULLogger Logger
		{
			get => _logger;
			set
			{
				_logger = value;
				Methods.ulPlatformSetLogger(value);
			}
		}
		public static ULFileSystem FileSystem
		{
			get => _filesystem;
			set
			{
				_filesystem = value;
				Methods.ulPlatformSetFileSystem(value);
				fileSystemSet = true;
			}
		}
		public static ULGPUDriver GPUDriver
		{
			get => _gpudriver;
			set
			{
				_gpudriver = value;
				Methods.ulPlatformSetGPUDriver(value);
				gpudriverSet = true;
			}
		}
		public static ULClipboard Clipboard
		{
			get => _clipboard;
			set
			{
				_clipboard = value;
				Methods.ulPlatformSetClipboard(value);
			}
		}

		public static Renderer CreateRenderer(ULConfig config, bool dispose = true)
		{
			thread = Thread.CurrentThread;
			unsafe
			{
				if (SetDefaultLogger && _logger.__LogMessage is null)
				{
					Console.WriteLine("UltralightNet: no logger set, console logger will be used.");

					Logger = new()
					{
						LogMessage = (level, message) => { foreach (string line in message.Split('\n')) { Console.WriteLine($"(UL) {level}: {line}"); } }
					};
				}
				if (SetDefaultFileSystem && !fileSystemSet) // TODO
				{
					Console.WriteLine("UltralightNet: no filesystem set, default (with access only to required files) will be used.");

					List<Stream> files = new();
					Stack<nuint> freeFileIds = new();

					FileSystem = new()
					{
						FileExists = (file) =>
						{
							return file switch
							{
								"resources/cacert.pem" => true,
								"resources/icudt67l.dat" => true,
								"resources/mediaControls.css" => true,
								"resources/mediaControls.js" => true,
								"resources/mediaControlsLocalizedStrings.js" => true,
								_ => false
							};
						},
						GetFileMimeType = (string file, out string result) =>
						{
							result = file switch
							{
								"resources/mediaControls.css" => "text/css",
								"resources/mediaControls.js" => "application/javascript",
								"resources/mediaControlsLocalizedStrings.js" => "application/javascript",
								_ => "application/octet-stream"
							};

							return true;
						},
						OpenFile = (file, _) =>
						{
							nuint id;
							if (freeFileIds.Count is not 0) id = freeFileIds.Pop();
							else id = (nuint)files.Count;
							files.Insert((int)id, file switch
							{
								"resources/cacert.pem" => Resources.Cacertpem!,
								"resources/icudt67l.dat" => Resources.Icudt67ldat!,
								"resources/mediaControls.css" => Resources.MediaControlscss!,
								"resources/mediaControls.js" => Resources.MediaControlsjs!,
								"resources/mediaControlsLocalizedStrings.js" => Resources.MediaControlsLocalizedStringsjs!,
								_ => throw new ArgumentOutOfRangeException(nameof(file), "Tried to open not required file.")
							});

							return id;
						},
						GetFileSize = (nuint handle, out long size) =>
						{
							size = files[(int)handle].Length;
							return true;
						},
						ReadFromFile = (handle, data) =>
						{
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
							return files[(int)handle].Read(data);
#else
							fixed(byte* dataPtr = data)
							{
								UnmanagedMemoryStream unmanagedMemoryStream = new(dataPtr, data.Length, data.Length, FileAccess.Write);
								files[(int)handle].CopyTo(unmanagedMemoryStream);
							}
							return files[(int)handle].Length;
#endif
						},
						CloseFile = (handle) =>
						{
							Console.WriteLine($"CloseFile({handle})");
							files[(int)handle].Close();
							files[(int)handle] = null!;
							freeFileIds.Push(handle);
						}
					};
				}
				else
				{
					ULStringGeneratedDllImportMarshaler m = new("resources/icudt67l.dat");

					if (ErrorMissingResources && (!_filesystem.__FileExists(m.Value)))
					{
						throw new FileNotFoundException($"{typeof(ULFileSystem)} doesn't provide icudt67l.dat from resources/ folder. (Disable error by setting ULPlatform.ErrorMissingResources to false.)");
					}

					m.FreeNative();
				}
			}
			return new Renderer(config, dispose);
		}
	}
}
