using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UltralightNet;
using UltralightNet.AppCore;

namespace UltralightNetTestApplication
{
	class Program
	{
		unsafe static void Main()
		{
			AppCoreMethods.ulEnablePlatformFileSystem("./");
			AppCoreMethods.ulEnablePlatformFontLoader();

			Renderer renderer = ULPlatform.CreateRenderer(new());
			View view = renderer.CreateView(512, 512);

			ref readonly JSContext context = ref view.LockJSContext();

			// context.GlobalObject["GetMessage"] = (JSObject) (arguments) => (JSValue) "Hello from C#!";

			delegate* unmanaged[Cdecl]<void*, void*, void*, nuint, void**, void**, void*> f = &GetMessage;
			context.GlobalObject["GetMessage"] = f;

			Console.WriteLine(context.EvaluateScript("GetMessage()"));

			view.UnlockJSContext();

			Console.WriteLine(view.EvaluateScript("GetMessage()", out _));
		}

		[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
		static unsafe void* GetMessage(void* context, void* function, void* thisObject, nuint argumentCount, void** arguments, void** exception)
		{
			JSValue value = "Hello from C#!";
			value.Context = context;
			return value.Handle;
		}
		static JSValue GetMessageManaged(JSContext context, JSObject function, JSObject thisObject, ReadOnlySpan<JSValue> arguments, out JSValue exception)
		{
			exception = null;
			return context.MakeString("Hello from C#!");
		}
	}
}
