using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		/// <summary>Create config with default values (see <Ultralight/platform/Config.h>).</summary>
		[DllImport("Ultralight")]
		public static extern IntPtr ulCreateConfig();

		/// <summary>Destroy config.</summary>
		[DllImport("Ultralight")]
		public static extern void ulDestroyConfig(IntPtr config);

		/// <summary>Set the file path to the directory that contains Ultralight's bundled resources (eg, cacert.pem and other localized resources).</summary>
		[DllImport("Ultralight")]
		public static extern void ulConfigSetResourcePath(IntPtr config, IntPtr resource_path);

		/// <summary>Set the file path to a writable directory that will be used to store cookies, cached resources, and other persistent data.</summary>
		[DllImport("Ultralight")]
		public static extern void ulConfigSetCachePath(IntPtr config, IntPtr cache_path);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetUseGPURenderer(IntPtr config, bool use_gpu);

		/// <summary>Set the amount that the application DPI has been scaled, used for scaling device coordinates to pixels and oversampling raster shapes.</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetDeviceScale(IntPtr config, double value = 1.0);

		/// <summary>The winding order for front-facing triangles.</summary>
		/// <see cref="ULFaceWinding"/>
		/// <remarks>This is only used with custom GPUDrivers</remarks>
		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetFaceWinding(IntPtr config, ULFaceWinding winding);

		/// <summary>Set whether images should be enabled.</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetEnableImages(IntPtr config, bool enabled = true);

		/// <summary>Set whether JavaScript should be eanbled.</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetEnableJavaScript(IntPtr config, bool enabled = true);

		/// <summary>The hinting algorithm to use when rendering fonts.</summary>
		/// <see cref="ULFontHinting"/>
		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetFontHinting(IntPtr config, ULFontHinting font_hinting = ULFontHinting.Normal);

		/// <summary>The gamma to use when compositing font glyphs, change this value to adjust contrast (Adobe and Apple prefer 1.8, others may prefer 2.2).</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetFontGamma(IntPtr config, double font_gamma = 1.8);

		/// <summary>Set default font-family to use (Default = Times New Roman).</summary>
		[DllImport("Ultralight")]
		public static extern void ulConfigSetFontFamilyStandard(IntPtr config, IntPtr font_name);

		/// <summary>Set default font-family to use for fixed fonts, eg <pre> and <code> (Default = Courier New).</summary>
		[DllImport("Ultralight")]
		public static extern void ulConfigSetFontFamilyFixed(IntPtr config, IntPtr font_name);

		/// <summary>Set default font-family to use for serif fonts (Default = Times New Roman).</summary>
		[DllImport("Ultralight")]
		public static extern void ulConfigSetFontFamilySerif(IntPtr config, IntPtr font_name);

		/// <summary>Set default font-family to use for sans-serif fonts (Default = Arial).</summary>
		[DllImport("Ultralight")]
		public static extern void ulConfigSetFontFamilySansSerif(IntPtr config, IntPtr font_name);

		/// <summary>Set user agent string</summary>
		[DllImport("Ultralight")]
		public static extern void ulConfigSetUserAgent(IntPtr config, IntPtr font_name);

		/// <summary>Set user stylesheet (CSS) (Default = Empty).</summary>
		[DllImport("Ultralight")]
		public static extern void ulConfigSetUserStylesheet(IntPtr config, IntPtr font_name);

		/// <summary>Set whether or not we should continuously repaint any Views or compositor layers, regardless if they are dirty or not. This is mainly used to diagnose painting/shader issues.</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetForceRepaint(IntPtr config, bool enabled = false);

		/// <summary>Set the amount of time to wait before triggering another repaint when a CSS animation is active.</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetAnimationTimerDelay(IntPtr config, double delay = 1.0 / 60.0);

		/// <summary>When a smooth scroll animation is active, the amount of time (in seconds) to wait before triggering another repaint.</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetScrollTimerDelay(IntPtr config, double delay = 1.0 / 60.0);

		/// <summary>The amount of time (in seconds) to wait before running the recycler (will attempt to return excess memory back to the system).</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetRecycleDelay(IntPtr config, double delay = 4.0);

		/// <summary>Set the size of WebCore's memory cache for decoded images, scripts, and other assets in bytes.</summary>
		[DllImport("Ultralight")]
		public static extern void ulConfigSetMemoryCacheSize(IntPtr config, uint size = 64 * 1024 * 1024);

		/// <summary>Set the number of pages to keep in the cache.</summary>
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
		[DllImport("Ultralight")]
		public static extern void ulConfigSetOverrideRAMSize(IntPtr config, uint size);

		/// <summary>The minimum size of large VM heaps in JavaScriptCore. Set this to a lower value to make these heaps start with a smaller initial value.</summary>
		[DllImport("Ultralight")]
		public static extern void ulConfigSetMinLargeHeapSize(IntPtr config, uint size);

		/// <summary>The minimum size of small VM heaps in JavaScriptCore. Set this to a lower value to make these heaps start with a smaller initial value.</summary>
		[DllImport("Ultralight")]
		public static extern void ulConfigSetMinSmallHeapSize(IntPtr config, uint size);
	}

	/// <summary>Configuration settings for Ultralight.</summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct ULConfig_C
	{
		/// <summary>The file path to the directory that contains Ultralight's bundled resources (eg, cacert.pem and other localized resources).</summary>
		public ULString16 resource_path;
		/// <summary>The file path to a writable directory that will be used to store cookies, cached resources, and other persistent data.</summary>
		public ULString16 cache_path;

		/// <remarks>
		/// When enabled, each View will be rendered to an offscreen GPU texture<br/>
		/// using the GPU driver set in ulPlatformSetGPUDriver. You can fetch<br/>
		/// details for the texture via ulViewGetRenderTarget.<br/>
		/// <br/>
		/// When disabled (the default), each View will be rendered to an offscreen<br/>
		/// pixel buffer. This pixel buffer can optionally be provided by the user--<br/>
		/// for more info see ulViewGetSurface.
		/// </remarks>
		public bool use_gpu_renderer;
		public double device_scale;

		public ULFaceWinding face_winding;

		/// <summary>Whether or not images should be enabled.</summary>
		public bool enable_images;
		/// <summary>Whether or not JavaScript should be enabled.</summary>
		public bool enable_javascript;

		/// <summary>The hinting algorithm to use when rendering fonts.</summary>
		/// <see cref="ULFontHinting"/>
		public ULFontHinting font_hinting;

		/// <summary>The gamma to use when compositing font glyphs, change this value to adjust contrast (Adobe and Apple prefer 1.8, others may prefer 2.2).</summary>
		public double font_gamma;

		public ULString16 font_family_standard;
		public ULString16 font_family_fixed;
		public ULString16 font_family_serif;
		public ULString16 font_family_sans_serif;

		public ULString16 user_agent;
		public ULString16 user_stylesheet;

		public bool force_repaint;

		public double animation_timer_delay;
		public double scroll_timer_delay;
		public double recycle_delay;

		public uint memory_cache_size;
		public uint page_cache_size;
		public uint override_ram_size;
		public uint min_large_heap_size;
		public uint min_small_heap_size;
	}

	/// <summary>Configuration settings for Ultralight.</summary>
	public class ULConfig : IDisposable
	{
		public IntPtr Ptr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private set;
		}
		public ULConfig_C ULConfig_C
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Marshal.PtrToStructure<ULConfig_C>(Ptr);
		}

		public ULConfig(bool dispose = true)
		{
			Ptr = Methods.ulCreateConfig();
			IsDisposed = !dispose;
		}
		/// <summary>The file path to the directory that contains Ultralight's bundled resources (eg, cacert.pem and other localized resources).</summary>
		public string ResourcePath
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.resource_path.data_;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetResourcePath(Ptr, ((ULString)value).Ptr);
		}
		/// <summary>The file path to a writable directory that will be used to store cookies, cached resources, and other persistent data.</summary>
		public string CachePath
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.cache_path.data_;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetCachePath(Ptr, ((ULString)value).Ptr);
		}

		/// <remarks>
		/// When enabled, each View will be rendered to an offscreen GPU texture<br/>
		/// using the GPU driver set in ulPlatformSetGPUDriver. You can fetch<br/>
		/// details for the texture via ulViewGetRenderTarget.<br/>
		/// <br/>
		/// When disabled (the default), each View will be rendered to an offscreen<br/>
		/// pixel buffer. This pixel buffer can optionally be provided by the user--<br/>
		/// for more info see ulViewGetSurface.
		/// </remarks>
		public bool UseGpu
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.use_gpu_renderer;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetUseGPURenderer(Ptr, value);
		}

		/// <summary>The amount that the application DPI has been scaled (200% = 2.0).<br/>This should match the device scale set for the current monitor.</summary>
		/// <remarks>Device scales are rounded to nearest 1/8th (eg, 0.125).</remarks>
		public double DeviceScale
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.device_scale;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetDeviceScale(Ptr, value);
		}

		/// <summary>The winding order for front-facing triangles. <see cref="ULFaceWinding"/></summary>
		/// <remarks>This is only used when the GPU renderer is enabled.</remarks>
		public ULFaceWinding FaceWinding
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.face_winding;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetFaceWinding(Ptr, value);
		}

		/// <summary>Whether or not images should be enabled.</summary>
		public bool EnableImages
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.enable_images;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetEnableImages(Ptr, value);
		}
		/// <summary>Whether or not JavaScript should be enabled.</summary>
		public bool EnableJavaScript
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.enable_javascript;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetEnableJavaScript(Ptr, value);
		}

		/// <summary>The hinting algorithm to use when rendering fonts. <see cref="ULFontHinting"/></summary>
		public ULFontHinting FontHinting
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.font_hinting;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetFontHinting(Ptr, value);
		}

		/// <summary>The gamma to use when compositing font glyphs, change this value to adjust contrast (Adobe and Apple prefer 1.8, others may prefer 2.2).</summary>
		public double FontGamma
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.font_gamma;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetFontGamma(Ptr, value);
		}
		/// <summary>Default font-family to use.</summary>
		public string FontFamilyStandard
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.font_family_standard.data_;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetFontFamilyStandard(Ptr, ((ULString)value).Ptr);
		}
		/// <summary>Default font-family to use for fixed fonts. (pre/code)</summary>
		public string FontFamilyFixed
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.font_family_fixed.data_;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetFontFamilyFixed(Ptr, ((ULString)value).Ptr);
		}
		/// <summary>Default font-family to use for serif fonts.</summary>
		public string FontFamilySerif
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.font_family_serif.data_;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetFontFamilySerif(Ptr, ((ULString)value).Ptr);
		}
		/// <summary>Default font-family to use for sans-serif fonts.</summary>
		public string FontFamilySansSerif
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.font_family_sans_serif.data_;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetFontFamilySansSerif(Ptr, ((ULString)value).Ptr);
		}
		/// <summary>Default user-agent string.</summary>
		public string UserAgent
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.user_agent.data_;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetUserAgent(Ptr, ((ULString)value).Ptr);
		}
		/// <summary>
		/// Default user stylesheet. You should set this to your own custom CSS
		/// string to define default styles for various DOM elements, scrollbars,
		/// and platform input widgets.
		/// </summary>
		public string UserStylesheet
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.user_stylesheet.data_;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetUserStylesheet(Ptr, ((ULString)value).Ptr);
		}

		/// <summary>
		/// Whether or not we should continuously repaint any Views or compositor
		/// layers, regardless if they are dirty or not. This is mainly used to
		/// diagnose painting/shader issues.
		/// </summary>
		public bool ForceRepaint
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.force_repaint;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetForceRepaint(Ptr, value);
		}

		/// <summary>
		/// When a CSS animation is active, the amount of time (in seconds) to wait
		/// before triggering another repaint. Default is 60 Hz.
		/// </summary>
		public double AnimationTimerDelay
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.animation_timer_delay;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetAnimationTimerDelay(Ptr, value);
		}
		/// <summary>
		/// When a smooth scroll animation is active, the amount of time (in seconds)
		/// to wait before triggering another repaint. Default is 60 Hz.
		/// </summary>
		public double ScrollTimerDelay
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.scroll_timer_delay;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetScrollTimerDelay(Ptr, value);
		}
		/// <summary>
		/// The amount of time (in seconds) to wait before running the recycler (will
		/// attempt to return excess memory back to the system).
		/// </summary>
		public double RecycleDelay
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.recycle_delay;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetRecycleDelay(Ptr, value);
		}

		/// <summary>
		/// Size of WebCore's memory cache in bytes. 
		/// </summary>
		/// <remarks>
		/// You should increase this if you anticipate handling pages with
		/// large resources, Safari typically uses 128+ MiB for its cache.
		/// </remarks>
		public uint MemoryCacheSize
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.memory_cache_size;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetMemoryCacheSize(Ptr, value);
		}
		/// <summary>
		/// Number of pages to keep in the cache. Defaults to 0 (none).
		/// </summary>
		/// <remarks>
		/// Safari typically caches about 5 pages and maintains an on-disk
		/// cache to support typical web-browsing activities. If you increase
		/// this, you should probably increase the memory cache size as well.
		/// </remarks>
		public uint PageCacheSize
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.page_cache_size;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetPageCacheSize(Ptr, value);
		}
		/// <summary>
		/// JavaScriptCore tries to detect the system's physical RAM size to set
		/// reasonable allocation limits. Set this to anything other than 0 to
		/// override the detected value. Size is in bytes.
		/// </summary>
		/// <remarks>
		/// This can be used to force JavaScriptCore to be more conservative with
		/// its allocation strategy (at the cost of some performance).
		/// </remarks>
		public uint OverrideRAMSize
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.override_ram_size;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetOverrideRAMSize(Ptr, value);
		}
		/// <summary>
		/// The minimum size of large VM heaps in JavaScriptCore. Set this to a
		/// lower value to make these heaps start with a smaller initial value.
		/// </summary>
		public uint MinLargeHeapSize
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.min_large_heap_size;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetMinLargeHeapSize(Ptr, value);
		}
		/// <summary>
		/// The minimum size of small VM heaps in JavaScriptCore. Set this to a
		/// lower value to make these heaps start with a smaller initial value.
		/// </summary>
		public uint MinSmallHeapSize
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.min_small_heap_size;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetMinSmallHeapSize(Ptr, value);
		}

		public bool IsDisposed
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private set;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		~ULConfig() => Dispose();
		public void Dispose()
		{
			if (IsDisposed) return;
			Methods.ulDestroyConfig(Ptr);

			IsDisposed = true;
			GC.SuppressFinalize(this);
		}
	}
}