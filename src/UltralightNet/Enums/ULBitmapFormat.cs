namespace UltralightNet;

/// <summary>The various Bitmap formats.</summary>
public enum ULBitmapFormat : int // CAPI_Defines.h - no type
{
	/// <summary>
	/// Alpha channel only, 8-bits per pixel.
	/// <br/>
	/// Encoding: 8-bits per channel, unsigned normalized.
	/// <br/>
	/// Color-space: Linear (no gamma), alpha-coverage only.
	/// </summary>
	A8_UNORM,
	/// <summary>
	/// Blue Green Red Alpha channels, 32-bits per pixel.
	/// <br/>
	/// Encoding: 8-bits per channel, unsigned normalized.
	/// <br/>
	/// Color-space: sRGB gamma with premultiplied linear alpha channel.
	/// </summary>
	/// <remarks>Alpha is premultiplied with BGR channels _before_ sRGB gamma is applied so we can use sRGB conversion hardware and perform all blending in linear space on GPU.</remarks>
	BGRA8_UNORM_SRGB
}
