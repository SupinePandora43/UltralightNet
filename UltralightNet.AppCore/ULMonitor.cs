using System;
using System.Runtime.InteropServices;

namespace UltralightNet.AppCore
{
	public static partial class AppCoreMethods
	{
		[DllImport("AppCore")]
		public static extern double ulMonitorGetScale(IntPtr monitor);

		[DllImport("AppCore")]
		public static extern uint ulMonitorGetWidth(IntPtr monitor);

		[DllImport("AppCore")]
		public static extern uint ulMonitorGetHeight(IntPtr monitor);
	}

	public class ULMonitor
	{
		public IntPtr Ptr { get; private set; }

		public ULMonitor(IntPtr ptr)
		{
			Ptr = ptr;
		}

		public double Scale => AppCoreMethods.ulMonitorGetScale(Ptr);
		public uint Width => AppCoreMethods.ulMonitorGetWidth(Ptr);
		public uint Height => AppCoreMethods.ulMonitorGetHeight(Ptr);
	}
}
