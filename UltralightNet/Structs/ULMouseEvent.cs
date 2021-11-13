using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	public static partial class Methods
	{
		[DllImport("Ultralight")]
		public static extern IntPtr ulCreateMouseEvent(ULMouseEvent.ULMouseEventType type, int x, int y, ULMouseEvent.Button button);

		[DllImport("Ultralight")]
		public static extern void ulDestroyMouseEvent(IntPtr evt);
	}
	/// <summary>
	/// Mouse Event
	/// </summary>
	public ref struct ULMouseEvent
	{
		/// <summary>
		/// Type of event
		/// </summary>
		public enum ULMouseEventType
		{
			MouseMoved,
			MouseDown,
			MouseUp
		}
		/// <summary>
		/// Mouse Button
		/// </summary>
		public enum Button
		{
			None,
			Left,
			Middle,
			Right
		}
		public ULMouseEventType type;
		public int x;
		public int y;
		public Button button;
	}
}
