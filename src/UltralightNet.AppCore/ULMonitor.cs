using System.Runtime.InteropServices;

namespace UltralightNet.AppCore;

public static partial class AppCoreMethods
{
	[DllImport(LibAppCore)]
	public static extern double ulMonitorGetScale(ULMonitor monitor);

	[DllImport(LibAppCore)]
	public static extern uint ulMonitorGetWidth(ULMonitor monitor);

	[DllImport(LibAppCore)]
	public static extern uint ulMonitorGetHeight(ULMonitor monitor);
}

public readonly struct ULMonitor
{
	private readonly nuint _handle;

	public readonly double Scale => AppCoreMethods.ulMonitorGetScale(this);
	public readonly uint Width => AppCoreMethods.ulMonitorGetWidth(this);
	public readonly uint Height => AppCoreMethods.ulMonitorGetHeight(this);

	public readonly ULWindow CreateWindow(uint width, uint height, bool fullscreen = false, ULWindowFlags flags = ULWindowFlags.Titled | ULWindowFlags.Resizable | ULWindowFlags.Maximizable) => ULWindow.FromHandle(AppCoreMethods.ulCreateWindow(this, width, height, fullscreen, flags), true);
}
