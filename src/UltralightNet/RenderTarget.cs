using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	/// <summary>Rendering details for a View, to be used with your own GPUDriver</summary>
	[BlittableType]
	public struct RenderTarget
	{
		private byte _IsEmpty;
		/// <summary>Whether this target is empty (null texture)</summary>
		public bool IsEmpty { get => Unsafe.As<byte, bool>(ref _IsEmpty); set => _IsEmpty = Unsafe.As<bool, byte>(ref value); }

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
		public ULBitmapFormat TextureFormat { get => Unsafe.As<byte, ULBitmapFormat>(ref _TextureFormat); set => _TextureFormat = Unsafe.As<ULBitmapFormat, byte>(ref value); }

		/// <summary>UV coordinates of the texture (this is needed because the texture may be padded).</summary>
		public ULRect UV;

		/// <summary><see cref="ULGPUDriver" />'s render buffer id</summary>
		public uint RenderBufferId;
	}
}
