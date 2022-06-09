namespace UltralightNet;

public unsafe struct ULFontLoader {
    public delegate* unmanaged[Cdecl]<ULString*> __GetFallbackFont;
	public delegate* unmanaged[Cdecl]<ULString*, int, bool, ULString*> __GetFallbackFontForCharacters;
	public delegate* unmanaged[Cdecl]<ULString*, int, bool, ULFontFile> __Load;
}
