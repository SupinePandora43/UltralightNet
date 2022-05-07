using System.Runtime.CompilerServices;

namespace UltralightNet;

/// <summary>
/// Scroll event
/// </summary>
public ref struct ULScrollEvent
{
	private int _Type;
	/// <summary>
	/// Type of event
	/// </summary>
	public ULScrollEventType Type { get => Unsafe.As<int, ULScrollEventType>(ref _Type); set => _Type = Unsafe.As<ULScrollEventType, int>(ref value); }
	/// <summary>
	/// horizontal scroll
	/// </summary>
	public int DeltaX;
	/// <summary>
	/// vertical scroll
	/// </summary>
	public int DeltaY;
}
