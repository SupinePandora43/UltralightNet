using System;

namespace UltralightNet;

/// <summary>An enumeration of the different keyboard modifiers.</summary>
[Flags]
public enum ULKeyEventModifiers : byte // KeyEvent.h - uint8_t, CAPI_KeyEvent.h - unsigned int
{
	/// <summary>Whether or not an ALT key is down</summary>
	AltKey = 1 << 0,
	/// <summary>Whether or not a Control key is down</summary>
	CtrlKey = 1 << 1,
	/// <summary>Whether or not a meta key (Command-key on Mac, Windows-key on Win) is down</summary>
	MetaKey = 1 << 2,
	/// <summary>Whether or not a Shift key is down</summary>
	ShiftKey = 1 << 3
}
