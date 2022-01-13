using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
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

			Renderer renderer = ULPlatform.CreateRenderer();
			View view = renderer.CreateView(512, 512);

			var context = view.LockJSContext();

			// context.GlobalObject["GetMessage"] = (JSObject) (arguments) => (JSValue) "Hello from C#!";
			// context.GlobalObject["GetMessage"] = (JSObject) &GetMessage;

			void* funcObject = JavaScriptMethods.JSObjectMakeFunctionWithCallback(context.Handle, new JSString("GetMessage").Handle, (delegate* unmanaged[Cdecl]<void*, void*, void*, nuint, void**, void**, void*>) &GetMessage);

			context.GlobalObject["GetMessage"] = new JSObject(context.Handle, funcObject);

			view.UnlockJSContext();

			Console.WriteLine(view.EvaluateScript("GetMessage()", out _));
		}

		[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
		static unsafe void* GetMessage(void* context, void* function, void* thisObject, nuint argumentCount, void** arguments, void** exception)
		{
			JSString str = new("Hello from C#!");

			void* value = JavaScriptMethods.JSValueMakeString(context, str.Handle);

			return value;
		}
	}
}
