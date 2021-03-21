using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		/// <summary>Whether or not a ULIntRect is empty (all members equal to 0)</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial bool ulIntRectIsEmpty(ULIntRect rect);

		/// <summary>Create an empty ULIntRect (all members equal to 0)</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial ULIntRect ulIntRectMakeEmpty();
	}

	[BlittableType]
	public struct ULIntRect
	{
		public int left;
		public int top;
		public int right;
		public int bottom;

		public bool IsEmpty() => Methods.ulIntRectIsEmpty(this);

		public bool IsManagedEmpty() => Equals(default(ULIntRect));

		public static ULIntRect MakeEmpty() => Methods.ulIntRectMakeEmpty();

		//public static bool operator ==(ULIntRect a, ULIntRect b) => a.left == b.left && a.top == b.top && a.right == b.right && a.bottom == b.bottom;
		//public static bool operator !=(ULIntRect a, ULIntRect b) => !(a == b);
	}
}
