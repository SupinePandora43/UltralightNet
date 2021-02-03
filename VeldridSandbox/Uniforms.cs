using System.Numerics;

namespace VeldridSandbox
{
	/// <summary>
	/// Uniforms
	/// </summary>
	public struct Uniforms
	{
		public Vector4 State; // 16
		public Matrix4x4 Transform; // 64

		public Vector4 Scalar4_0; // 16
		public Vector4 Scalar4_1; // 16

		public Vector4 Vector_0; // 16
		public Vector4 Vector_1; // 16
		public Vector4 Vector_2; // 16
		public Vector4 Vector_3; // 16
		public Vector4 Vector_4; // 16
		public Vector4 Vector_5; // 16
		public Vector4 Vector_6; // 16
		public Vector4 Vector_7; // 16

		public float fClipSize;  // 4

		public Matrix4x4 Clip_0; // 64
		public Matrix4x4 Clip_1; // 64
		public Matrix4x4 Clip_2; // 64
		public Matrix4x4 Clip_3; // 64
		public Matrix4x4 Clip_4; // 64
		public Matrix4x4 Clip_5; // 64
		public Matrix4x4 Clip_6; // 64
		public Matrix4x4 Clip_7; // 64
	}
}
