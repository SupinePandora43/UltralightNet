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
	public class ULMouseEvent : IDisposable
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
			/// <summary>
			/// Probably a click on mouse wheel
			/// </summary>
			Middle,
			Right
		}

		public readonly IntPtr Ptr;
		public bool IsDisposed { get; private set; }

		public ULMouseEvent(ULMouseEventType type, int x, int y, Button button)
		{
			Ptr = Methods.ulCreateMouseEvent(type, x, y, button);
		}

		~ULMouseEvent() => Dispose();

		public void Dispose()
		{
			if (IsDisposed) return;
			Methods.ulDestroyMouseEvent(Ptr);

			IsDisposed = true;
			GC.SuppressFinalize(this);
		}
	}
}
