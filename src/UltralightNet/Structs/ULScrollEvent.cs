using System;
using System.Runtime.CompilerServices;

namespace UltralightNet;

/// <summary>
/// Scroll event
/// </summary>
public struct ULScrollEvent : IEquatable<ULScrollEvent>
{
	private int _Type;
	/// <summary>
	/// Type of event
	/// </summary>
	public ULScrollEventType Type { readonly get => Unsafe.As<int, ULScrollEventType>(ref Unsafe.AsRef(_Type)); set => _Type = Unsafe.As<ULScrollEventType, int>(ref value); }

	/// <summary>
	/// horizontal scroll
	/// </summary>
	public int DeltaX;

	/// <summary>
	/// vertical scroll
	/// </summary>
	public int DeltaY;

	public readonly bool Equals(ULScrollEvent other) => Type == other.Type && DeltaX == other.DeltaX && DeltaY == other.DeltaY;
}
