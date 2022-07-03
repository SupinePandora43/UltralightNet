using System;
using System.Runtime.InteropServices;

namespace UltralightNet;

public unsafe struct ULFontFile : IDisposable // TODO: INativeContainer
{
	private nuint handle;

	public ULFontFile(ULString* path)
	{
		[DllImport(Methods.LibUltralight)]
		static extern nuint ulFontFileCreateFromFilePath(ULString* path);
		handle = ulFontFileCreateFromFilePath(path);
	}
	public ULFontFile(ULBuffer buffer)
	{
		[DllImport(Methods.LibUltralight)]
		static extern nuint ulFontFileCreateFromBuffer(ULBuffer buffer);
		handle = ulFontFileCreateFromBuffer(buffer);
	}

	public void Dispose()
	{
		if (handle is 0) return;
		[DllImport(Methods.LibUltralight)]
		static extern void ulDestroyFontFile(nuint handle);
		ulDestroyFontFile(handle);
		handle = 0;
	}
}
