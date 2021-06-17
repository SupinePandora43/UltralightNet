using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public struct ULRenderBuffer
	{
		public uint texture_id;
		public uint width;
		public uint height;

		[MarshalAs(UnmanagedType.I1)]
		public bool has_stencil_buffer;
		[MarshalAs(UnmanagedType.I1)]
		public bool has_depth_buffer;
	}
}
