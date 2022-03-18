using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		[DllImport("Ultralight")]
		public static extern IntPtr ulCreateMouseEvent(ULMouseEventType type, int x, int y, ULMouseEventButton button);

		[DllImport("Ultralight")]
		public static extern void ulDestroyMouseEvent(IntPtr evt);
	}
	/// <summary>
	/// Mouse Event
	/// </summary>
	public ref struct ULMouseEvent
	{
		public ULMouseEventType type;
		public int x;
		public int y;
		public ULMouseEventButton button;
	}
}
