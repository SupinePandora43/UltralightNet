using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet;

[StructLayout(LayoutKind.Sequential)]
public struct ULRenderBuffer
{
	public uint TextureId;
	public uint Width;
	public uint Height;

	private byte _HasStencilBuffer;
	public bool HasStencilBuffer
	{
		get => Unsafe.As<byte, bool>(ref Unsafe.AsRef(_HasStencilBuffer));
		set => _HasStencilBuffer = Unsafe.As<bool, byte>(ref Unsafe.AsRef(value));
	}
	private byte _HasDepthBuffer;
	public bool HasDepthBuffer
	{
		get => Unsafe.As<byte, bool>(ref Unsafe.AsRef(_HasDepthBuffer));
		set => _HasDepthBuffer = Unsafe.As<bool, byte>(ref Unsafe.AsRef(value));
	}
}
