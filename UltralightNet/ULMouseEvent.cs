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
	[NativeMarshalling(typeof(ULMouseEventNative))]
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

	internal ref struct ULMouseEventNative
	{
		public ULMouseEventNative(ULMouseEvent evt)
		{
			type = (int)evt.type;
			x = evt.x;
			y = evt.y;
			button = (int)evt.button;
		}

		public int type;
		public int x;
		public int y;
		public int button;

		public ULMouseEvent ToManaged() => new() { type = (ULMouseEvent.ULMouseEventType)type, x = x, y = y, button = (ULMouseEvent.Button)button };
	}
}
