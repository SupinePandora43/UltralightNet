using System.Runtime.CompilerServices;

namespace UltralightNet.JavaScript;

public readonly ref struct JSPropertyNameAccumulatorRef
{
	private readonly nuint _value;

	public void AddName(JSStringRef jsString) => JavaScriptMethods.JSPropertyNameAccumulatorAddName(this, jsString);
}

unsafe partial class JavaScriptMethods
{
	public static void AddName(ref this JSPropertyNameAccumulator accumulator, void* jsString)
	{
		delegate*<ref nint, void*> asPointer = &Unsafe.AsPointer<nint>;
		delegate*<ref JSPropertyNameAccumulator, JSPropertyNameAccumulator*> b = (delegate*<ref JSPropertyNameAccumulator, JSPropertyNameAccumulator*>)asPointer;
		JavaScriptMethods.JSPropertyNameAccumulatorAddName(b(ref accumulator), jsString);
	}
	public static void AddName(ref this JSPropertyNameAccumulator accumulator, JSString jsString)
	{
		delegate*<ref nint, void*> asPointer = &Unsafe.AsPointer<nint>;
		delegate*<ref JSPropertyNameAccumulator, JSPropertyNameAccumulator*> b = (delegate*<ref JSPropertyNameAccumulator, JSPropertyNameAccumulator*>)asPointer;
		b(ref accumulator)->AddName(jsString.Handle);
	}
}
