using System.Runtime.CompilerServices;

namespace UltralightNet;

public readonly ref struct JSPropertyNameArray
{
	public JSPropertyNameArray() => JavaScriptMethods.ThrowUnsupportedConstructor();
}

unsafe partial class JavaScriptMethods
{
	public static JSPropertyNameArray* Retain(ref this JSPropertyNameArray array)
	{
		delegate*<ref nint, void*> asPointer = &Unsafe.AsPointer<nint>;
		delegate*<ref JSPropertyNameArray, JSPropertyNameArray*> b = (delegate*<ref JSPropertyNameArray, JSPropertyNameArray*>)asPointer;
		return JavaScriptMethods.JSPropertyNameArrayRetain(b(ref array));
	}
	public static void Release(ref this JSPropertyNameArray array)
	{
		delegate*<ref nint, void*> asPointer = &Unsafe.AsPointer<nint>;
		delegate*<ref JSPropertyNameArray, JSPropertyNameArray*> b = (delegate*<ref JSPropertyNameArray, JSPropertyNameArray*>)asPointer;
		JavaScriptMethods.JSPropertyNameArrayRelease(b(ref array));
	}
	public static nuint GetCount(ref this JSPropertyNameArray array)
	{
		delegate*<ref nint, void*> asPointer = &Unsafe.AsPointer<nint>;
		delegate*<ref JSPropertyNameArray, JSPropertyNameArray*> b = (delegate*<ref JSPropertyNameArray, JSPropertyNameArray*>)asPointer;
		return JavaScriptMethods.JSPropertyNameArrayGetCount(b(ref array));
	}
	public static void* GetNameAtIndex(ref this JSPropertyNameArray array, nuint index)
	{
		delegate*<ref nint, void*> asPointer = &Unsafe.AsPointer<nint>;
		delegate*<ref JSPropertyNameArray, JSPropertyNameArray*> b = (delegate*<ref JSPropertyNameArray, JSPropertyNameArray*>)asPointer;
		return JavaScriptMethods.JSPropertyNameArrayGetNameAtIndex(b(ref array), index);
	}
}
