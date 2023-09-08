using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UltralightNet.Platform;
using UltralightNet.Platform.HighPerformance;

namespace UltralightNet;

public static partial class Methods
{
	[LibraryImport("Ultralight")]
	internal static partial void ulPlatformSetLogger(ULLogger logger);

	[LibraryImport("Ultralight")]
	internal static partial void ulPlatformSetFileSystem(ULFileSystem filesystem);

	[LibraryImport("Ultralight")]
	internal static partial void ulPlatformSetFontLoader(ULFontLoader fontLoader);

	[LibraryImport("Ultralight")]
	internal static partial void ulPlatformSetGPUDriver(ULGPUDriver gpuDriver);

	[LibraryImport("Ultralight")]
	internal static partial void ulPlatformSetSurfaceDefinition(ULSurfaceDefinition surfaceDefinition);

	[LibraryImport("Ultralight")]
	internal static partial void ulPlatformSetClipboard(ULClipboard clipboard);
}

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
public static unsafe class ULPlatform
{
	public static bool EnableDefaultLogger { get; set; } = true;
	/// <summary>
	/// Default filesystrem with access to bundled in assembly files (cacert.pem, icudt67l.dat, mediaControls*).
	/// By disabling that (ULPlatform.SetDefaultFileSystem = false), you will face crash, if no filesystem was set.
	/// </summary>
	public static bool SetDefaultFileSystem { get; set; } = true;
	public static bool SetDefaultFontLoader { get; set; } = true;

	public static bool ErrorMissingResources { get; set; } = true;
	public static bool ErrorGPUDriverNotSet { get; set; } = true;

	public static bool ErrorWrongThread { get; set; } = true;

	static ILogger.Wrapper? loggerWrapper;
	static IFileSystem.Wrapper? filesystemWrapper;
	static IFontLoader.Wrapper? fontloaderWrapper;
	static IGPUDriver.Wrapper? gpuDriverWrapper;
	static ISurfaceDefinition.Wrapper? surfaceDefinitionWrapper;
	static IClipboard.Wrapper? clipboardWrapper;

	public static ILogger Logger { set => Methods.ulPlatformSetLogger((loggerWrapper = new(value)).NativeStruct); }
	public static IFileSystem FileSystem { set => Methods.ulPlatformSetFileSystem((filesystemWrapper = new(value)).NativeStruct); }
	public static IFontLoader FontLoader { set => Methods.ulPlatformSetFontLoader((fontloaderWrapper = new(value)).NativeStruct); }
	public static IGPUDriver GPUDriver { set => Methods.ulPlatformSetGPUDriver((gpuDriverWrapper = new(value)).NativeStruct); }
	public static ISurfaceDefinition SurfaceDefinition { set => Methods.ulPlatformSetSurfaceDefinition((surfaceDefinitionWrapper = new(value)).NativeStruct); }
	public static IClipboard Clipboard { set => Methods.ulPlatformSetClipboard((clipboardWrapper = new(value)).NativeStruct); }

	public static Renderer CreateRenderer() => CreateRenderer(new());
	public static Renderer CreateRenderer(ULConfig config, bool dispose = true)
	{
		if (config == default) throw new ArgumentException($"You passed default({nameof(ULConfig)}). It's invalid. Use at least \"new {nameof(ULConfig)}()\" instead.", nameof(config));

		if (EnableDefaultLogger && loggerWrapper is null)
		{
			Logger = DefaultLogger;
		}
		if (SetDefaultFileSystem && filesystemWrapper is null) FileSystem = config.ResourcePathPrefix is "resources/" ? DefaultFileSystem : throw new ArgumentException("Default file system supports only \"resources\" ResourcePathPrefix", nameof(config));
		else if (ErrorMissingResources && filesystemWrapper is not null)
		{
#if !NETSTANDARD
			var path = config.ResourcePathPrefix + "icudt67l.dat";
			using ULString str = new(path.AsSpan());
			if (filesystemWrapper.NativeStruct.FileExists is null || !filesystemWrapper.NativeStruct.FileExists(&str)) throw new Exception($"{nameof(FileSystem)}.{nameof(IFileSystem.FileExists)}(\"{path}\") returned 'false'. {nameof(ULConfig)}.{nameof(ULConfig.ResourcePathPrefix)} + \"icudt67l.dat\" is required for Renderer creation. (Set {nameof(ULPlatform)}.{nameof(ErrorMissingResources)} to \'false\' to ignore this exception, however, be ready for unhandled crash.)");
#else
			// throw new PlatformNotSupportedException("We're unable to check presence of required files on netstandard");
#endif
		}

		if (SetDefaultFontLoader && fontloaderWrapper is null) throw new Exception($"{nameof(FontLoader)} not set.");

		var returnValue = Renderer.FromHandle(Methods.ulCreateRenderer(in config), true);
		returnValue.ThreadId = Environment.CurrentManagedThreadId;

		returnValue.loggerWrapper = loggerWrapper;
		returnValue.filesystemWrapper = filesystemWrapper;
		returnValue.fontloaderWrapper = fontloaderWrapper;
		returnValue.gpuDriverWrapper = gpuDriverWrapper;
		returnValue.surfaceDefinitionWrapper = surfaceDefinitionWrapper;
		returnValue.clipboardWrapper = clipboardWrapper;

		return returnValue;
	}

