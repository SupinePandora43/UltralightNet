using System.Numerics;

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

		public PixelShaderData pixelData;

		public bool enable_scissor;

		public ULIntRect scissor_rect;

		public struct PixelShaderData
		{
			public float scalar_0;
			public float scalar_1;
			public float scalar_2;
			public float scalar_3;
			public float scalar_4;
			public float scalar_5;
			public float scalar_6;
			public float scalar_7;

			public Vector4 vector_0;
			public Vector4 vector_1;
			public Vector4 vector_2;
			public Vector4 vector_3;
			public Vector4 vector_4;
			public Vector4 vector_5;
			public Vector4 vector_6;
			public Vector4 vector_7;

			public byte clip_size;

			public Matrix4x4 clip_0;
			public Matrix4x4 clip_1;
			public Matrix4x4 clip_2;
			public Matrix4x4 clip_3;
			public Matrix4x4 clip_4;
			public Matrix4x4 clip_5;
			public Matrix4x4 clip_6;
			public Matrix4x4 clip_7;
		}
	}
}
