using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UltralightNet.LowStuff;

namespace UltralightNet;

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
public static unsafe class ULPlatform
{
	static void ULPLatform() => Methods.Preload();

	private static readonly Dictionary<ULLogger, List<GCHandle>> loggerHandles = new(1);
	private static readonly Dictionary<ULClipboard, List<GCHandle>> clipboardHandles = new(1);
	private static readonly Dictionary<ULFileSystem, List<GCHandle>> filesystemHandles = new(1);
	private static readonly Dictionary<ULFontLoader, List<GCHandle>> fontloaderHandles = new(1);
	private static readonly Dictionary<ULGPUDriver, List<GCHandle>> gpudriverHandles = new(1);
	private static readonly Dictionary<ULSurfaceDefinition, List<GCHandle>> surfaceHandles = new(1);

	internal static void Handle(ref ULLogger originalLogger, in ULLogger newLogger, Delegate? func = null)
	{
		if (!loggerHandles.Remove(originalLogger, out List<GCHandle>? handles)) handles = new(1);
		if (func is not null) handles!.Add(GCHandle.Alloc(func));
		loggerHandles[originalLogger = newLogger] = handles!;
	}
	internal static void Handle(ref ULClipboard originalClipboard, in ULClipboard newClipboard, Delegate? func = null)
	{
		if (!clipboardHandles.Remove(originalClipboard, out List<GCHandle>? handles)) handles = new(3);
		if (func is not null) handles!.Add(GCHandle.Alloc(func));
		clipboardHandles[originalClipboard = newClipboard] = handles!;
	}
	internal static void Handle(ref ULFileSystem originalFileSystem, in ULFileSystem newFileSystem, Delegate? func = null)
	{
		if (!filesystemHandles.Remove(originalFileSystem, out List<GCHandle>? handles)) handles = new(6);
		if (func is not null) handles!.Add(GCHandle.Alloc(func));
		filesystemHandles[originalFileSystem = newFileSystem] = handles!;
	}
	internal static void Handle(ref ULFontLoader fontLoader, in ULFontLoader newFontLoader, Delegate? func = null)
	{
		if (!fontloaderHandles.Remove(fontLoader, out List<GCHandle>? handles)) handles = new(3);
		if (func is not null) handles!.Add(GCHandle.Alloc(func));
		fontloaderHandles[fontLoader = newFontLoader] = handles;
	}
	internal static void Handle(ref ULGPUDriver originalGPUDriver, in ULGPUDriver newGPUDriver, Delegate? func = null)
	{
		if (!gpudriverHandles.Remove(originalGPUDriver, out List<GCHandle>? handles)) handles = new(14);
		if (func is not null) handles!.Add(GCHandle.Alloc(func));
		gpudriverHandles[originalGPUDriver = newGPUDriver] = handles!;
	}
	internal static void Handle(ref ULSurfaceDefinition originalSurface, in ULSurfaceDefinition newSurface, Delegate? func = null)
	{
		if (!surfaceHandles.Remove(originalSurface, out List<GCHandle>? handles)) handles = new(9);
		if (func is not null) handles!.Add(GCHandle.Alloc(func));
		surfaceHandles[originalSurface = newSurface] = handles!;
	}

#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1)
		private static bool Remove<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TValue? value) where TKey : notnull
		{
			if (dict.ContainsKey(key))
			{
				value = dict[key];
				dict.Remove(key);
				return true;
			}
			else
			{
				value = default;
				return false;
			}
		}
