using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		[DllImport("Ultralight")]
		public static extern IntPtr ulCreateViewConfig();

		[DllImport("Ultralight")]
		public static extern void ulDestroyViewConfig(IntPtr config);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulViewConfigSetIsAccelerated(IntPtr config, [MarshalAs(UnmanagedType.I1)] bool is_accelerated);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulViewConfigSetIsTransparent(IntPtr config, [MarshalAs(UnmanagedType.I1)] bool is_transparent);

		[DllImport("Ultralight")]
		public static extern void ulViewConfigSetInitialDeviceScale(IntPtr config, double initial_device_scale);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulViewConfigSetInitialFocus(IntPtr config, [MarshalAs(UnmanagedType.I1)] bool is_focused);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulViewConfigSetEnableImages(IntPtr config, [MarshalAs(UnmanagedType.I1)] bool enabled);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulViewConfigSetEnableJavaScript(IntPtr config, [MarshalAs(UnmanagedType.I1)] bool enabled);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulViewConfigSetFontFamilyStandard(IntPtr config, [MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))] string font_name);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulViewConfigSetFontFamilyFixed(IntPtr config, [MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))] string font_name);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulViewConfigSetFontFamilySerif(IntPtr config, [MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))] string font_name);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulViewConfigSetFontFamilySansSerif(IntPtr config, [MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))] string font_name);

		[GeneratedDllImport("Ultralight")]
		public static partial void ulViewConfigSetUserAgent(IntPtr config, [MarshalUsing(typeof(ULStringGeneratedDllImportMarshaler))] string agent_string);

	}

	public class ULViewConfig : IDisposable
	{
		public readonly IntPtr Ptr;
		public bool IsDisposed { get; private set; }

		public ULViewConfig()
		{
			Ptr = Methods.ulCreateViewConfig();
		}

		~ULViewConfig() => Dispose();

		public void Dispose()
		{
			if (IsDisposed) return;
			Methods.ulDestroyViewConfig(Ptr);

			GC.SuppressFinalize(this);
			IsDisposed = true;
		}

		public struct STRUCT
		{
			[MarshalAs(UnmanagedType.I1)]
			public bool is_accelerated;
			[MarshalAs(UnmanagedType.I1)]
			public bool is_transparent;
			public double initial_device_scale;
			[MarshalAs(UnmanagedType.I1)]
			public bool initial_focus;
			[MarshalAs(UnmanagedType.I1)]
			public bool enable_images;
			[MarshalAs(UnmanagedType.I1)]
			public bool enable_javascript;

			public ULStringMarshaler.ULStringPTR font_family_standard;
			public ULStringMarshaler.ULStringPTR font_family_fixed;
			public ULStringMarshaler.ULStringPTR font_family_serif;
			public ULStringMarshaler.ULStringPTR font_family_sans_serif;
			public ULStringMarshaler.ULStringPTR user_agent;
		}

		public STRUCT ULViewConfig_C
		{
#if NET5_0_OR_GREATER || NET451 || NETSTANDARD2_0_OR_GREATER
			get => Marshal.PtrToStructure<STRUCT>(Ptr);
#else
			get => (STRUCT)Marshal.PtrToStructure(Ptr, typeof(STRUCT));
#endif
		}

		public bool IsAccelerated
		{
			get => ULViewConfig_C.is_accelerated;
			set => Methods.ulViewConfigSetIsAccelerated(Ptr, value);
		}
		public bool IsTransparent
		{
			get => ULViewConfig_C.is_transparent;
			set => Methods.ulViewConfigSetIsTransparent(Ptr, value);
		}

		public double InitialDeviceScale
		{
			get => ULViewConfig_C.initial_device_scale;
			set => Methods.ulViewConfigSetInitialDeviceScale(Ptr, value);
		}

		public bool InitialFocus
		{
			get => ULViewConfig_C.initial_focus;
			set => Methods.ulViewConfigSetInitialFocus(Ptr, value);
		}
		public bool EnableImages
		{
			get => ULViewConfig_C.enable_images;
			set => Methods.ulViewConfigSetEnableImages(Ptr, value);
		}
		public bool EnableJavaScript
		{
			get => ULViewConfig_C.enable_javascript;
			set => Methods.ulViewConfigSetEnableJavaScript(Ptr, value);
		}

		public string FontFamilyStandard
		{
			get => ULViewConfig_C.font_family_standard.ToManaged();
			set => Methods.ulViewConfigSetFontFamilyStandard(Ptr, value);
		}
		public string FontFamilyFixed
		{
			get => ULViewConfig_C.font_family_fixed.ToManaged();
			set => Methods.ulViewConfigSetFontFamilyFixed(Ptr, value);
		}
		public string FontFamilySerif
		{
			get => ULViewConfig_C.font_family_serif.ToManaged();
			set => Methods.ulViewConfigSetFontFamilySerif(Ptr, value);
		}
		public string FontFamilySansSerif
		{
			get => ULViewConfig_C.font_family_sans_serif.ToManaged();
			set => Methods.ulViewConfigSetFontFamilySansSerif(Ptr, value);
		}
		public string UserAgent
		{
			get => ULViewConfig_C.user_agent.ToManaged();
			set => Methods.ulViewConfigSetUserAgent(Ptr, value);
		}
	}
}
