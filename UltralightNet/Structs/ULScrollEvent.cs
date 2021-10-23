using System.Runtime.InteropServices;

namespace UltralightNet
{
	/// <summary>
	/// Scroll event
	/// </summary>
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
}
