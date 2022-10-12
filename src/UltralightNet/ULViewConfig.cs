using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet;

public static unsafe partial class Methods
{
	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern _ULViewConfig* ulCreateViewConfig();

	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern void ulDestroyViewConfig(_ULViewConfig* viewConfig);

	[Obsolete]
	[GeneratedDllImport(LibUltralight)]
	public static partial void ulViewConfigSetIsAccelerated(_ULViewConfig* viewConfig, [MarshalAs(UnmanagedType.I1)] bool isAccelerated = false);

	[Obsolete]
	[GeneratedDllImport(LibUltralight)]
	public static partial void ulViewConfigSetIsTransparent(_ULViewConfig* viewConfig, [MarshalAs(UnmanagedType.I1)] bool isTransparent = false);

	[Obsolete]
	[DllImport(LibUltralight)]
	public static extern void ulViewConfigSetInitialDeviceScale(_ULViewConfig* viewConfig, double initialDeviceScale = 1.0);

	[Obsolete]
	[GeneratedDllImport(LibUltralight)]
	public static partial void ulViewConfigSetInitialFocus(_ULViewConfig* viewConfig, [MarshalAs(UnmanagedType.I1)] bool initialFocus = true);

	[Obsolete]
	[GeneratedDllImport(LibUltralight)]
	public static partial void ulViewConfigSetEnableImages(_ULViewConfig* viewConfig, [MarshalAs(UnmanagedType.I1)] bool enable = true);

	[Obsolete]
	[GeneratedDllImport(LibUltralight)]
	public static partial void ulViewConfigSetEnableJavaScript(_ULViewConfig* viewConfig, [MarshalAs(UnmanagedType.I1)] bool enable = true);

	[Obsolete]
	[GeneratedDllImport(LibUltralight)]
	public static partial void ulViewConfigSetFontFamilyStandard(_ULViewConfig* viewConfig, [MarshalUsing(typeof(ULString.ToNative))] string fontName = "Times New Roman");

	[Obsolete]
	[GeneratedDllImport(LibUltralight)]
	public static partial void ulViewConfigSetFontFamilyFixed(_ULViewConfig* viewConfig, [MarshalUsing(typeof(ULString.ToNative))] string fontName = "Courier New");

	[Obsolete]
	[GeneratedDllImport(LibUltralight)]
	public static partial void ulViewConfigSetFontFamilySerif(_ULViewConfig* viewConfig, [MarshalUsing(typeof(ULString.ToNative))] string fontName = "Times New Roman");

	[Obsolete]
	[GeneratedDllImport(LibUltralight)]
	public static partial void ulViewConfigSetFontFamilySansSerif(_ULViewConfig* viewConfig, [MarshalUsing(typeof(ULString.ToNative))] string fontName = "Arial");

	[Obsolete]
	[GeneratedDllImport(LibUltralight)]
	public static partial void ulViewConfigSetUserAgent(_ULViewConfig* viewConfig, [MarshalUsing(typeof(ULString.ToNative))] string agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/608.3.10 (KHTML, like Gecko) Ultralight/1.3.0 Safari/608.3.10");
}

public struct _ULViewConfig : IDisposable
{
	private byte _IsAccelerated = 0;
	/// <summary>The View will be rendered to an offscreen GPU texture using the <see cref="ULPlatform.GPUDriver" />. You can fetch details for the texture via <see cref="View.RenderTarget" />.</summary>
	public bool IsAccelerated { get => Unsafe.As<byte, bool>(ref _IsAccelerated); set => _IsAccelerated = Unsafe.As<bool, byte>(ref value); }
	private byte _IsTransparent = 0;
	/// <summary>The view's background will be (0,0,0,0).</summary>
	/// <remarks>HTML must also use transparent background.</remarks>
	public bool IsTransparent { get => Unsafe.As<byte, bool>(ref _IsTransparent); set => _IsTransparent = Unsafe.As<bool, byte>(ref value); }

	public double InitialDeviceScale = 1.0;
	private byte _InitialFocus = 1;
	public bool InitialFocus { get => Unsafe.As<byte, bool>(ref _InitialFocus); set => _InitialFocus = Unsafe.As<bool, byte>(ref value); }

	private byte _EnableImages = 1;
	/// <summary>Whether or not images should be enabled.</summary>
	public bool EnableImages { get => Unsafe.As<byte, bool>(ref _EnableImages); set => _EnableImages = Unsafe.As<bool, byte>(ref value); }
	private byte _EnableJavaScript = 1;
	/// <summary>Whether or not JavaScript should be enabled.</summary>
	public bool EnableJavaScript { get => Unsafe.As<byte, bool>(ref _EnableJavaScript); set => _EnableJavaScript = Unsafe.As<bool, byte>(ref value); }

	/// <summary>Default font-family to use.</summary>
	public ULString FontFamilyStandard;
	/// <summary>Default font-family to use for fixed fonts. (pre/code)</summary>
	public ULString FontFamilyFixed;
	/// <summary>Default font-family to use for serif fonts.</summary>
	public ULString FontFamilySerif;
	/// <summary>Default font-family to use for sans-serif fonts.</summary>
	public ULString FontFamilySansSerif;

