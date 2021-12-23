using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[BlittableType]
	public unsafe struct ULVertexBuffer
	{
		public ULVertexBufferFormat format;
		public uint size;
		public void* data;
	}
}
