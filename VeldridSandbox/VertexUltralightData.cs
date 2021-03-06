using System.Numerics;

namespace VeldridSandbox
{
	public struct Byte4
	{
		public Byte4(byte x, byte y, byte z, byte w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}
		public byte x;
		public byte y;
		public byte z;
		public byte w;
	}
	public struct VertexUltralightData
	{
		public Vector2 in_Position;
		public Byte4 in_Color;
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
