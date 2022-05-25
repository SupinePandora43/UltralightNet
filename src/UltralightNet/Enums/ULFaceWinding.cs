namespace UltralightNet
{
	public enum ULFaceWinding: byte // CAPI_Defines.h - no type, platform/Config.h - uint8_t
	{
		/// <summary>Clockwise Winding (Direct3D, etc.)</summary>
		Clockwise,
		/// <summary>Counter-Clockwise Winding (OpenGL, etc.)</summary>
		CounterClockwise
	}
}
