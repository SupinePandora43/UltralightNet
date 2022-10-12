using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet;

public static unsafe partial class Methods
{
	/// <summary>Create config with default values (see <Ultralight/platform/Config.h>).</summary>
	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern _ULConfig* ulCreateConfig();

	/// <summary>Destroy config.</summary>
	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern void ulDestroyConfig(_ULConfig* config);

	/// <summary>Set the file path to a writable directory that will be used to store cookies, cached resources, and other persistent data.</summary>
	[Obsolete]
	[GeneratedDllImport(LibUltralight)]
	public static partial void ulConfigSetCachePath(_ULConfig* config, [MarshalUsing(typeof(ULString.ToNative))] string cachePath = "");

	[Obsolete]
	[GeneratedDllImport(LibUltralight)]
	public static partial void ulConfigSetResourcePathPrefix(_ULConfig* config, [MarshalUsing(typeof(ULString.ToNative))] string resourcePathPrefix = "resources/");

	/// <summary>The winding order for front-facing triangles.</summary>
	/// <see cref="ULFaceWinding"/>
	/// <remarks>This is only used with custom GPUDrivers</remarks>
	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern void ulConfigSetFaceWinding(_ULConfig* config, ULFaceWinding winding);

	/// <summary>The hinting algorithm to use when rendering fonts.</summary>
	/// <see cref="ULFontHinting"/>
	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern void ulConfigSetFontHinting(_ULConfig* config, ULFontHinting font_hinting = ULFontHinting.Normal);

	/// <summary>The gamma to use when compositing font glyphs, change this value to adjust contrast (Adobe and Apple prefer 1.8, others may prefer 2.2).</summary>
	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern void ulConfigSetFontGamma(_ULConfig* config, double font_gamma = 1.8);

	/// <summary>Set user stylesheet (CSS) (Default = Empty).</summary>
	[Obsolete]
	[GeneratedDllImport(LibUltralight)]
	public static partial void ulConfigSetUserStylesheet(_ULConfig* config, [MarshalUsing(typeof(ULString.ToNative))] string font_name);

	/// <summary>Set whether or not we should continuously repaint any Views or compositor layers, regardless if they are dirty or not. This is mainly used to diagnose painting/shader issues.</summary>
	[Obsolete]
	[GeneratedDllImport(LibUltralight)]
	public static partial void ulConfigSetForceRepaint(_ULConfig* config, [MarshalAs(UnmanagedType.I1)] bool enabled = false);

	/// <summary>Set the amount of time to wait before triggering another repaint when a CSS animation is active.</summary>
	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern void ulConfigSetAnimationTimerDelay(_ULConfig* config, double delay = 1.0 / 60.0);

	/// <summary>When a smooth scroll animation is active, the amount of time (in seconds) to wait before triggering another repaint.</summary>
	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern void ulConfigSetScrollTimerDelay(_ULConfig* config, double delay = 1.0 / 60.0);

	/// <summary>The amount of time (in seconds) to wait before running the recycler (will attempt to return excess memory back to the system).</summary>
	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern void ulConfigSetRecycleDelay(_ULConfig* config, double delay = 4.0);

	/// <summary>Set the size of WebCore's memory cache for decoded images, scripts, and other assets in bytes.</summary>
	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern void ulConfigSetMemoryCacheSize(_ULConfig* config, uint size = 64 * 1024 * 1024);

	/// <summary>Set the number of pages to keep in the cache.</summary>
	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern void ulConfigSetPageCacheSize(_ULConfig* config, uint size = 0);

	/// <summary>
	/// JavaScriptCore tries to detect the system's physical RAM size to set
	/// reasonable allocation limits. Set this to anything other than 0 to
	/// override the detected value. Size is in bytes.
	/// <br/>
	/// This can be used to force JavaScriptCore to be more conservative with
	/// its allocation strategy (at the cost of some performance).
	/// </summary>
	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern void ulConfigSetOverrideRAMSize(_ULConfig* config, uint size = 0);

	/// <summary>The minimum size of large VM heaps in JavaScriptCore. Set this to a lower value to make these heaps start with a smaller initial value.</summary>
	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern void ulConfigSetMinLargeHeapSize(_ULConfig* config, uint size = 32 * 1024 * 1024);

