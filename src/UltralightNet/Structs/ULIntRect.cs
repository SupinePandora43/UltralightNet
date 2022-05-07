using System.Runtime.InteropServices;

namespace UltralightNet
{
	[BlittableType]
	public struct ULIntRect
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;

		public bool IsEmpty => (Left == Right) || (Top == Bottom);
	}
}
