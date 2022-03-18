using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		/// <summary>Create config with default values (see <Ultralight/platform/Config.h>).</summary>
		[Obsolete]
		[DllImport("Ultralight")]
		public static extern IntPtr ulCreateConfig();

		/// <summary>Destroy config.</summary>
		[Obsolete]
		[DllImport("Ultralight")]
		public static extern void ulDestroyConfig(IntPtr config);

		/// <summary>Set the file path to a writable directory that will be used to store cookies, cached resources, and other persistent data.</summary>
		[Obsolete]
		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetCachePath(IntPtr config, [MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))] string cache_path);

		/// <summary>The winding order for front-facing triangles.</summary>
		/// <see cref="ULFaceWinding"/>
		/// <remarks>This is only used with custom GPUDrivers</remarks>
		[Obsolete]
		[DllImport("Ultralight")]
		public static extern void ulConfigSetFaceWinding(IntPtr config, ULFaceWinding winding);

		/// <summary>The hinting algorithm to use when rendering fonts.</summary>
		/// <see cref="ULFontHinting"/>
		[Obsolete]
		[DllImport("Ultralight")]
		public static extern void ulConfigSetFontHinting(IntPtr config, ULFontHinting font_hinting = ULFontHinting.Normal);

		/// <summary>The gamma to use when compositing font glyphs, change this value to adjust contrast (Adobe and Apple prefer 1.8, others may prefer 2.2).</summary>
		[Obsolete]
		[DllImport("Ultralight")]
		public static extern void ulConfigSetFontGamma(IntPtr config, double font_gamma = 1.8);

		/// <summary>Set user stylesheet (CSS) (Default = Empty).</summary>
		[Obsolete]
		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetUserStylesheet(IntPtr config, [MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))] string font_name);

		/// <summary>Set whether or not we should continuously repaint any Views or compositor layers, regardless if they are dirty or not. This is mainly used to diagnose painting/shader issues.</summary>
		[Obsolete]
		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetForceRepaint(IntPtr config, [MarshalAs(UnmanagedType.I1)] bool enabled = false);

		/// <summary>Set the amount of time to wait before triggering another repaint when a CSS animation is active.</summary>
		[Obsolete]
		[DllImport("Ultralight")]
		public static extern void ulConfigSetAnimationTimerDelay(IntPtr config, double delay = 1.0 / 60.0);

		/// <summary>When a smooth scroll animation is active, the amount of time (in seconds) to wait before triggering another repaint.</summary>
		[Obsolete]
		[DllImport("Ultralight")]
		public static extern void ulConfigSetScrollTimerDelay(IntPtr config, double delay = 1.0 / 60.0);

		/// <summary>The amount of time (in seconds) to wait before running the recycler (will attempt to return excess memory back to the system).</summary>
		[Obsolete]
		[DllImport("Ultralight")]
		public static extern void ulConfigSetRecycleDelay(IntPtr config, double delay = 4.0);

		/// <summary>Set the size of WebCore's memory cache for decoded images, scripts, and other assets in bytes.</summary>
		[Obsolete]
		[DllImport("Ultralight")]
		public static extern void ulConfigSetMemoryCacheSize(IntPtr config, uint size = 64 * 1024 * 1024);

		/// <summary>Set the number of pages to keep in the cache.</summary>
		[Obsolete]
		[DllImport("Ultralight")]
		public static extern void ulConfigSetPageCacheSize(IntPtr config, uint size = 0);

		/// <summary>
		/// JavaScriptCore tries to detect the system's physical RAM size to set
		/// reasonable allocation limits. Set this to anything other than 0 to
		/// override the detected value. Size is in bytes.
		/// <br/>
		/// This can be used to force JavaScriptCore to be more conservative with
		/// its allocation strategy (at the cost of some performance).
		/// </summary>
		[Obsolete]
		[DllImport("Ultralight")]
		public static extern void ulConfigSetOverrideRAMSize(IntPtr config, uint size);

		/// <summary>The minimum size of large VM heaps in JavaScriptCore. Set this to a lower value to make these heaps start with a smaller initial value.</summary>
		[Obsolete]
		[DllImport("Ultralight")]
		public static extern void ulConfigSetMinLargeHeapSize(IntPtr config, uint size);

		/// <summary>The minimum size of small VM heaps in JavaScriptCore. Set this to a lower value to make these heaps start with a smaller initial value.</summary>
		[Obsolete]
		[DllImport("Ultralight")]
		public static extern void ulConfigSetMinSmallHeapSize(IntPtr config, uint size);
	}

	/// <summary>Configuration settings for Ultralight.</summary>
	[BlittableType]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct _ULConfig
	{
		/// <summary>The file path to a writable directory that will be used to store cookies, cached resources, and other persistent data.</summary>
		public ULString CachePath;
		/// <summary>Path to use for filesystem resources.</summary>
		public ULString ResourcePathPrefix;

		private byte _FaceWinding = (byte)ULFaceWinding.CounterClockwise;
		/// <summary>Face winding for ULGPUDriver.</summary>
		public ULFaceWinding FaceWinding { get => Unsafe.As<byte, ULFaceWinding>(ref _FaceWinding); set => _FaceWinding = Unsafe.As<ULFaceWinding, byte>(ref value); }

		private byte _FontHinting = (byte)ULFontHinting.Normal;
		/// <summary>The hinting algorithm to use when rendering fonts.</summary>
		/// <see cref="ULFontHinting"/>
		public ULFontHinting FontHinting { get => Unsafe.As<byte, ULFontHinting>(ref _FontHinting); set => _FontHinting = Unsafe.As<ULFontHinting, byte>(ref value); }
		/// <summary>The gamma to use when compositing font glyphs, change this value to adjust contrast (Adobe and Apple prefer 1.8, others may prefer 2.2).</summary>
		public double FontGamma = 1.8;

		/// <summary>Default user style (CSS).</summary>
		public ULString UserStylesheet;

		private byte _ForceRepaint = 0;
		/// <summary>Whether or not we should continuously repaint any Views or compositor layers, regardless if they are dirty or not.</summary>
		/// <remarks>This is mainly used to diagnose painting/shader issues.</remarks>
		public bool ForceRepaint { get => Unsafe.As<byte, bool>(ref _ForceRepaint); set => _ForceRepaint = Unsafe.As<bool, byte>(ref value); }

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

		public _ULConfig(ULConfig config)
		{
			CachePath = ULString.CreateOpaque(config.CachePath ?? string.Empty);
			ResourcePathPrefix = ULString.CreateOpaque(config.ResourcePathPrefix ?? string.Empty);
			_FaceWinding = Unsafe.As<ULFaceWinding, byte>(ref config.FaceWinding);
			_FontHinting = Unsafe.As<ULFontHinting, byte>(ref config.FontHinting);
			FontGamma = config.FontGamma;
			UserStylesheet = ULString.CreateOpaque(config.UserStylesheet ?? string.Empty);
			_ForceRepaint = Unsafe.As<bool, byte>(ref config.ForceRepaint);
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

		public void FreeNative()
		{
			// INTEROPTODO: ZEROFREE
			ULString.FreeOpaque(CachePath);
			ULString.FreeOpaque(ResourcePathPrefix);
			ULString.FreeOpaque(UserStylesheet);
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

		public override bool Equals([NotNullWhen(true)] object? obj) => obj is ULConfig ? Equals((ULConfig)obj) : false;
		public bool Equals(ULConfig other) =>
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
		public override int GetHashCode() => HashCode.Combine(HashCode.Combine(CachePath, ResourcePathPrefix, FaceWinding, FontHinting, FontGamma, UserStylesheet, ForceRepaint, AnimationTimerDelay), HashCode.Combine(ScrollTimerDelay, RecycleDelay, MemoryCacheSize, PageCacheSize, OverrideRAMSize, MinLargeHeapSize, MinSmallHeapSize, NumRendererThreads), HashCode.Combine(MaxUpdateTime, BitmapAlignment));
		public static bool operator ==(ULConfig left, ULConfig right) => left.Equals(right);
		public static bool operator !=(ULConfig left, ULConfig right) => !(left == right);
	}
}
