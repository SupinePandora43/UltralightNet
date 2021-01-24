using ImpromptuNinjas.UltralightSharp;

public static class ModuleInitializer
{
	public static void Initialize()
	{
#if !NETFRAMEWORK
		Native.Init();
#endif
	}
}
