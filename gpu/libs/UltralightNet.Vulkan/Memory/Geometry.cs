using Silk.NET.Vulkan;

namespace UltralightNet.Vulkan.Memory;

public readonly struct BufferResource
{
	public Buffer Buffer { readonly get; init; }
	public ulong Offset { readonly get; init; }
	public ulong Size { readonly get; init; }
	public unsafe void* Mapped { readonly get; init; }
}
