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
public struct ULMouseEvent
{
	private int _Type;
	public ULMouseEventType Type { get => Unsafe.As<int, ULMouseEventType>(ref _Type); set => _Type = Unsafe.As<ULMouseEventType, int>(ref value); }
	public int X;
	public int Y;
	private int _Button;
	public ULMouseEventButton Button { get => Unsafe.As<int, ULMouseEventButton>(ref _Button); set => _Button = Unsafe.As<ULMouseEventButton, int>(ref value); }
}
