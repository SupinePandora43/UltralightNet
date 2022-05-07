using System.Runtime.InteropServices;

namespace UltralightNet
{
	[BlittableType]
	public struct ULRect
	{
		public float Left;
		public float Top;
		public float Right;
		public float Bottom;

		public bool IsEmpty => (Left == Right) || (Top == Bottom);
	}
}
