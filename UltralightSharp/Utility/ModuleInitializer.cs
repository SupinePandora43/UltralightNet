namespace ImpromptuNinjas.UltralightSharp.Utility
{
	public static class ModuleInitializer
	{
		public static void Initialize()
		{
#if NETCOREAPP3_0_OR_GREATER
			NativeOSXFix.Init();
#endif
		}
	}
}
