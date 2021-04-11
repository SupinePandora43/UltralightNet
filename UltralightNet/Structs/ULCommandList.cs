using System;
using System.Runtime.CompilerServices;

namespace UltralightNet
{
	//todo
	public struct ULCommandList
	{
		public uint size;
		public IntPtr commandsPtr;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySpan<ULCommand> ToSpan()
		{
			unsafe
			{
				return new((void*)commandsPtr, (int)size);
			}
		}
	}
}
