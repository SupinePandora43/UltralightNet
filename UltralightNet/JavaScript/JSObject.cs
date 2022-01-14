using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{

	unsafe partial class JavaScriptMethods
	{
		// [DllImport("WebCore")]
		// public static extern void* JSClassCreate(JSClassDefinition* jsClassDefinition);

		[DllImport("WebCore")]
		public static extern void* JSClassRetain(void* jsClass);
		
		[DllImport("WebCore")]
		public static extern void JSClassRelease(void* jsClass);

		[DllImport("WebCore")]
		public static extern void* JSClassGetPrivate(void* jsClass);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool JSClassSetPrivate(void* jsClass, void* data);

		[DllImport("WebCore")]
		public static extern void* JSObjectMake(void* context, void* jsClass, void* data);

		[DllImport("WebCore")]
		public static extern void* JSObjectMakeFunctionWithCallback(void* context, void* name, delegate* unmanaged[Cdecl]<void*, void*, void*, nuint, void**, void**, void*> func);

		[DllImport("WebCore")]
		public static extern void* JSObjectMakeConstructor(void* context, void* jsClass, delegate* unmanaged[Cdecl]<void*, void*, nuint, void**, void**, void*> func);

		[DllImport("WebCore")]
		public static extern void* JSObjectMakeArray(void* context, nuint argumentCount, void** arguments, void** exception);

		[DllImport("WebCore")]
		public static extern void* JSObjectMakeDate(void* context, nuint argumentCount, void** arguments, void** exception);

		[DllImport("WebCore")]
		public static extern void* JSObjectMakeError(void* context, nuint argumentCount, void** arguments, void** exception);

		[DllImport("WebCore")]
		public static extern void* JSObjectMakeRegExp(void* context, nuint argumentCount, void** arguments, void** exception);

		[DllImport("WebCore")]
		public static extern void* JSObjectMakeDeferredPromise(void* context, void* resolve, void* reject, void** exception);

		[DllImport("WebCore")]
		public static extern void* JSObjectMakeFunction(void* context, void* name, uint parameterCount, void** parameterNames, void* body, void* sourceURL, int startingLineNumber, void** exception);

		[DllImport("WebCore")]
		public static extern void* JSObjectGetPrototype(void* context, void* jsObject);

		[DllImport("WebCore")]
		public static extern void JSObjectSetPrototype(void* context, void* jsObject, void* jsValue);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool JSObjectHasProperty(void* jsClass, void* jsObject, void* propertyName);

		[DllImport("WebCore")]
		public static extern void* JSObjectGetProperty(void* context, void* jsObject, void* propertyName, void** exception);

		[DllImport("WebCore")]
		public static extern void JSObjectSetProperty(void* context, void* jsObject, void* propertyName, void* value, JSPropertyAttributes attributes = JSPropertyAttributes.None, void** exception = null);
	}

	public unsafe class JSObject
	{
		public JSObject(void* context, void* ptr)
		{
			contextPtr = context;
			handle = ptr;
		}

		private void* handle;
		public void* Handle => handle;

		private void* contextPtr;

		public JSObject this[string key]
		{
			set
			{
				SetProperty(key, value);
			}
		}

		public void SetProperty(JSString name, JSObject obj)
		{
			JavaScriptMethods.JSObjectSetProperty(contextPtr, handle, name.Handle, obj.Handle);
		}

		public void SetProperty(string name, JSObject obj)
		{
			SetProperty(new JSString(name), obj);
		}
	}

}