	/// <summary>The minimum size of small VM heaps in JavaScriptCore. Set this to a lower value to make these heaps start with a smaller initial value.</summary>
	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern void ulConfigSetMinSmallHeapSize(_ULConfig* config, uint size = 1 * 1024 * 1024);

	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern void ulConfigSetNumRendererThreads(_ULConfig* config, uint count = 0);

	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern void ulConfigSetMaxUpdateTime(_ULConfig* config, double time = 0.01);

	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern void ulConfigSetBitmapAlignment(_ULConfig* config, uint alignment = 16);
}

/// <summary>Configuration settings for Ultralight.</summary>
[StructLayout(LayoutKind.Sequential)]
[CustomTypeMarshaller(typeof(ULConfig), CustomTypeMarshallerKind.Value, Direction = CustomTypeMarshallerDirection.In, Features = CustomTypeMarshallerFeatures.TwoStageMarshalling)]
public unsafe struct _ULConfig : IDisposable
{
	/// <summary>The file path to a writable directory that will be used to store cookies, cached resources, and other persistent data.</summary>
	public ULString CachePath;
	/// <summary>Path to use for filesystem resources.</summary>
	public ULString ResourcePathPrefix;

	private byte _FaceWinding = (byte)ULFaceWinding.CounterClockwise;
	/// <summary>Face winding for ULGPUDriver.</summary>
	public ULFaceWinding FaceWinding { readonly get => Unsafe.As<byte, ULFaceWinding>(ref Unsafe.AsRef(_FaceWinding)); set => _FaceWinding = Unsafe.As<ULFaceWinding, byte>(ref value); }

	private byte _FontHinting = (byte)ULFontHinting.Normal;
	/// <summary>The hinting algorithm to use when rendering fonts.</summary>
	/// <see cref="ULFontHinting"/>
	public ULFontHinting FontHinting { readonly get => Unsafe.As<byte, ULFontHinting>(ref Unsafe.AsRef(_FontHinting)); set => _FontHinting = Unsafe.As<ULFontHinting, byte>(ref value); }
	/// <summary>The gamma to use when compositing font glyphs, change this value to adjust contrast (Adobe and Apple prefer 1.8, others may prefer 2.2).</summary>
	public double FontGamma = 1.8;

	/// <summary>Default user style (CSS).</summary>
	public ULString UserStylesheet;

	private byte _ForceRepaint = 0;
	/// <summary>Whether or not we should continuously repaint any Views or compositor layers, regardless if they are dirty or not.</summary>
	/// <remarks>This is mainly used to diagnose painting/shader issues.</remarks>
	public bool ForceRepaint { readonly get => Unsafe.As<byte, bool>(ref Unsafe.AsRef(_ForceRepaint)); set => _ForceRepaint = Unsafe.As<bool, byte>(ref value); }

	/// <summary>When a CSS animation is active, the amount of time (in seconds) to wait before triggering another repaint.</summary>
	public double AnimationTimerDelay = 1.0 / 60.0;
	/// <summary>When a smooth scroll animation is active, the amount of time (in seconds) to wait before triggering another repaint.</summary>
	public double ScrollTimerDelay = 1.0 / 60.0;
	/// <summary>The amount of time (in seconds) to wait before running the recycler (will attempt to return excess memory back to the system).</summary>
	public double RecycleDelay = 4.0;

	/// <summary>Size of WebCore's memory cache in bytes.</summary>
	/// <remarks>You should increase this if you anticipate handling pages with large resources, Safari typically uses 128+ MiB for its cache.</remarks>
	public uint MemoryCacheSize = 64 * 1024 * 1024;
	/// <summary>Number of pages to keep in the cache. Defaults to 0 (none).</summary>
	/// <remarks>Safari typically caches about 5 pages and maintains an on-disk cache to support typical web-browsing activities. If you increase this, you should probably increase the memory cache size as well.</remarks>
	public uint PageCacheSize = 0;
	/// <summary>JavaScriptCore tries to detect the system's physical RAM size to set reasonable allocation limits. Set this to anything other than 0 to override the detected value. Size is in bytes.</summary>
	/// <remarks>This can be used to force JavaScriptCore to be more conservative with its allocation strategy (at the cost of some performance).</remarks>
	public uint OverrideRAMSize = 0;
	/// <summary>The minimum size of large VM heaps in JavaScriptCore.</summary>
	/// <remarks>Set this to a lower value to make these heaps start with a smaller initial value.</remarks>
	public uint MinLargeHeapSize = 32 * 1024 * 1024;
	/// <summary>The minimum size of small VM heaps in JavaScriptCore.</summary>
	/// <remarks>Set this to a lower value to make these heaps start with a smaller initial value.</remarks>
	public uint MinSmallHeapSize = 1 * 1024 * 1024;

