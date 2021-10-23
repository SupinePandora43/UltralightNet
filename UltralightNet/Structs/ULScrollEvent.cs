using System.Runtime.InteropServices;

namespace UltralightNet
{
	/// <summary>
	/// Scroll event
	/// </summary>
	[NativeMarshalling(typeof(ULScrollEventNative))]
	public ref struct ULScrollEvent
	{
		/// <summary>
		/// Type of event
		/// </summary>
		public enum ScrollType
		{
			ByPixel,
			ByPage
		}

		/// <summary>
		/// Type of event
		/// </summary>
		public ScrollType type;
		/// <summary>
		/// horizontal scroll
		/// </summary>
		public int deltaX;
		/// <summary>
		/// vertical scroll
		/// </summary>
		public int deltaY;
	}

	[BlittableType]
	[StructLayout(LayoutKind.Sequential)]
	internal ref struct ULScrollEventNative
	{
		public int type;
		public int deltaX;
		public int deltaY;

		public ULScrollEventNative(ULScrollEvent scrollEvent)
		{
			type = (int)scrollEvent.type;
			deltaX = scrollEvent.deltaX;
			deltaY = scrollEvent.deltaY;
		}

		public ULScrollEvent ToManaged() => new()
		{
			type = (ULScrollEvent.ScrollType)type,
			deltaX = deltaX,
			deltaY = deltaY
		};
	}
}
