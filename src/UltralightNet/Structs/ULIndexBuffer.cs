using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	//[BlittableType]
	public unsafe struct ULIndexBuffer
	{
		public uint size;
		public void* data;
	}
}
