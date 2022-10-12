using System;
using System.Runtime.CompilerServices;

namespace UltralightNet;

public unsafe struct ULVertexBuffer : IEquatable<ULVertexBuffer>
{
	private byte format;
	public uint size;
	public void* data;

	public ULVertexBufferFormat Format
	{
		readonly get => Unsafe.As<byte, ULVertexBufferFormat>(ref Unsafe.AsRef(format));
		set => format = Unsafe.As<ULVertexBufferFormat, byte>(ref value);
	}

	public readonly bool Equals(ULVertexBuffer other)
	{
		if (size != other.size || Format != other.Format) return false;
		else if (data == other.data) return true;
		else if (size < (uint)int.MaxValue) return new ReadOnlySpan<byte>(data, (int)size).SequenceEqual(new ReadOnlySpan<byte>(other.data, (int)size));
		else for (uint i = 0; i < size; i++) if (((byte*)data)[i] != ((byte*)other.data)[i]) return false;
		return true;
	}
}
