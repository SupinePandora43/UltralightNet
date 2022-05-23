using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ULGPUState
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

		private float scalar_0;
		private float scalar_1;
		private float scalar_2;
		private float scalar_3;
		private float scalar_4;
		private float scalar_5;
		private float scalar_6;
		private float scalar_7;
		public Span<float> Scalar =>
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1
			MemoryMarshal.CreateSpan<float>(ref scalar_0, 8);
#else
			new Span<float>(Unsafe.AsPointer(ref scalar_0), 8);
#endif

		private Vector4 vector_0;
		private Vector4 vector_1;
		private Vector4 vector_2;
		private Vector4 vector_3;
		private Vector4 vector_4;
		private Vector4 vector_5;
		private Vector4 vector_6;
		private Vector4 vector_7;
		public Span<Vector4> Vector =>
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1
			MemoryMarshal.CreateSpan<Vector4>(ref vector_0, 8);
#else
			new Span<Vector4>(Unsafe.AsPointer(ref vector_0), 8);
#endif

		private byte _ClipSize;
		public byte ClipSize {
			get => _ClipSize;
			set {
				static void Throw(){ throw new ArgumentOutOfRangeException(nameof(value), "ClipSize can't be bigger than 8"); }
				if(value <= 8) _ClipSize = value;
				else Throw();
			}
		}

		private Matrix4x4 clip_0;
		private Matrix4x4 clip_1;
		private Matrix4x4 clip_2;
		private Matrix4x4 clip_3;
		private Matrix4x4 clip_4;
		private Matrix4x4 clip_5;
		private Matrix4x4 clip_6;
		private Matrix4x4 clip_7;
		public Span<Matrix4x4> Clip =>
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1
			MemoryMarshal.CreateSpan<Matrix4x4>(ref clip_0, 8).Slice(0, (int)_ClipSize);
#else
			new Span<Matrix4x4>(Unsafe.AsPointer(ref clip_0), (int)ClipSize);
#endif

		private byte _EnableScissor;
		public bool EnableScissor { get => Unsafe.As<byte, bool>(ref _EnableScissor); set => _EnableScissor = Unsafe.As<bool, byte>(ref value); }

		public ULIntRect ScissorRect;
	}
}
