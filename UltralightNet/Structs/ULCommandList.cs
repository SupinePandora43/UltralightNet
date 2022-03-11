using System;
using System.Runtime.CompilerServices;

namespace UltralightNet
{
	public unsafe ref struct ULCommandList
	{
		public uint size;
		public ULCommand* commandsPtr;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySpan<ULCommand> ToSpan()
		{
			return new(commandsPtr, (int)size);
		}
	}
}