	/// <summary>The number of threads to use in the Renderer (for parallel painting on the CPU, etc.).</summary>
	/// <remarks>If this value is 0 (the default), the number of threads will be determined at runtime using the following formula: <c>max(PhysicalProcessorCount() - 1, 1)</c></remarks>
	public uint NumRendererThreads = 0;

	/// <summary>The max amount of time (in seconds) to allow <see cref="Renderer.Update()"/> to run per call.</summary>
	/// <remarks>The library will attempt to throttle timers and/or reschedule work if this time budget is exceeded.</remarks>
	public double MaxUpdateTime = 1.0 / 100.0;

	/// <summary>The alignment (in bytes) of the BitmapSurface when using the CPU renderer.</summary>
	/// <remarks>You can set this to '0' to perform no padding (row_bytes will always be width * 4) at a slight cost to performance.</remarks>
	public uint BitmapAlignment = 16;

	/// <example>
	/// using <see cref="ULString" /> cache = new("./cache");
	/// using <see cref="ULString" /> resources = new("resources/");
	/// using <see cref="ULString" /> styles = new("/* idk anything about css */");
	///
	/// _ULConfig config = new(cache, resources, styles);
	///
	/// // TODO: _ULConfig usage???
	/// </example>
	public _ULConfig(ULString cachePath, ULString resourcePathPrefix, ULString userStylesheet)
	{
		CachePath = cachePath;
		ResourcePathPrefix = resourcePathPrefix;
		UserStylesheet = userStylesheet;
	}

	public _ULConfig(in ULConfig config)
	{
#if NET5_0_OR_GREATER
		Unsafe.SkipInit(out this);
#endif
		CachePath = new(config.CachePath.AsSpan());
		ResourcePathPrefix = new ULString(config.ResourcePathPrefix.AsSpan());
		_FaceWinding = Unsafe.As<ULFaceWinding, byte>(ref Unsafe.AsRef(config.FaceWinding));
		_FontHinting = Unsafe.As<ULFontHinting, byte>(ref Unsafe.AsRef(config.FontHinting));
		FontGamma = config.FontGamma;
		UserStylesheet = new(config.UserStylesheet.AsSpan());
		_ForceRepaint = Unsafe.As<bool, byte>(ref Unsafe.AsRef(config.ForceRepaint));
		AnimationTimerDelay = config.AnimationTimerDelay;
		ScrollTimerDelay = config.ScrollTimerDelay;
		RecycleDelay = config.RecycleDelay;
		MemoryCacheSize = config.MemoryCacheSize;
		PageCacheSize = config.PageCacheSize;
		OverrideRAMSize = config.OverrideRAMSize;
		MinLargeHeapSize = config.MinLargeHeapSize;
		MinSmallHeapSize = config.MinSmallHeapSize;
		NumRendererThreads = config.NumRendererThreads;
		MaxUpdateTime = config.MaxUpdateTime;
		BitmapAlignment = config.BitmapAlignment;
	}

	public void Dispose()
	{
		CachePath.Dispose();
		ResourcePathPrefix.Dispose();
		UserStylesheet.Dispose();
	}
}
/// <inheritdoc cref="_ULConfig" />
[NativeMarshalling(typeof(_ULConfig))]
public struct ULConfig : IEquatable<ULConfig>
{
	/// <inheritdoc cref="_ULConfig.CachePath" />
	public string CachePath = string.Empty;
	/// <inheritdoc cref="_ULConfig.ResourcePathPrefix" />
	public string ResourcePathPrefix = "resources/";

