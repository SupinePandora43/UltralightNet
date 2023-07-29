using System.Runtime.CompilerServices;

namespace UltralightNet;

public unsafe ref struct ULCommandList
{
	public uint size;
	public ULCommand* commandsPtr;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly ReadOnlySpan<ULCommand> AsSpan()
	{
		return new(commandsPtr, checked((int)size));
	}
}
