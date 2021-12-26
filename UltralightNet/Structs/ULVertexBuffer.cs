using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	//[BlittableType]
	public unsafe struct ULVertexBuffer
	{
		private int format;
		public uint size;
		public void* data;

		public ULVertexBufferFormat Format
		{
			get => (ULVertexBufferFormat)format;
			set => format = (int)value;
		}
	}
}