	/// <inheritdoc cref="_ULConfig.FaceWinding" />
	public ULFaceWinding FaceWinding = ULFaceWinding.CounterClockwise;

	/// <inheritdoc cref="_ULConfig.FontHinting" />
	public ULFontHinting FontHinting = ULFontHinting.Normal;
	/// <inheritdoc cref="_ULConfig.FontGamma" />
	public double FontGamma = 1.8;

	/// <inheritdoc cref="_ULConfig.UserStylesheet" />
	public string UserStylesheet = string.Empty;

	/// <inheritdoc cref="_ULConfig.ForceRepaint" />
	public bool ForceRepaint = false;

	/// <inheritdoc cref="_ULConfig.AnimationTimerDelay" />
	public double AnimationTimerDelay = 1.0 / 60.0;
	/// <inheritdoc cref="_ULConfig.ScrollTimerDelay" />
	public double ScrollTimerDelay = 1.0 / 60.0;
	/// <inheritdoc cref="_ULConfig.RecycleDelay" />
	public double RecycleDelay = 4.0;

	/// <inheritdoc cref="_ULConfig.MemoryCacheSize" />
	public uint MemoryCacheSize = 64 * 1024 * 1024;
	/// <inheritdoc cref="_ULConfig.PageCacheSize" />
	public uint PageCacheSize = 0;
	/// <inheritdoc cref="_ULConfig.OverrideRAMSize" />
	public uint OverrideRAMSize = 0;
	/// <inheritdoc cref="_ULConfig.MinLargeHeapSize" />
	public uint MinLargeHeapSize = 32 * 1024 * 1024;
	/// <inheritdoc cref="_ULConfig.MinSmallHeapSize" />
	public uint MinSmallHeapSize = 1 * 1024 * 1024;

	/// <inheritdoc cref="_ULConfig.NumRendererThreads" />
	public uint NumRendererThreads = 0;

	/// <inheritdoc cref="_ULConfig.MaxUpdateTime" />
	public double MaxUpdateTime = 1.0 / 100.0;

	/// <inheritdoc cref="_ULConfig.BitmapAlignment" />
	public uint BitmapAlignment = 16;

	public ULConfig() { }

	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is ULConfig ? Equals((ULConfig)obj) : false;
	public readonly bool Equals(ULConfig other) =>
		CachePath == other.CachePath &&
		ResourcePathPrefix == other.ResourcePathPrefix &&
		FaceWinding == other.FaceWinding &&
		FontHinting == other.FontHinting &&
		FontGamma == other.FontGamma &&
		UserStylesheet == other.UserStylesheet &&
		ForceRepaint == other.ForceRepaint &&
		AnimationTimerDelay == other.AnimationTimerDelay &&
		ScrollTimerDelay == other.ScrollTimerDelay &&
		RecycleDelay == other.RecycleDelay &&
		MemoryCacheSize == other.MemoryCacheSize &&
		PageCacheSize == other.PageCacheSize &&
		OverrideRAMSize == other.OverrideRAMSize &&
		MinLargeHeapSize == other.MinLargeHeapSize &&
		MinSmallHeapSize == other.MinSmallHeapSize &&
		NumRendererThreads == other.NumRendererThreads &&
		MaxUpdateTime == other.MaxUpdateTime &&
		BitmapAlignment == other.BitmapAlignment;
#if NETSTANDARD2_1 || NETCOREAPP2_1_OR_GREATER
	public readonly override int GetHashCode() => HashCode.Combine(HashCode.Combine(CachePath, ResourcePathPrefix, FaceWinding, FontHinting, FontGamma, UserStylesheet, ForceRepaint, AnimationTimerDelay), HashCode.Combine(ScrollTimerDelay, RecycleDelay, MemoryCacheSize, PageCacheSize, OverrideRAMSize, MinLargeHeapSize, MinSmallHeapSize, NumRendererThreads), HashCode.Combine(MaxUpdateTime, BitmapAlignment));
#else
		public readonly override int GetHashCode() => base.GetHashCode();

#endif
	public static bool operator ==(ULConfig left, ULConfig right) => left.Equals(right);
	public static bool operator !=(ULConfig left, ULConfig right) => !(left == right);
}