	public static ILogger DefaultLogger => new DefaultConsoleLogger();
	public static IFileSystem DefaultFileSystem => new DefaultResourceOnlyFileSystem();

	private sealed class DefaultConsoleLogger : ILogger
	{
		internal DefaultConsoleLogger() { }

		void ILogger.LogMessage(ULLogLevel logLevel, string message)
		{
			foreach (ReadOnlySpan<char> line in new SpanEnumerator<char>(message.AsSpan(), '\n'))
			{
				Console.WriteLine($"(UL) {logLevel}: {line.ToString()}");
			}
		}
#if NET5_0_OR_GREATER
		[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
		private static void LogMessage(ULLogLevel logLevel, ULString* message)
		{
			foreach (ReadOnlySpan<byte> line in new SpanEnumerator<byte>(message->ToSpan(), "\n"u8[0]))
			{
				Console.WriteLine($"(UL) {logLevel}: {Encoding.UTF8.GetString(line)}");
			}
		}
		ULLogger? ILogger.GetNativeStruct() => new() { LogMessage = &LogMessage };
#elif NETSTANDARD2_0
		ULLogger? ILogger.GetNativeStruct() => null;
#endif

		ref struct SpanEnumerator<T> where T : IEquatable<T>
		{
			ReadOnlySpan<T> span;
			readonly T splitter;

			public SpanEnumerator(ReadOnlySpan<T> span, T splitter)
			{
				this.span = span;
				this.splitter = splitter;
				Current = default;
			}

			public readonly SpanEnumerator<T> GetEnumerator() => this;

			public bool MoveNext()
			{
				if (span.Length is 0) return false;
				var index = span.IndexOf(splitter);
				if (index == -1)
				{
					Current = span;
					span = ReadOnlySpan<T>.Empty;
					return true;
				}

#pragma warning disable IDE0057
				Current = span.Slice(0, index);
				span = span.Slice(index + 1);
				return true;
			}

			public ReadOnlySpan<T> Current { readonly get; private set; }
		}
	}
	private sealed class DefaultResourceOnlyFileSystem : IFileSystem
	{
		internal DefaultResourceOnlyFileSystem() { }

		bool IFileSystem.FileExists(string path) => path switch
		{
			"resources/cacert.pem" => true,
			"resources/icudt67l.dat" => true,
			_ => false
		};
		string IFileSystem.GetFileCharset(string path) => "utf-8";
		string IFileSystem.GetFileMimeType(string path) => "application/octet-stream";
		ULBuffer IFileSystem.OpenFile(string path)
		{
			Stream? s = path switch
			{
				"resources/cacert.pem" => Resources.Cacertpem,
				"resources/icudt67l.dat" => Resources.Icudt67ldat,
				_ => null
			};
			if (s is UnmanagedMemoryStream unmanagedMemoryStream) return ULBuffer.CreateFromOwnedData(unmanagedMemoryStream.PositionPointer, checked((nuint)unmanagedMemoryStream.Length));
			else if (s is not null)
			{
				var bytes = new byte[s.Length];
				s.Read(bytes, 0, checked((int)s.Length));
				return ULBuffer.CreateFromDataCopy<byte>(bytes.AsSpan());
			}
			else return default;
		}

#if NET5_0_OR_GREATER
		// TODO: HighPerformance version with utf8 strings
#elif NETSTANDARD2_0
		ULFileSystem? IFileSystem.GetNativeStruct() => null;
#endif
	}
}
