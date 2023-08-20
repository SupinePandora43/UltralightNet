using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UltralightNet.JavaScript;
using UltralightNet.JavaScript.Low;

namespace UltralightNet.Test;

[Collection("Renderer")]
[Trait("Category", "JS")]
public unsafe sealed class JSBasic
{
	Renderer Renderer { get; }
	public JSBasic(RendererFixture fixture) => Renderer = fixture.Renderer;

	[Fact]
	public void GetMessageTest()
	{
		using var view = Renderer.CreateView(128, 128);
		var ctx = view.LockJSContext();

		using var name = JSString.CreateFromUTF16("GetMessage");
		JSObjectRef func = JavaScriptMethods.JSObjectMakeFunctionWithCallback(ctx, name.JSHandle, &GetMessage);
		JavaScriptMethods.JSObjectSetProperty(ctx, JavaScriptMethods.JSContextGetGlobalObject(ctx), name.JSHandle, func);

		view.UnlockJSContext();

		var result = view.EvaluateScript("GetMessage()", out var exception);
		Assert.Equal("Hello from C#!", result);
		Assert.Empty(exception);
	}

	[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
	static JSValueRef GetMessage(JSContextRef ctx, JSObjectRef function, JSObjectRef thisObject, nuint argumentCount, JSValueRef* arguments, JSValueRef* exception)
	{
		{
			var thisObjectName = JavaScriptMethods.JSValueToStringCopy(ctx, thisObject);
			using var wrappedThisObjectName = JSString.FromHandle(thisObjectName, true);
			Assert.Equal("[object Window]", wrappedThisObjectName.ToString());
		}
		Assert.Equal((nuint)0, argumentCount); // (even though 'argumentCount' is 0, 'arguments' itself may not be null)

		using var jsString = JSString.CreateFromUTF16("Hello from C#!");
		JSValueRef value = JavaScriptMethods.JSValueMakeString(ctx, jsString.JSHandle);
		return value;
	}
}
