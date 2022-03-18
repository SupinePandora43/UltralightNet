using System;

namespace UltralightNet
{
	[Flags]
	public enum JSClassAttributes : uint
	{
		None = 0,
		NoAutomaticPrototype = 1 << 1
	}
}
