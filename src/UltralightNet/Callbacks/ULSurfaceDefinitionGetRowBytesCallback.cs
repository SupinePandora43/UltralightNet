using System.Runtime.InteropServices;

namespace UltralightNet
{
	/// <summary>
	/// known as "stride"
	/// </summary>
	/// <example>width * 4</example>
	/// <param name="userData"></param>
	/// <returns></returns>
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate uint ULSurfaceDefinitionGetRowBytesCallback(void* userData);
}
