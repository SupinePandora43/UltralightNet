using System;
using System.Runtime.InteropServices;
namespace UltralightNet;

public delegate string ULFontLoaderGetFallbackFont();
public unsafe delegate ULString* _ULFontLoaderGetFallbackFont();

public delegate string ULFontLoaderGetFallbackFontForCharactersCallback(string characters, int weight, bool italic);
public unsafe delegate ULString* _ULFontLoaderGetFallbackFontForCharactersCallback(ULString* characters, int weight, bool italic);

public delegate ULFontFile ULFontLoaderLoadCallback(string font, int weight, bool italic);
public unsafe delegate ULFontFile _ULFontLoaderLoadCallback(ULString* font, int weight, bool italic);

public unsafe struct ULFontLoader : IDisposable, IEquatable<ULFontLoader>
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
		set => ULPlatform.Handle(ref this, this with { __GetFallbackFont = value is not null ? (delegate* unmanaged[Cdecl]<ULString*>)Marshal.GetFunctionPointerForDelegate(value) : null }, value);
		readonly get
		{
			var p = __GetFallbackFont;
			return p is not null ? () => p() : null;
		}
	}

	public ULFontLoaderGetFallbackFontForCharactersCallback? GetFallbackFontForCharacters
	{
		set => _GetFallbackFontForCharacters = value is not null ? (charsUL, weight, italic) => new ULString(value(charsUL->ToString(), weight, italic).AsSpan()).Allocate() : null;
		readonly get
		{
			var c = _GetFallbackFontForCharacters;
			return c is not null ? (characters, weight, italic) =>
			{
				using ULString charactersUL = new(characters.AsSpan());
				ULString* resultUL = c(&charactersUL, weight, italic);
				string result = resultUL->ToString();
				resultUL->Deallocate();
				return result;
			}
			: null;
		}
	}
	public _ULFontLoaderGetFallbackFontForCharactersCallback? _GetFallbackFontForCharacters
	{
		set => ULPlatform.Handle(ref this, this with { __GetFallbackFontForCharacters = value is not null ? (delegate* unmanaged[Cdecl]<ULString*, int, bool, ULString*>)Marshal.GetFunctionPointerForDelegate(value) : null }, value);
		readonly get
		{
			var p = __GetFallbackFontForCharacters;
			return p is not null ? (charactersUL, weight, italic) => p(charactersUL, weight, italic) : null;
		}
	}

	public ULFontLoaderLoadCallback? Load
	{
		set => _Load = value is not null ? (fontNameUL, weight, italic) => value(fontNameUL->ToString(), weight, italic) : null;
		readonly get
		{
			var c = _Load;
			return c is not null ? (fontName, weight, italic) =>
			{
				using ULString fontNameUL = new(fontName.AsSpan());
				return c(&fontNameUL, weight, italic);
			}
			: null;
		}
	}
	public _ULFontLoaderLoadCallback? _Load
	{
		set => ULPlatform.Handle(ref this, this with { __Load = value is not null ? (delegate* unmanaged[Cdecl]<ULString*, int, bool, ULFontFile>)Marshal.GetFunctionPointerForDelegate(value) : null }, value);
		readonly get
		{
			var p = __Load;
			return p is not null ? (fontNameUL, weight, italic) => p(fontNameUL, weight, italic) : null;
		}
	}

	public delegate* unmanaged[Cdecl]<ULString*> __GetFallbackFont;
	public delegate* unmanaged[Cdecl]<ULString*, int, bool, ULString*> __GetFallbackFontForCharacters;
	public delegate* unmanaged[Cdecl]<ULString*, int, bool, ULFontFile> __Load;

	public void Dispose() => throw new NotImplementedException();

#pragma warning disable CS8909
	public readonly bool Equals(ULFontLoader other) => __GetFallbackFont == other.__GetFallbackFont && __GetFallbackFontForCharacters == other.__GetFallbackFontForCharacters && __Load == other.__Load;
#pragma warning restore CS8909
	public override int GetHashCode() =>
#if NETSTANDARD2_1 || NETCOREAPP2_1_OR_GREATER
		HashCode.Combine((nuint)__GetFallbackFont, (nuint)__GetFallbackFontForCharacters, (nuint)__Load);
#else
		base.GetHashCode();
#endif
}
