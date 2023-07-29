using System.Runtime.InteropServices;

namespace UltralightNet.Platform.HighPerformance;

internal static class Helper
{
	public static nint AllocateDelegate(Delegate d, out GCHandle handle)
	{
		handle = GCHandle.Alloc(d);
		return Marshal.GetFunctionPointerForDelegate(d);
	}
}
