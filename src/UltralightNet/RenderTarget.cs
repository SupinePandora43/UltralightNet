using System;
using System.Runtime.CompilerServices;
#if NETCOREAPP3_0_OR_GREATER
using System.Runtime.Intrinsics;
#endif

namespace UltralightNet;

/// <summary>Rendering details for a View, to be used with your own GPUDriver</summary>
public struct RenderTarget : IEquatable<RenderTarget>
{
	private byte _IsEmpty;
	/// <summary>Whether this target is empty (null texture)</summary>
	public bool IsEmpty { readonly get => Unsafe.As<byte, bool>(ref Unsafe.AsRef(_IsEmpty)); set => _IsEmpty = Unsafe.As<bool, byte>(ref value); }

	/// <summary>The viewport width (in device coordinates).</summary>
	public uint Width;

	/// <summary>The viewport height (in device coordinates).</summary>
	public uint Height;

	/// <summary><see cref="ULGPUDriver" />'s texture id</summary>
	public uint TextureId;

	/// <summary>The texture width (in pixels). This may be padded.</summary>
	public uint TextureWidth;

	/// <summary>The texture height (in pixels). This may be padded.</summary>
	public uint TextureHeight;

	private byte _TextureFormat;
	/// <summary>The pixel format of the texture.</summary>
	public ULBitmapFormat TextureFormat { readonly get => Unsafe.As<byte, ULBitmapFormat>(ref Unsafe.AsRef(_TextureFormat)); set => _TextureFormat = Unsafe.As<ULBitmapFormat, byte>(ref value); }

	/// <summary>UV coordinates of the texture (this is needed because the texture may be padded).</summary>
	public ULRect UV;

	/// <summary><see cref="ULGPUDriver" />'s render buffer id</summary>
	public uint RenderBufferId;

	public readonly bool Equals(RenderTarget rt) =>
		IsEmpty == rt.IsEmpty &&
		TextureFormat == rt.TextureFormat &&
#if NETCOREAPP3_0_OR_GREATER
		Vector64.Create(TextureId, RenderBufferId).Equals(Vector64.Create(rt.TextureId, rt.RenderBufferId)) &&
		Vector128.Create(Width, Height, TextureWidth, TextureHeight).Equals(Vector128.Create(rt.Width, rt.Height, rt.TextureWidth, rt.TextureHeight)) &&
#else
		TextureId == rt.TextureId &&
		RenderBufferId == rt.RenderBufferId &&
		Width == rt.Width &&
		Height == rt.Height &&
		TextureWidth == rt.TextureWidth &&
		TextureHeight == rt.TextureHeight &&
#endif
		UV == rt.UV;
	public override readonly bool Equals(object? other) => other is RenderTarget rt ? Equals(rt) : false;
#if NETSTANDARD2_1 || NETCOREAPP2_1_OR_GREATER
	public override readonly int GetHashCode() => HashCode.Combine(HashCode.Combine(IsEmpty, Width, Height, TextureId), HashCode.Combine(TextureWidth, TextureHeight, TextureFormat, UV), RenderBufferId);
#endif
}
