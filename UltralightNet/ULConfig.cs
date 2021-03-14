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

		/// <summary>Set the amount that the application DPI has been scaled, used for scaling device coordinates to pixels and oversampling raster shapes (Default = 1.0).</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetDeviceScale(IntPtr config, double value = 1.0);

		/// <summary>The winding order for front-facing triangles.</summary>
		/// <see cref="ULFaceWinding"/>
		/// <remarks>This is only used with custom GPUDrivers</remarks>
		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetFaceWinding(IntPtr config, ULFaceWinding winding);

		/// <summary>Set whether images should be enabled (Default = True).</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetEnableImages(IntPtr config, bool enabled);

		/// <summary>Set whether JavaScript should be eanbled (Default = True).</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial void ulConfigSetEnableJavaScript(IntPtr config, bool enabled);
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
	}

	/// <summary>Configuration settings for Ultralight.</summary>
	public class ULConfig : IDisposable
	{
		public IntPtr Ptr { get; private set; }
		public ULConfig_C ULConfig_C
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Marshal.PtrToStructure<ULConfig_C>(Ptr);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ULConfig() => Ptr = Methods.ulCreateConfig();

		/// <summary>The file path to the directory that contains Ultralight's bundled resources (eg, cacert.pem and other localized resources).</summary>
		public string ResourcePath
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.resource_path.data_;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetResourcePath(Ptr, ((ULString)value).Ptr);
		}
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
		public double DeviceScale
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.device_scale;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetDeviceScale(Ptr, value);
		}

		public ULFaceWinding FaceWinding
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.face_winding;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetFaceWinding(Ptr, value);
		}

		public bool EnableImages
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.enable_images;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetEnableImages(Ptr, value);
		}
		public bool EnableJavaScript
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ULConfig_C.enable_javascript;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => Methods.ulConfigSetEnableJavaScript(Ptr, value);
		}

		public bool IsDisposed { get; private set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		~ULConfig() => Dispose();

		public void Dispose()
		{
			if (IsDisposed) return;
			//todo

			IsDisposed = true;
			GC.SuppressFinalize(this);
		}
	}
}
