using System;
using System.Runtime.InteropServices;
namespace UltralightNet;

public delegate string ULFontLoaderGetFallbackFont();
public unsafe delegate ULString* _ULFontLoaderGetFallbackFont();

public delegate string ULFontLoaderGetFallbackFontForCharactersCallback(string characters, int weight, bool italic);
public unsafe delegate ULString* _ULFontLoaderGetFallbackFontForCharactersCallback(ULString* characters, int weight, bool italic);

public delegate ULFontFile ULFontLoaderLoadCallback(string font, int weight, bool italic);
public unsafe delegate ULFontFile _ULFontLoaderLoadCallback(ULString* font, int weight, bool italic);

public unsafe struct ULFontLoader
{
	public ULFontLoaderGetFallbackFont? GetFallbackFont
	{
		set => _GetFallbackFont = value is not null ? () => new ULString(value().AsSpan()).Allocate() : null;
		readonly get
		{
			var c = _GetFallbackFont;
			return c is not null ? () =>
			{
				ULString* font = c();
				string fontString = font->ToString();
				font->Deallocate();
				return fontString;
			}
			: null;
		}
	}
	public _ULFontLoaderGetFallbackFont? _GetFallbackFont
	{
		set => ULPlatform.Handle(ref this, this with { __GetFallbackFont = value is not null ? (delegate* unmanaged[Cdecl]<ULString*>)Marshal.GetFunctionPointerForDelegate(value) : null}, value);
		readonly get
		{
			var p = __GetFallbackFont;
			return p is not null ? () => p() : null;
		}
	}

	public delegate* unmanaged[Cdecl]<ULString*> __GetFallbackFont;
	public delegate* unmanaged[Cdecl]<ULString*, int, bool, ULString*> __GetFallbackFontForCharacters;
	public delegate* unmanaged[Cdecl]<ULString*, int, bool, ULFontFile> __Load;
}
