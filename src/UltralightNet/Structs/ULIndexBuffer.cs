using System;

namespace UltralightNet;

public unsafe struct ULIndexBuffer : IEquatable<ULIndexBuffer>
{
	public uint size;
	public void* data;

	public readonly bool Equals(ULIndexBuffer other)
	{
		if (size != other.size) return false;
		else if (data == other.data) return true;
		else if (size < (uint)int.MaxValue) return new ReadOnlySpan<byte>(data, (int)size).SequenceEqual(new ReadOnlySpan<byte>(other.data, (int)size));
		else for (uint i = 0; i < size; i++) if (((byte*)data)[i] != ((byte*)other.data)[i]) return false;
		return true;
	}
}
