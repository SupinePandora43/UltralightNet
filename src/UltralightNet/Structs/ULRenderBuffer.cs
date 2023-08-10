using System.Runtime.CompilerServices;

namespace UltralightNet;

public struct ULRenderBuffer
{
	public uint TextureId;
	public uint Width;
	public uint Height;

	private byte _HasStencilBuffer;
	public bool HasStencilBuffer
	{
		readonly get => Unsafe.As<byte, bool>(ref Unsafe.AsRef(_HasStencilBuffer));
		set => _HasStencilBuffer = Unsafe.As<bool, byte>(ref Unsafe.AsRef(value));
	}
	private byte _HasDepthBuffer;
	public bool HasDepthBuffer
	{
		readonly get => Unsafe.As<byte, bool>(ref Unsafe.AsRef(_HasDepthBuffer));
		set => _HasDepthBuffer = Unsafe.As<bool, byte>(ref Unsafe.AsRef(value));
	}
}
