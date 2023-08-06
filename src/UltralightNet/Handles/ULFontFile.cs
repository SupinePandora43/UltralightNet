using System.Runtime.InteropServices;

namespace UltralightNet.Platform;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct ULFontFile : IDisposable // TODO: INativeContainer ?
{
	private nuint handle;

	private ULFontFile(nuint handle) { this.handle = handle; }

	public static ULFontFile CreateFromFile(ULString* path) => new(ulFontFileCreateFromFilePath(path));
	public static ULFontFile CreateFromFile(ReadOnlySpan<char> path)
	{
		using ULString pathUL = new(path);
		return CreateFromFile(&pathUL);
	}

	public static ULFontFile Create(ULBuffer buffer) => new(ulFontFileCreateFromBuffer(buffer));

	public void Dispose()
	{
		if (handle is 0) return;

		ulDestroyFontFile(handle);
		handle = 0;
	}

	[DllImport(Methods.LibUltralight)]
	static extern nuint ulFontFileCreateFromFilePath(ULString* path);
	[DllImport(Methods.LibUltralight)]
	static extern nuint ulFontFileCreateFromBuffer(ULBuffer buffer);
	[DllImport(Methods.LibUltralight)]
	static extern void ulDestroyFontFile(nuint handle);
}
