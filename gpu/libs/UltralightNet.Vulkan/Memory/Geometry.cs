using System;

namespace UltralightNet.GPU.Vulkan;

internal readonly struct BufferResource
{
	public Silk.NET.Vulkan.Buffer Buffer { readonly get; init; }
	public ulong Offset { readonly get; init; }
	public ulong Size { readonly get; init; }
	public unsafe void* Mapped { readonly get; init; }

	public unsafe readonly Span<byte> AsSpan() => new(Mapped, (int)Size);
}
