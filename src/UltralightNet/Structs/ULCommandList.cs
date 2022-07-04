using System;
using System.Runtime.CompilerServices;

namespace UltralightNet;

public unsafe ref struct ULCommandList // TODO: IEquatable<ULCommandList>
{
	public uint size;
	public ULCommand* commandsPtr;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<ULCommand> ToSpan()
	{
		return new(commandsPtr, checked((int)size));
	}

	public readonly bool Equals(ULCommandList other){
		if(size != other.size) return false;
		else if(size < (uint)int.MaxValue) return new ReadOnlySpan<ULCommand>(commandsPtr, unchecked((int)size)).SequenceEqual(new ReadOnlySpan<ULCommand>(other.commandsPtr, unchecked((int)size)));
		else for(uint i = 0; i < size; i++) if(!commandsPtr[i].Equals(other.commandsPtr[i])) return false;
		return true;
	}
}
