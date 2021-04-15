using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 ulApplyProjection(Matrix4x4 transform, float viewport_width, float viewport_height, [MarshalAs(UnmanagedType.I1)] bool flip_y) => ulApplyProjection__PInvoke__(transform, viewport_width, viewport_height, (byte)(flip_y ? 1 : 0));

		[DllImport("Ultralight", EntryPoint = "ulApplyProjection")]
		extern public static unsafe Matrix4x4 ulApplyProjection__PInvoke__(Matrix4x4 transform, float viewport_width, float viewport_height, byte flip_y);
}

	public static class Matrix4x4Extensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 ApplyProjection(in this Matrix4x4 transform, float viewport_width, float viewport_height, bool flip_y) => Methods.ulApplyProjection(transform, viewport_width, viewport_height, flip_y);
	}
}
