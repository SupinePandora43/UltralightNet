using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if NETCOREAPP3_0_OR_GREATER
using System.Runtime.Intrinsics;
#endif

namespace UltralightNet;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct ULGPUState : IEquatable<ULGPUState>
{
	public uint ViewportWidth;

	public uint ViewportHeight;

	public Matrix4x4 Transform;

	private byte _EnableTexturing;
	public bool EnableTexturing { readonly get => Unsafe.As<byte, bool>(ref Unsafe.AsRef(_EnableTexturing)); set => _EnableTexturing = Unsafe.As<bool, byte>(ref value); }

	private byte _EnableBlend;
	public bool EnableBlend { readonly get => Unsafe.As<byte, bool>(ref Unsafe.AsRef(_EnableBlend)); set => _EnableBlend = Unsafe.As<bool, byte>(ref value); }

	private byte _ShaderType;
	public ULShaderType ShaderType { readonly get => Unsafe.As<byte, ULShaderType>(ref Unsafe.AsRef(_ShaderType)); set => _ShaderType = Unsafe.As<ULShaderType, byte>(ref value); }

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
	public readonly Span<float> Scalar =>
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1
		MemoryMarshal.CreateSpan<float>(ref Unsafe.AsRef(scalar_0), 8);
#else
		new Span<float>(Unsafe.AsPointer(ref Unsafe.AsRef(scalar_0)), 8);
#endif

	private Vector4 vector_0;
	private Vector4 vector_1;
	private Vector4 vector_2;
	private Vector4 vector_3;
	private Vector4 vector_4;
	private Vector4 vector_5;
	private Vector4 vector_6;
	private Vector4 vector_7;
	public readonly Span<Vector4> Vector =>
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1
		MemoryMarshal.CreateSpan<Vector4>(ref Unsafe.AsRef(vector_0), 8);
#else
		new Span<Vector4>(Unsafe.AsPointer(ref Unsafe.AsRef(vector_0)), 8);
#endif

	private byte _ClipSize;
	public byte ClipSize
	{
		readonly get => _ClipSize;
		set
		{
			static void Throw() { throw new ArgumentOutOfRangeException(nameof(value), "ClipSize can't be bigger than 8"); }
			if (value <= 8) _ClipSize = value;
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
	public readonly Span<Matrix4x4> Clip =>
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1
		MemoryMarshal.CreateSpan<Matrix4x4>(ref Unsafe.AsRef(clip_0), 8).Slice(0, (int)_ClipSize);
#else
		new Span<Matrix4x4>(Unsafe.AsPointer(ref Unsafe.AsRef(clip_0)), (int)ClipSize);
#endif

	private byte _EnableScissor;
	public bool EnableScissor { readonly get => Unsafe.As<byte, bool>(ref Unsafe.AsRef(_EnableScissor)); set => _EnableScissor = Unsafe.As<bool, byte>(ref value); }

	public ULIntRect ScissorRect;

	public readonly bool Equals(ULGPUState other) =>
#if NETCOREAPP3_0_OR_GREATER
		Vector64.Create(ViewportWidth, ViewportHeight).Equals(Vector64.Create(other.ViewportWidth, other.ViewportHeight))
#else
		ViewportWidth == other.ViewportWidth && ViewportHeight == other.ViewportHeight
#endif
		&& Transform == other.Transform
		&& EnableTexturing == other.EnableTexturing && EnableBlend == other.EnableBlend
#if NETCOREAPP3_0_OR_GREATER
		&& Vector128.Create(RenderBufferId, Texture1Id, Texture2Id, Texture3Id).Equals(Vector128.Create(other.RenderBufferId, other.Texture1Id, other.Texture2Id, other.Texture3Id))
		&& Vector256.Create(scalar_0, scalar_1, scalar_2, scalar_3, scalar_4, scalar_5, scalar_6, scalar_7).Equals(Vector256.Create(other.scalar_0, other.scalar_1, other.scalar_2, other.scalar_3, other.scalar_4, other.scalar_5, other.scalar_6, other.scalar_7))
#else
		&& RenderBufferId == other.RenderBufferId && Texture1Id == other.Texture1Id && Texture2Id == other.Texture2Id && Texture3Id == other.Texture3Id
		&& Scalar.SequenceEqual(other.Scalar)
#endif
		&& Vector.SequenceEqual(other.Vector)
		&& ClipSize == other.ClipSize && Clip.SequenceEqual(other.Clip)
		&& EnableScissor == other.EnableScissor
		&& ScissorRect == other.ScissorRect;
	public override readonly bool Equals(object? other) => other is ULGPUState otherState && Equals(otherState);
}
