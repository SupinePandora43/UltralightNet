using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	//[BlittableType]
	public unsafe struct ULVertexBuffer
	{
		private byte format;
		public uint size;
		public void* data;

		public ULVertexBufferFormat Format
		{
			get => (ULVertexBufferFormat)format;
			set => format = (byte)value;
		}
	}
}
