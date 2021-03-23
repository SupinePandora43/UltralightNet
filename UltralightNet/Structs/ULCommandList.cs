using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	//todo
	public struct ULCommandList
	{
		public uint size;
		public IntPtr commandsPtr;

		public ULCommand[] ToArray()
		{
			ULCommand[] commands = new ULCommand[size];

			for (int i = 0; i < size; i++)
			{
				IntPtr structurePtr = Marshal.ReadIntPtr(commandsPtr, i);
				commands[i] = Marshal.PtrToStructure<ULCommand>(structurePtr);
			}

			return commands;
		}
	}
}
