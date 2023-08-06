using System.Runtime.CompilerServices;

namespace UltralightNet;

public unsafe ref struct ULVertexBuffer
{
	private byte format;
	public uint size;
	public void* data;

	public ULVertexBufferFormat Format
	{
		readonly get => Unsafe.As<byte, ULVertexBufferFormat>(ref Unsafe.AsRef(format));
		set => format = Unsafe.As<ULVertexBufferFormat, byte>(ref value);
	}
}
