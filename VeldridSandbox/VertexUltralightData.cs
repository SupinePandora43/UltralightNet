using System.Numerics;
using System.Runtime.InteropServices;

namespace VeldridSandbox
{
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexUltralightData
	{
		public Vector2 in_Position;
		public Vector4 in_Color;
		public Vector2 in_TexCoord;
		public Vector2 in_ObjCoord;
		public Vector4 in_Data0;
		public Vector4 in_Data1;
		public Vector4 in_Data2;
		public Vector4 in_Data3;
		public Vector4 in_Data4;
		public Vector4 in_Data5;
		public Vector4 in_Data6;
	}
}
