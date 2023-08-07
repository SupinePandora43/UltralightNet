using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;

namespace UltralightNet;

[NativeMarshalling(typeof(Marshaller))]
public struct ULViewConfig : IEquatable<ULViewConfig>
{
	/// <summary>The View will be rendered to an offscreen GPU texture using the <see cref="ULPlatform.GPUDriver" />. You can fetch details for the texture via <see cref="View.RenderTarget" />.</summary>
	public bool IsAccelerated = false;
	/// <summary>The view's background will be (0,0,0,0).</summary>
	/// <remarks>HTML must also use transparent background.</remarks>
	public bool IsTransparent = false;

	public double InitialDeviceScale = 1.0;
	public bool InitialFocus = true;

	/// <summary>Whether or not images should be enabled.</summary>
	public bool EnableImages = true;
	/// <summary>Whether or not JavaScript should be enabled.</summary>
	public bool EnableJavaScript = true;

	/// <summary>Default font-family to use.</summary>
	public string FontFamilyStandard = "Times New Roman";
	/// <summary>Default font-family to use for fixed fonts. (pre/code)</summary>
	public string FontFamilyFixed = "Courier New";
	/// <summary>Default font-family to use for serif fonts.</summary>
	public string FontFamilySerif = "Times New Roman";
	/// <summary>Default font-family to use for sans-serif fonts.</summary>
	public string FontFamilySansSerif = "Arial";

	/// <summary>Default user-agent string.</summary>
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

	[CustomMarshaller(typeof(ULViewConfig), MarshalMode.ManagedToUnmanagedIn, typeof(Marshaller))]
	internal ref struct Marshaller
	{
		public byte IsAccelerated;
		public byte IsTransparent;

		public double InitialDeviceScale;
		public byte InitialFocus;

		public byte EnableImages;
		public byte EnableJavaScript;

		public ULString FontFamilyStandard;
		public ULString FontFamilyFixed;
		public ULString FontFamilySerif;
		public ULString FontFamilySansSerif;

		public ULString UserAgent;

		public void FromManaged(ULViewConfig config)
		{
			IsAccelerated = Unsafe.As<bool, byte>(ref config.IsAccelerated);
			IsTransparent = Unsafe.As<bool, byte>(ref config.IsTransparent);
			InitialDeviceScale = config.InitialDeviceScale;
			InitialFocus = Unsafe.As<bool, byte>(ref config.InitialFocus);
			EnableImages = Unsafe.As<bool, byte>(ref config.EnableImages);
			EnableJavaScript = Unsafe.As<bool, byte>(ref config.EnableJavaScript);
			FontFamilyStandard = new(config.FontFamilyStandard.AsSpan());
			FontFamilyFixed = new(config.FontFamilyFixed.AsSpan());
			FontFamilySerif = new(config.FontFamilySerif.AsSpan());
			FontFamilySansSerif = new(config.FontFamilySansSerif.AsSpan());
			UserAgent = new(config.UserAgent.AsSpan());
		}
		public readonly Marshaller ToUnmanaged() => this;
		public void Free()
		{
			FontFamilyStandard.Dispose();
			FontFamilyFixed.Dispose();
			FontFamilySerif.Dispose();
			FontFamilySansSerif.Dispose();
			UserAgent.Dispose();
		}
	}
}
