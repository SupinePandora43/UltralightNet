using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		/// <summary>Whether or not a ULRect is empty (all members equal to 0)</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial bool ulRectIsEmpty(ULRect rect);

		/// <summary>Create an empty ULRect (all members equal to 0)</summary>
		[GeneratedDllImport("Ultralight")]
		public static partial ULRect ulRectMakeEmpty();
	}

	[BlittableType]
	public struct ULRect
	{
		public float left;
		public float top;
		public float right;
		public float bottom;

		public static ULRect MakeEmpty() => Methods.ulRectMakeEmpty();

		public bool IsEmpty() => Methods.ulRectIsEmpty(this);
	}
}
