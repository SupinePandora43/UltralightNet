using System;

namespace UltralightNet
{
	//todo
	public struct ULCommandList
	{
		public uint size;
		public IntPtr commandsPtr;

		public Span<ULCommand> ToSpan()
		{
			unsafe
			{
				return new((void*)commandsPtr, (int)size);
			}
		}
	}
}