	/// <summary>Default user-agent string.</summary>
	public ULString UserAgent;

	public _ULViewConfig(ULString fontFamilyStandard, ULString fontFamilyFixed, ULString fontFamilySerif, ULString fontFamilySansSerif, ULString userAgent)
	{
		FontFamilyStandard = fontFamilyStandard;
		FontFamilyFixed = fontFamilyFixed;
		FontFamilySerif = fontFamilySerif;
		FontFamilySansSerif = fontFamilySansSerif;
		UserAgent = userAgent;
	}

	public _ULViewConfig(ULViewConfig config)
	{
#if NET5_0_OR_GREATER
		Unsafe.SkipInit(out this);
#endif
		_IsAccelerated = Unsafe.As<bool, byte>(ref config.IsAccelerated);
		_IsTransparent = Unsafe.As<bool, byte>(ref config.IsTransparent);
		InitialDeviceScale = config.InitialDeviceScale;
		_InitialFocus = Unsafe.As<bool, byte>(ref config.InitialFocus);
		_EnableImages = Unsafe.As<bool, byte>(ref config.EnableImages);
		_EnableJavaScript = Unsafe.As<bool, byte>(ref config.EnableJavaScript);
		FontFamilyStandard = new(config.FontFamilyStandard.AsSpan());
		FontFamilyFixed = new(config.FontFamilyFixed.AsSpan());
		FontFamilySerif = new(config.FontFamilySerif.AsSpan());
		FontFamilySansSerif = new(config.FontFamilySansSerif.AsSpan());
		UserAgent = new(config.UserAgent.AsSpan());
	}
	public void Dispose()
	{
		FontFamilyStandard.Dispose();
		FontFamilyFixed.Dispose();
		FontFamilySerif.Dispose();
		FontFamilySansSerif.Dispose();
		UserAgent.Dispose();
	}
}
/// <inheritdoc cref="_ULViewConfig" />
[NativeMarshalling(typeof(_ULViewConfig))]
public struct ULViewConfig : IEquatable<ULViewConfig>
{
	/// <inheritdoc cref="_ULViewConfig.IsAccelerated" />
	public bool IsAccelerated = false;
	/// <inheritdoc cref="_ULViewConfig.IsTransparent" />
	public bool IsTransparent = false;

	/// <inheritdoc cref="_ULViewConfig.InitialDeviceScale" />
	public double InitialDeviceScale = 1.0;
	/// <inheritdoc cref="_ULViewConfig.InitialFocus" />
	public bool InitialFocus = true;

	/// <inheritdoc cref="_ULViewConfig.EnableImages" />
	public bool EnableImages = true;
	/// <inheritdoc cref="_ULViewConfig.EnableJavaScript" />
	public bool EnableJavaScript = true;

	/// <inheritdoc cref="_ULViewConfig.FontFamilyStandard" />
	public string FontFamilyStandard = "Times New Roman";
	/// <inheritdoc cref="_ULViewConfig.FontFamilyFixed" />
	public string FontFamilyFixed = "Courier New";
	/// <inheritdoc cref="_ULViewConfig.FontFamilySerif" />
	public string FontFamilySerif = "Times New Roman";
	/// <inheritdoc cref="_ULViewConfig.FontFamilySansSerif" />
	public string FontFamilySansSerif = "Arial";

	/// <inheritdoc cref="_ULViewConfig.UserAgent" />
	public string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/608.3.10 (KHTML, like Gecko) Ultralight/1.3.0 Safari/608.3.10";

	public ULViewConfig() { }

	public readonly bool Equals(ULViewConfig other) =>
		IsAccelerated == other.IsAccelerated &&
		IsTransparent == other.IsTransparent &&
		InitialDeviceScale == other.InitialDeviceScale &&
		InitialFocus == other.InitialFocus &&
		EnableImages == other.EnableImages &&
		EnableJavaScript == other.EnableJavaScript &&
		FontFamilyStandard == other.FontFamilyStandard &&
		FontFamilyFixed == other.FontFamilyFixed &&
		FontFamilySerif == other.FontFamilySerif &&
		FontFamilySansSerif == other.FontFamilySansSerif &&
		UserAgent == other.UserAgent;

	public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is ULViewConfig ? Equals((ULViewConfig)obj) : false;
#if NETSTANDARD2_1 || NETCOREAPP2_1_OR_GREATER
	public readonly override int GetHashCode() => HashCode.Combine(HashCode.Combine(IsAccelerated, IsTransparent, InitialDeviceScale, InitialFocus, EnableImages, EnableJavaScript, FontFamilyStandard, FontFamilyFixed), HashCode.Combine(FontFamilySerif, FontFamilySansSerif, UserAgent));
#else
		public readonly override int GetHashCode() => base.GetHashCode();
#endif
}
