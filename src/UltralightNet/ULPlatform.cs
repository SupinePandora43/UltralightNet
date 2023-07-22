using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
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

	[DllImport("Ultralight", EntryPoint = "ulPlatformSetFontLoader")]
	public static extern void ulPlatformSetFontLoader(ULFontLoader fontLoader);

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
	internal static void Free(in ULFontLoader fontLoader)
	{
		if (fontloaderHandles.Remove(fontLoader, out List<GCHandle>? handles))
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
					lock (fontloaderHandles)
						lock (gpudriverHandles)
							lock (surfaceHandles)
							{
								foreach (List<GCHandle> handles in loggerHandles.Values) foreach (GCHandle handle in handles) if (handle.IsAllocated) handle.Free();
								foreach (List<GCHandle> handles in clipboardHandles.Values) foreach (GCHandle handle in handles) if (handle.IsAllocated) handle.Free();
								foreach (List<GCHandle> handles in filesystemHandles.Values) foreach (GCHandle handle in handles) if (handle.IsAllocated) handle.Free();
								foreach (List<GCHandle> handles in fontloaderHandles.Values) foreach (GCHandle handle in handles) if (handle.IsAllocated) handle.Free();
								foreach (List<GCHandle> handles in gpudriverHandles.Values) foreach (GCHandle handle in handles) if (handle.IsAllocated) handle.Free();
								foreach (List<GCHandle> handles in surfaceHandles.Values) foreach (GCHandle handle in handles) if (handle.IsAllocated) handle.Free();

								loggerHandles.Clear();
								clipboardHandles.Clear();
								filesystemHandles.Clear();
								fontloaderHandles.Clear();
								gpudriverHandles.Clear();
								surfaceHandles.Clear();
							}
	}

	public static bool SetDefaultLogger { get; set; } = true;
	/// <summary>
	/// Default filesystrem with access to bundled in assembly files (cacert.pem, icudt67l.dat, mediaControls*).
	/// By disabling that (ULPlatform.SetDefaultFileSystem = false), you will face crash, if no filesystem was set.
	/// </summary>
	public static bool SetDefaultFileSystem { get; set; } = true;

	public static bool ErrorMissingResources { get; set; } = true;
	public static bool ErrorGPUDriverNotSet { get; set; } = true;

	public static bool ErrorWrongThread { get; set; } = true;

	private static ULLogger _logger;
	internal static ULFileSystem _filesystem;
	private static ULFontLoader _fontLoader;
	private static ULGPUDriver _gpudriver;
	private static ULClipboard _clipboard;

	[Obsolete]
	internal static bool gpudriverSet = false;
	[Obsolete]
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
		}
	}
	public static ULFontLoader FontLoader
	{
		get => _fontLoader;
		set
		{
			_fontLoader = value;
			Methods.ulPlatformSetFontLoader(value);
		}
	}
	public static ULGPUDriver GPUDriver
	{
		get => _gpudriver;
		set
		{
			_gpudriver = value;
			Methods.ulPlatformSetGPUDriver(value);
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
#if NET5_0_OR_GREATER
		if (OperatingSystem.IsMacOS() && Environment.CurrentManagedThreadId is not 1) throw new Exception("Renderer can only be created from the main thread on MacOS.");
#endif
		if (SetDefaultLogger && _logger.__LogMessage is null)
		{
			Console.WriteLine("UltralightNet: no logger set, console logger will be used.");
			Logger = new()
			{
				LogMessage = (ULLogLevel level, string message) => { foreach (ReadOnlySpan<char> line in new LineEnumerator(message.AsSpan())) { Console.WriteLine($"(UL) {level}: {line.ToString()}"); } }
			};
		}
		var log = Logger.__LogMessage;
		if (config == default) throw new ArgumentException($"You passed default({nameof(ULConfig)}). It's invalid. Use at least \"new {nameof(ULConfig)}()\" instead.", nameof(config));
		if (_filesystem != default) // some filesystem was set in managed env
		{
			if (_filesystem.__FileExists == null || _filesystem.__GetFileCharset == null || _filesystem.__GetFileMimeType == null || _filesystem.__OpenFile == null) throw new ArgumentException($"Not all callbacks of current {nameof(ULFileSystem)} are set."); // it doesn't implement all required functions
			if (ErrorMissingResources) // some filesystem was set in managed env, optionally check required files
			{
				using ULString icuUL = new(Path.Combine(config.ResourcePathPrefix, "icudt67l.dat").AsSpan());
				if (_filesystem.__FileExists(&icuUL) is false) throw new FileNotFoundException($"Known to UltralightNet {typeof(ULFileSystem)} doesn't provide icudt67l.dat in \"{config.ResourcePathPrefix}\". (Disable this check by setting {nameof(ULPlatform)}.{nameof(ErrorMissingResources)} to false.)");

				using ULString cacertUL = new(Path.Combine(config.ResourcePathPrefix, "cacert.pem").AsSpan());
				if (_filesystem.__FileExists(&cacertUL) is false) if (log is not null)
					{
						using ULString noticeMsg = new("(UltralightNet) FileSystem doesn't provide \"cacert.pem\", no network functionality will be available."u8);
						log(ULLogLevel.Warning, &noticeMsg);
					}
			}
		}
		else if (SetDefaultFileSystem) // no filesystem was set in managed env, optionally set default
		{
			if (log is not null)
			{
				using ULString noticeMsg = new("(UltralightNet) No FileSystem set, default (with access only to required files) will be used."u8);
				log(ULLogLevel.Info, &noticeMsg);
			}

#if NET5_0_OR_GREATER
			[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
#endif
			static ULString* GetFileMimeType(ULString* file)
			{
				var span = file->ToString();
				ReadOnlySpan<byte> result;

				if (span.EndsWith(".css")) result = "text/css"u8;
				else if (span.EndsWith(".js")) result = "application/javascript"u8;
				else result = "application/octet-stream"u8;

				ULString returnValue = new(result);
				return returnValue.Allocate();
			}

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
#if NET5_0_OR_GREATER
				__GetFileMimeType = &GetFileMimeType,
#else
				_GetFileMimeType = GetFileMimeType,
#endif
				GetFileCharset = (string file) => "utf-8",
				_OpenFile = (ULString* file) =>
				{
					Stream? s = file->ToString() switch
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
						ULBuffer buffer;

						if (s is UnmanagedMemoryStream unmanagedMemoryStream) buffer = ULBuffer.CreateFromOwnedData(unmanagedMemoryStream.PositionPointer, checked((nuint)unmanagedMemoryStream.Length));
						else
						{
							var bytes = new byte[s.Length];
							int len = s.Read(bytes, 0, checked((int)s.Length));
#pragma warning disable IDE0057
							buffer = ULBuffer.CreateFromDataCopy<byte>(bytes.AsSpan().Slice(0, len));
#pragma warning restore IDE0057
						}

						GC.KeepAlive(s);

						return buffer;
					}
					else return default;
				}
			};
		}
		var returnValue = Renderer.FromHandle(Methods.ulCreateRenderer(in config), true);
		returnValue.ThreadId = Environment.CurrentManagedThreadId;
		return returnValue;
	}
}
