using System.Runtime.InteropServices;

namespace UltralightNet
{
	/// <summary>
	/// Mouse Event
	/// </summary>
	[NativeMarshalling(typeof(ULMouseEventNative))]
	public struct ULMouseEvent
	{
		/// <summary>
		/// Type of event
		/// </summary>
		public enum Type
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

		/// <summary>
		/// Type of event
		/// </summary>
		public Type type;
		/// <summary>
		/// X position of mouse
		/// </summary>
		public int x;
		/// <summary>
		/// Y position of mouse
		/// </summary>
		public int y;
		/// <summary>
		/// Mouse Button
		/// </summary>
		public Button button;
	}

	[BlittableType]
	internal struct ULMouseEventNative
	{
		public int type;
		public int x;
		public int y;
		public int button;

		public ULMouseEventNative(ULMouseEvent mouseEvent)
		{
			type = (int)mouseEvent.type;
			x = mouseEvent.x;
			y = mouseEvent.y;
			button = (int)mouseEvent.button;
		}

		public ULMouseEvent ToManaged() => new()
		{
			type = (ULMouseEvent.Type)type,
			x = x,
			y = y,
			button = (ULMouseEvent.Button)button
		};
	}
}
