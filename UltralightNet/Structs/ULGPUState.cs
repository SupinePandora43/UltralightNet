using System.Numerics;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public struct ULGPUState
	{
		public uint viewport_width;

		public uint viewport_height;

		public Matrix4x4 transform;

		public bool enable_texturing;

		public bool enable_blend;

		public ULShaderType shader_type;

		public uint render_buffer_id;

		public uint texture_1_id;

		public uint texture_2_id;

		public uint texture_3_id;

		/// pass to pixel shader
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public float[] uniform_scalar;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public Vector4[] puniform_vector;
		public byte clip_size;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public Matrix4x4[] clip;

		public bool enable_scissor;

		public ULIntRect scissor_rect;
	}
}
