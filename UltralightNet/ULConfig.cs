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
		public static extern void ulConfigSetResourcePath(IntPtr config, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string resource_path);

		/// <summary>Set the file path to a writable directory that will be used to store cookies, cached resources, and other persistent data.</summary>
		[DllImport("Ultralight")]
		public static extern void ulConfigSetCachePath(IntPtr config, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string cache_path);

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
		public static extern void ulConfigSetFontFamilyStandard(IntPtr config, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string font_name);

		/// <summary>Set default font-family to use for fixed fonts, eg <pre> and <code> (Default = Courier New).</summary>
		[DllImport("Ultralight")]
		public static extern void ulConfigSetFontFamilyFixed(IntPtr config, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string font_name);

		/// <summary>Set default font-family to use for serif fonts (Default = Times New Roman).</summary>
		[DllImport("Ultralight")]
		public static extern void ulConfigSetFontFamilySerif(IntPtr config, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string font_name);

		/// <summary>Set default font-family to use for sans-serif fonts (Default = Arial).</summary>
		[DllImport("Ultralight")]
		public static extern void ulConfigSetFontFamilySansSerif(IntPtr config, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string font_name);

		/// <summary>Set user agent string</summary>
		[DllImport("Ultralight")]
		public static extern void ulConfigSetUserAgent(IntPtr config, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string font_name);

		/// <summary>Set user stylesheet (CSS) (Default = Empty).</summary>
		[DllImport("Ultralight")]
		public static extern void ulConfigSetUserStylesheet(IntPtr config, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string font_name);

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
	[NativeMarshalling(typeof(ULConfigNative))]
	public struct ULConfig
	{
		/// <summary>The file path to the directory that contains Ultralight's bundled resources (eg, cacert.pem and other localized resources).</summary>
		public string ResourcePath;
		/// <summary>The file path to a writable directory that will be used to store cookies, cached resources, and other persistent data.</summary>
		public string CachePath;

		/// <remarks>
		/// When enabled, each View will be rendered to an offscreen GPU texture<br/>
		/// using the GPU driver set in ulPlatformSetGPUDriver. You can fetch<br/>
		/// details for the texture via ulViewGetRenderTarget.<br/>
		/// <br/>
		/// When disabled (the default), each View will be rendered to an offscreen<br/>
		/// pixel buffer. This pixel buffer can optionally be provided by the user--<br/>
		/// for more info see ulViewGetSurface.
		/// </remarks>
		public bool UseGpu;

		/// <summary>The amount that the application DPI has been scaled (200% = 2.0).<br/>This should match the device scale set for the current monitor.</summary>
		/// <remarks>Device scales are rounded to nearest 1/8th (eg, 0.125).</remarks>
		public double DeviceScale;

		/// <summary>The winding order for front-facing triangles. <see cref="ULFaceWinding"/></summary>
		/// <remarks>This is only used when the GPU renderer is enabled.</remarks>
		public ULFaceWinding FaceWinding;

		/// <summary>Whether or not images should be enabled.</summary>
		public bool EnableImages;
		/// <summary>Whether or not JavaScript should be enabled.</summary>
		public bool EnableJavaScript;

		/// <summary>The hinting algorithm to use when rendering fonts. <see cref="ULFontHinting"/></summary>
		public ULFontHinting FontHinting;

		/// <summary>The gamma to use when compositing font glyphs, change this value to adjust contrast (Adobe and Apple prefer 1.8, others may prefer 2.2).</summary>
		public double FontGamma;
		/// <summary>Default font-family to use.</summary>
		public string FontFamilyStandard;
		/// <summary>Default font-family to use for fixed fonts. (pre/code)</summary>
		public string FontFamilyFixed;
		/// <summary>Default font-family to use for serif fonts.</summary>
		public string FontFamilySerif;
		/// <summary>Default font-family to use for sans-serif fonts.</summary>
		public string FontFamilySansSerif;
		/// <summary>Default user-agent string.</summary>
		public string UserAgent;
		/// <summary>
		/// Default user stylesheet. You should set this to your own custom CSS
		/// string to define default styles for various DOM elements, scrollbars,
		/// and platform input widgets.
		/// </summary>
		public string UserStylesheet;

		/// <summary>
		/// Whether or not we should continuously repaint any Views or compositor
		/// layers, regardless if they are dirty or not. This is mainly used to
		/// diagnose painting/shader issues.
		/// </summary>
		public bool ForceRepaint;

		/// <summary>
		/// When a CSS animation is active, the amount of time (in seconds) to wait
		/// before triggering another repaint. Default is 60 Hz.
		/// </summary>
		public double AnimationTimerDelay;
		/// <summary>
		/// When a smooth scroll animation is active, the amount of time (in seconds)
		/// to wait before triggering another repaint. Default is 60 Hz.
		/// </summary>
		public double ScrollTimerDelay;
		/// <summary>
		/// The amount of time (in seconds) to wait before running the recycler (will
		/// attempt to return excess memory back to the system).
		/// </summary>
		public double RecycleDelay;

		/// <summary>
		/// Size of WebCore's memory cache in bytes. 
		/// </summary>
		/// <remarks>
		/// You should increase this if you anticipate handling pages with
		/// large resources, Safari typically uses 128+ MiB for its cache.
		/// </remarks>
		public uint MemoryCacheSize;
		/// <summary>
		/// Number of pages to keep in the cache. Defaults to 0 (none).
		/// </summary>
		/// <remarks>
		/// Safari typically caches about 5 pages and maintains an on-disk
		/// cache to support typical web-browsing activities. If you increase
		/// this, you should probably increase the memory cache size as well.
		/// </remarks>
		public uint PageCacheSize;
		/// <summary>
		/// JavaScriptCore tries to detect the system's physical RAM size to set
		/// reasonable allocation limits. Set this to anything other than 0 to
		/// override the detected value. Size is in bytes.
		/// </summary>
		/// <remarks>
		/// This can be used to force JavaScriptCore to be more conservative with
		/// its allocation strategy (at the cost of some performance).
		/// </remarks>
		public uint OverrideRAMSize;
		/// <summary>
		/// The minimum size of large VM heaps in JavaScriptCore. Set this to a
		/// lower value to make these heaps start with a smaller initial value.
		/// </summary>
		public uint MinLargeHeapSize;
		/// <summary>
		/// The minimum size of small VM heaps in JavaScriptCore. Set this to a
		/// lower value to make these heaps start with a smaller initial value.
		/// </summary>
		public uint MinSmallHeapSize;
	}

	/// <summary>
	/// FOR <b>INTERNAL</b> USE<br/>but you can use this too<br/>pls don't
	/// </summary>
	[BlittableType]
	public struct ULConfigNative
	{
		public ULStringMarshaler.ULStringPTR ResourcePath;
		public ULStringMarshaler.ULStringPTR CachePath;

		public byte UseGpu;

		public double DeviceScale;

		public int FaceWinding;

		public byte EnableImages;
		public byte EnableJavaScript;

		public double FontGamma;

		public ULStringMarshaler.ULStringPTR FontFamilyStandard;
		public ULStringMarshaler.ULStringPTR FontFamilyFixed;
		public ULStringMarshaler.ULStringPTR FontFamilySerif;
		public ULStringMarshaler.ULStringPTR FontFamilySansSerif;
		public ULStringMarshaler.ULStringPTR UserAgent;
		public ULStringMarshaler.ULStringPTR UserStylesheet;

		public byte ForceRepaint;

		public double AnimationTimerDelay;
		public double ScrollTimerDelay;
		public double RecycleDelay;

		public uint MemoryCacheSize;
		public uint PageCacheSize;
		public uint OverrideRAMSize;
		public uint MinLargeHeapSize;
		public uint MinSmallHeapSize;

		public ULConfigNative(ULConfig config)
		{
			ResourcePath = ULStringMarshaler.ULStringPTR.ManagedToNative(config.ResourcePath);
			CachePath = ULStringMarshaler.ULStringPTR.ManagedToNative(config.CachePath);
			UseGpu = (byte)(config.UseGpu ? 1 : 0);
			DeviceScale = config.DeviceScale;
			FaceWinding = (int)config.FaceWinding;
			EnableImages = (byte)(config.EnableImages ? 1 : 0);
			EnableJavaScript = (byte)(config.EnableJavaScript ? 1 : 0);
			FontGamma = config.FontGamma;
			FontFamilyStandard = ULStringMarshaler.ULStringPTR.ManagedToNative(config.FontFamilyStandard);
			FontFamilyFixed = ULStringMarshaler.ULStringPTR.ManagedToNative(config.FontFamilyFixed);
			FontFamilySerif = ULStringMarshaler.ULStringPTR.ManagedToNative(config.FontFamilySerif);
			FontFamilySansSerif = ULStringMarshaler.ULStringPTR.ManagedToNative(config.FontFamilySansSerif);
			UserAgent = ULStringMarshaler.ULStringPTR.ManagedToNative(config.UserAgent);
			UserStylesheet = ULStringMarshaler.ULStringPTR.ManagedToNative(config.UserStylesheet);
			ForceRepaint = (byte)(config.ForceRepaint ? 1 : 0);
			AnimationTimerDelay = config.AnimationTimerDelay;
			ScrollTimerDelay = config.ScrollTimerDelay;
			RecycleDelay = config.RecycleDelay;
			MemoryCacheSize = config.MemoryCacheSize;
			PageCacheSize = config.PageCacheSize;
			OverrideRAMSize = config.OverrideRAMSize;
			MinLargeHeapSize = config.MinLargeHeapSize;
			MinSmallHeapSize = config.MinSmallHeapSize;
		}
		public void FreeNative()
		{
			ULStringMarshaler.ULStringPTR.CleanUpNative(ResourcePath);
			ULStringMarshaler.ULStringPTR.CleanUpNative(CachePath);
			ULStringMarshaler.ULStringPTR.CleanUpNative(FontFamilyStandard);
			ULStringMarshaler.ULStringPTR.CleanUpNative(FontFamilyFixed);
			ULStringMarshaler.ULStringPTR.CleanUpNative(FontFamilySerif);
			ULStringMarshaler.ULStringPTR.CleanUpNative(FontFamilySansSerif);
			ULStringMarshaler.ULStringPTR.CleanUpNative(UserAgent);
			ULStringMarshaler.ULStringPTR.CleanUpNative(UserStylesheet);
		}

		public ULConfig ToManaged() => new()
		{
			ResourcePath = ResourcePath.ToManged(),
			CachePath = CachePath.ToManged(),
			UseGpu = UseGpu != 0,
			DeviceScale = DeviceScale,
			FaceWinding = (ULFaceWinding)FaceWinding,
			EnableImages = EnableImages != 0,
			EnableJavaScript = EnableJavaScript != 0,
			FontGamma = FontGamma,
			FontFamilyStandard = FontFamilyStandard.ToManged(),
			FontFamilyFixed = FontFamilyFixed.ToManged(),
			FontFamilySerif = FontFamilySerif.ToManged(),
			FontFamilySansSerif = FontFamilySansSerif.ToManged(),
			UserAgent = UserAgent.ToManged(),
			UserStylesheet = UserStylesheet.ToManged(),
			ForceRepaint = ForceRepaint != 0,
			AnimationTimerDelay = AnimationTimerDelay,
			ScrollTimerDelay = ScrollTimerDelay,
			RecycleDelay = RecycleDelay,
			MemoryCacheSize = MemoryCacheSize,
			PageCacheSize = PageCacheSize,
			OverrideRAMSize = OverrideRAMSize,
			MinLargeHeapSize = MinLargeHeapSize,
			MinSmallHeapSize = MinSmallHeapSize,
		};
	}
}
