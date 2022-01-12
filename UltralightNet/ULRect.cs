using System.Runtime.InteropServices;

namespace UltralightNet
{
	[BlittableType]
	public struct ULRect
	{
		public float left;
		public float top;
		public float right;
		public float bottom;

		public bool IsEmpty => (left == right) && (top == bottom);
	}
}
