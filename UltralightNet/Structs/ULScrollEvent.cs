using System.Runtime.InteropServices;

namespace UltralightNet
{
	/// <summary>
	/// Scroll event
	/// </summary>
	[NativeMarshalling(typeof(ULScrollEventNative))]
	public struct ULScrollEvent
	{
		/// <summary>
		/// Type of event
		/// </summary>
		public enum Type
		{
			ByPixel,
			ByPage
		}

		/// <summary>
		/// Type of event
		/// </summary>
		public Type type;
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
	internal struct ULScrollEventNative
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
			type = (ULScrollEvent.Type)type,
			deltaX = deltaX,
			deltaY = deltaY
		};
	}
}
