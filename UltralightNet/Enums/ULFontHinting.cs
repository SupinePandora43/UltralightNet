namespace UltralightNet
{
	public enum ULFontHinting
	{
		/// <summary>Lighter hinting algorithm-- glyphs are slightly fuzzier but better resemble their original shape. This is achieved by snapping glyphs to the pixel grid only vertically which better preserves inter-glyph spacing.</summary>
		kFontHinting_Smooth,

		/// <summary>Default hinting algorithm-- offers a good balance between sharpness and shape at smaller font sizes.</summary>
		kFontHinting_Normal,

		/// <summary>Strongest hinting algorithm-- outputs only black/white glyphs. The result is usually unpleasant if the underlying TTF does not contain hints for this type of rendering.</summary>
		kFontHinting_Monochrome,
	}
}