#endif

	// use Dispose()

	internal static void Free(in ULLogger logger)
	{
		if (loggerHandles.Remove(logger, out List<GCHandle>? handles))
			foreach (GCHandle handle in handles!) handle.Free();
	}
	internal static void Free(in ULClipboard clipboard)
	{
		if (clipboardHandles.Remove(clipboard, out List<GCHandle>? handles))
			foreach (GCHandle handle in handles!) handle.Free();
	}
	internal static void Free(in ULFileSystem filesystem)
	{
		if (filesystemHandles.Remove(filesystem, out List<GCHandle>? handles))
			foreach (GCHandle handle in handles!) handle.Free();
	}
	internal static void Free(in ULGPUDriver gpudriver)
	{
		if (gpudriverHandles.Remove(gpudriver, out List<GCHandle>? handles))
			foreach (GCHandle handle in handles!) handle.Free();
	}
	internal static void Free(in ULSurfaceDefinition surface)
	{
		if (surfaceHandles.Remove(surface, out List<GCHandle>? handles))
			foreach (GCHandle handle in handles!) handle.Free();
	}

	/// <summary>
	/// Free structures passed to methods
	/// </summary>
	/// <remarks>
	/// Will make all structs with assigned delegates (not delegate pointers) invalid. (No matter where it is, passed to native or out of scope in managed)
	/// <remarks/>
	public static void Free()
	{
		lock (loggerHandles)
			lock (clipboardHandles)
				lock (filesystemHandles)
					lock (gpudriverHandles)
						lock (surfaceHandles)
						{
							foreach (List<GCHandle> handles in loggerHandles.Values) foreach (GCHandle handle in handles) if (handle.IsAllocated) handle.Free();
							foreach (List<GCHandle> handles in clipboardHandles.Values) foreach (GCHandle handle in handles) if (handle.IsAllocated) handle.Free();
							foreach (List<GCHandle> handles in filesystemHandles.Values) foreach (GCHandle handle in handles) if (handle.IsAllocated) handle.Free();
							foreach (List<GCHandle> handles in gpudriverHandles.Values) foreach (GCHandle handle in handles) if (handle.IsAllocated) handle.Free();
							foreach (List<GCHandle> handles in surfaceHandles.Values) foreach (GCHandle handle in handles) if (handle.IsAllocated) handle.Free();

							loggerHandles.Clear();
							clipboardHandles.Clear();
							filesystemHandles.Clear();
							gpudriverHandles.Clear();
							surfaceHandles.Clear();
						}
	}

	public static bool SetDefaultLogger { get; set; } = true;
	public static bool SetDefaultFileSystem { get; set; } = true;

	public static bool ErrorMissingResources { get; set; } = true;
	public static bool ErrorGPUDriverNotSet { get; set; } = true;

	public static bool ErrorWrongThread { get; set; } = true;

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

	/// <summary>Cheap copy of MEZIANTOU's ROS<char> line enumerator</summary>
	private ref struct LineEnumerator
	{
		private ReadOnlySpan<char> span;

		public LineEnumerator(ReadOnlySpan<char> span)
		{
			this.span = span;
			Current = default;
		}

		public readonly LineEnumerator GetEnumerator() => this;

		public bool MoveNext()
		{
			if (span.Length is 0) return false;
			var index = span.IndexOf('\n');
			if (index == -1)
			{
				Current = new() { line = span };
				span = ReadOnlySpan<char>.Empty;
				return true;
			}
			Current = new() { line = span.Slice(0, index) };
			span = span.Slice(index + 1);
			return true;
		}

		public LineEntry Current { readonly get; private set; }

		public ref struct LineEntry
		{
			internal ReadOnlySpan<char> line;
			public static implicit operator ReadOnlySpan<char>(LineEntry entry) => entry.line;
		}
	}

	public static Renderer CreateRenderer() => CreateRenderer(new());
	public static Renderer CreateRenderer(ULConfig config, bool dispose = true)
	{
		if (config == default(ULConfig)) throw new ArgumentException($"You passed default({nameof(ULConfig)}). It's invalid. Use at least \"new {nameof(ULConfig)}()\"", nameof(config));
		if (SetDefaultFileSystem && !fileSystemSet) // TODO
		{
			Console.WriteLine("UltralightNet: no filesystem set, default (with access only to required files) will be used.");

			FileSystem = new()
			{
				FileExists = (string file) =>
				{
					Console.WriteLine(file);
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
				GetFileMimeType = (string file) =>
				{
					return file switch
					{
						"resources/mediaControls.css" => "text/css",
						"resources/mediaControls.js" => "application/javascript",
						"resources/mediaControlsLocalizedStrings.js" => "application/javascript",
						_ => "application/octet-stream" // or "application/unknown"
					};
				},
				GetFileCharset = (string file) => "utf-8",
				_OpenFile = (ULString* file) =>
				{
					Stream? s = ULString.NativeToManaged(file) switch
					{
						"resources/cacert.pem" => Resources.Cacertpem,
						"resources/icudt67l.dat" => Resources.Icudt67ldat,
						"resources/mediaControls.css" => Resources.MediaControlscss,
						"resources/mediaControls.js" => Resources.MediaControlsjs,
						"resources/mediaControlsLocalizedStrings.js" => Resources.MediaControlsLocalizedStringsjs,
						_ => null
					};

					if (s is not null)
					{
						void* data = NativeMemory.Alloc((nuint)s.Length);
						s.CopyTo(new UnmanagedMemoryStream((byte*)data, s.Length, s.Length, FileAccess.Write));
						ULBuffer buffer = new(data, (nuint)s.Length);
						NativeMemory.Free(data);
						return buffer;
					}
					else return default;
				}
			};
		}
		else
		{
			using ULString m = new("resources/icudt67l.dat".AsSpan()); // C#VER: utf8 (u8)

			if (ErrorMissingResources && _filesystem.__FileExists(&m) is 0)
			{
				throw new FileNotFoundException($"{typeof(ULFileSystem)} doesn't provide icudt67l.dat from resources/ folder. (Disable error by setting ULPlatform.ErrorMissingResources to false.)");
			}
		}
		if (SetDefaultLogger && _logger.__LogMessage is null)
		{
			Console.WriteLine("UltralightNet: no logger set, console logger will be used.");
			Logger = new()
			{
				LogMessage = (ULLogLevel level, string message) => { foreach (ReadOnlySpan<char> line in new LineEnumerator(message.AsSpan())) { Console.WriteLine($"(UL) {level}: {line.ToString()}"); } }
			};
		}
		var returnValue = Renderer.FromHandle((Handle<Renderer>)Methods.ulCreateRenderer(config), true);
		returnValue.ThreadId = Thread.CurrentThread.ManagedThreadId;
		return returnValue;
	}
}
