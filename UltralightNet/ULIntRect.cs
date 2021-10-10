using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		/// <summary>Whether or not a ULIntRect is empty (all members equal to 0)</summary>
		[GeneratedDllImport("Ultralight")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulIntRectIsEmpty(ULIntRect rect);

		/// <summary>Create an empty ULIntRect (all members equal to 0)</summary>
		[DllImport("Ultralight")]
		public static extern ULIntRect ulIntRectMakeEmpty();
	}

	[BlittableType]
	public struct ULIntRect
	{
		public int left;
		public int top;
		public int right;
		public int bottom;

		public ULIntRect(int left, int top, int right, int bottom)
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}

		public bool IsEmpty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Methods.ulIntRectIsEmpty(this);
		}

		public bool IsManagedEmpty() => Equals(default(ULIntRect));

		public static ULIntRect MakeEmpty() => Methods.ulIntRectMakeEmpty();

		//public static bool operator ==(ULIntRect a, ULIntRect b) => a.left == b.left && a.top == b.top && a.right == b.right && a.bottom == b.bottom;
		//public static bool operator !=(ULIntRect a, ULIntRect b) => !(a == b);
	}
}
