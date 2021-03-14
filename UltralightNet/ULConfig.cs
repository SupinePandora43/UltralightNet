using System;
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

		/// <summary>Set the amount that the application DPI has been scaled, used for scaling device coordinates to pixels and oversampling raster shapes (Default = 1.0).</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetDeviceScale(IntPtr config, double value = 1.0);
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ULConfig_C
	{
		public ULString16 resource_path;
		public ULString16 cache_path;

		public bool use_gpu_renderer;
		public double device_scale;
	}
	public class ULConfig
	{
		internal IntPtr Ptr { get; private set; }
		public ULConfig_C ULConfig_C => Marshal.PtrToStructure<ULConfig_C>(Ptr);

		public ULConfig() => Ptr = Methods.ulCreateConfig();

		public string ResourcePath { get => ULConfig_C.resource_path.data_; set => Methods.ulConfigSetResourcePath(Ptr, ((ULString)value).Ptr); }
		public string CachePath { get => ULConfig_C.cache_path.data_; set => Methods.ulConfigSetCachePath(Ptr, ((ULString)value).Ptr); }
		/// <remarks>
		/// When enabled, each View will be rendered to an offscreen GPU texture<br/>
		/// using the GPU driver set in ulPlatformSetGPUDriver. You can fetch<br/>
		/// details for the texture via ulViewGetRenderTarget.<br/>
		/// <br/>
		/// When disabled (the default), each View will be rendered to an offscreen<br/>
		/// pixel buffer. This pixel buffer can optionally be provided by the user--<br/>
		/// for more info see ulViewGetSurface.
		/// </remarks>
		public bool UseGpu { get => ULConfig_C.use_gpu_renderer; set => Methods.ulConfigSetUseGPURenderer(Ptr, value); }
		public double DeviceScale { get => ULConfig_C.device_scale; set => Methods.ulConfigSetDeviceScale(Ptr, value); }
	}
}
