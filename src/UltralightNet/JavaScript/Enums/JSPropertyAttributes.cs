using System;

namespace UltralightNet
{
	[Flags]
	public enum JSPropertyAttributes : uint
	{
		None = 0,
		ReadOnly = 1 << 1,
		DontEnum = 1 << 2,
		DontDelete = 1 << 3
	}
}
