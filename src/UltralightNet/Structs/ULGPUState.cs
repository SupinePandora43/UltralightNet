using System.Numerics;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public struct ULGPUState
	{
		public uint ViewportWidth;

		public uint ViewportHeight;

		public Matrix4x4 Transform;

		private byte _EnableTexturing;
		public bool EnableTexturing { get => Unsafe.As<byte, bool>(ref _EnableTexturing); set => _EnableTexturing = Unsafe.As<bool, byte>(ref value); }

		private byte _EnableBlend;
		public bool EnableBlend { get => Unsafe.As<byte, bool>(ref _EnableBlend); set => _EnableBlend = Unsafe.As<bool, byte>(ref value); }

		private byte _ShaderType;
		public ULShaderType ShaderType { get => Unsafe.As<byte, ULShaderType>(ref _ShaderType); set => _ShaderType = Unsafe.As<ULShaderType, byte>(ref value); }

		public uint RenderBufferId;

		public uint Texture1Id;

		public uint Texture2Id;

		public uint Texture3Id;

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

		public byte ClipSize;

		public Matrix4x4 clip_0;
		public Matrix4x4 clip_1;
		public Matrix4x4 clip_2;
		public Matrix4x4 clip_3;
		public Matrix4x4 clip_4;
		public Matrix4x4 clip_5;
		public Matrix4x4 clip_6;
		public Matrix4x4 clip_7;

		private byte _EnableScissor;
		public bool EnableScissor { get => Unsafe.As<byte, bool>(ref _EnableScissor); set => _EnableScissor = Unsafe.As<bool, byte>(ref value); }

		public ULIntRect ScissorRect;
	}
}
