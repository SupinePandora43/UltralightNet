using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet;

public static unsafe partial class Methods
{
	[DllImport(LibUltralight)]
	public static extern ULMouseEvent* ulCreateMouseEvent(ULMouseEventType type, int x, int y, ULMouseEventButton button);

	[DllImport(LibUltralight)]
	public static extern void ulDestroyMouseEvent(ULMouseEvent* evt);
}

/// <summary>
/// Mouse Event
/// </summary>
public struct ULMouseEvent : IEquatable<ULMouseEvent>
{
	private int _Type;
	public ULMouseEventType Type { readonly get => Unsafe.As<int, ULMouseEventType>(ref Unsafe.AsRef(_Type)); set => _Type = Unsafe.As<ULMouseEventType, int>(ref value); }
	public int X;
	public int Y;
	private int _Button;
	public ULMouseEventButton Button { readonly get => Unsafe.As<int, ULMouseEventButton>(ref Unsafe.AsRef(_Button)); set => _Button = Unsafe.As<ULMouseEventButton, int>(ref value); }

	public readonly bool Equals(ULMouseEvent other) => Type == other.Type && X == other.X && Y == other.Y && Button == other.Button;
}
