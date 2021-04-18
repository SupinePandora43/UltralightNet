using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[BlittableType]
	public struct ULIndexBuffer
	{
		public uint size;
		public IntPtr data;
	}
}
