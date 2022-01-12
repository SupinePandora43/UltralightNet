using System.Runtime.InteropServices;

namespace UltralightNet
{
	[BlittableType]
	public struct ULIntRect
	{
		public int left;
		public int top;
		public int right;
		public int bottom;


		public bool IsEmpty => (left == right) && (top == bottom);
	}
}
