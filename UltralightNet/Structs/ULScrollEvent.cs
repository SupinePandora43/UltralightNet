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
		public ULScrollEventType type;
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
