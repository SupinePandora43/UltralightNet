using UltralightNet.JavaScript.Low;

namespace UltralightNet.JavaScript;

public unsafe struct JSStaticFunction
{
	public byte* name;
	public delegate* unmanaged[Cdecl]<void*, void*, void*, nuint, void**, void**, void*> callAsFunction;
	public JSPropertyAttributes attributes;
}
public unsafe struct JSStaticFunctionEx
{
	public byte* name;
	public delegate* unmanaged[Cdecl]<void*, void*, void*, void*, void*, nuint, void**, void**, void*> callAsFunctionEx;
	public JSPropertyAttributes attributes;
}
