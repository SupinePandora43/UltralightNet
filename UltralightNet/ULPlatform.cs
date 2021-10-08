using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

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
		internal static readonly Dictionary<ULLogger, GCHandle> loggerHandles = new(1);
		internal static readonly Dictionary<ULFileSystem, GCHandle> filesystemHandles = new(1);
		internal static readonly Dictionary<ULGPUDriver, GCHandle> gpudriverHandles = new(1);
		internal static readonly Dictionary<ULClipboard, GCHandle> clipboardHandles = new(1);

		/// <summary>
		/// Frees structures passed to methods
		/// </summary>
		public static void Free()
		{

		}

#if DEBUG
		public static bool SetDefaultLogger = true;
		public static bool SetDefaultFileSystem = true;
#else
		public static bool SetDefaultLogger = false;
		public static bool SetDefaultFileSystem = false;
#endif
		private static ULLogger _logger;
		private static ULFileSystem _filesystem;

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
			}
		}

		public static Renderer CreateRenderer(ULConfig config = null, bool dispose = true)
		{
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
				if (SetDefaultFileSystem && _filesystem.__GetFileMimeType is null) // TODO
				{
					Console.WriteLine("UltralightNet: no filesystem set, default (with access only to required files) will be used.");

					Dictionary<int, FileStream> files = new();

					int GetFileId()
					{
						for (int i = int.MinValue; i < int.MaxValue; i++)
						{
							if (!files.ContainsKey(i)) return i;
						}
						throw new IndexOutOfRangeException("UltralightNet (Default FileSystem): reached file limit.");
					}

					FileSystem = new()
					{
						FileExists = (path) =>
						{
							Console.WriteLine($"FileExists({path}) = {File.Exists(path)}");
							return File.Exists(path);
						},
						GetFileMimeType = (string file, out string result) =>
						{
							Console.WriteLine($"GetFileMimeType({file})");
							if (file.EndsWith("html"))
								result = "text/html";
							else if (file.EndsWith("js"))
								result = "application/javascript";
							else if (file.EndsWith("css"))
								result = "text/css";
							else
								result = "application/octet-stream";

							return true;
						},
						OpenFile = (file, _) =>
						{
							FileStream fs = File.Open(file, FileMode.Open);
							int id = GetFileId();
							Console.WriteLine($"OpenFile({file}) = {id}");
							files[id] = fs;
							return id;
						},
						GetFileSize = (int handle, out long size) =>
						{
							size = files[handle].Length;
							Console.WriteLine($"GetFileSize({handle}) = {size}");
							return true;
						},
						ReadFromFile = (handle, data, length) =>
						{
							Console.WriteLine($"ReadFromFile({handle})");
#if NET5_0 || NET5_0_OR_GREATER || NETSTANDARD2_1 || NETSTANDARD2_1_OR_GREATER
							return files[handle].Read(data);
#else
							fixed(byte* dataPtr = data)
							{
								UnmanagedMemoryStream unmanagedMemoryStream = new(dataPtr, length);
								files[handle].CopyTo(unmanagedMemoryStream);
							}
							return files[handle].Length;
#endif
						},
						CloseFile = (handle) =>
						{
							Console.WriteLine($"CloseFile({handle})");
							files[handle].Close();
							files.Remove(handle);
						}
					};
				}
			}
			return new Renderer(config ?? new(), dispose);
		}
	}
}
