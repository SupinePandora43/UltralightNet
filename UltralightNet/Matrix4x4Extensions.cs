using System.Numerics;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		[DllImport("Ultralight")]
		public static extern Matrix4x4 ulApplyProjection(Matrix4x4 transform, float viewport_width, float viewport_height, bool flip_y);
	}
	public static class Matrix4x4Extensions
	{
		public static Matrix4x4 ulApplyProjection(in this Matrix4x4 transform, float viewport_width, float viewport_height, bool flip_y) => Methods.ulApplyProjection(transform, viewport_width, viewport_height, flip_y);
	}
}
