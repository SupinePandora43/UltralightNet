using System;
using System.Runtime.InteropServices;

namespace UltralightNet.JavaScript;

unsafe partial class JavaScriptMethods
{
	[DllImport("WebCore")]
	public static extern void* JSClassCreate(JSClassDefinition* jsClassDefinition);

	[DllImport("WebCore")]
	public static extern void* JSClassRetain(void* jsClass);

	[DllImport("WebCore")]
	public static extern void JSClassRelease(void* jsClass);

	[DllImport("WebCore")]
	public static extern void* JSClassGetPrivate(void* jsClass);

	[GeneratedDllImport("WebCore")]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool JSClassSetPrivate(void* jsClass, void* data);

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
	public static extern void* JSObjectMakeDeferredPromise(void* context, void** resolve, void** reject, void** exception);

	[DllImport("WebCore")]
	public static extern void* JSObjectMakeFunction(void* context, void* name, uint parameterCount, void** parameterNames, void* body, void* sourceURL, int startingLineNumber, void** exception);

	[DllImport("WebCore")]
	public static extern void* JSObjectGetPrototype(void* context, void* jsObject);

	[DllImport("WebCore")]
	public static extern void JSObjectSetPrototype(void* context, void* jsObject, void* jsValue);

	[GeneratedDllImport("WebCore")]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool JSObjectHasProperty(void* jsClass, void* jsObject, void* propertyName);

	[DllImport("WebCore")]
	public static extern void* JSObjectGetProperty(void* context, void* jsObject, void* propertyName, void** exception);

	[DllImport("WebCore")]
	public static extern void JSObjectSetProperty(void* context, void* jsObject, void* propertyName, void* value, JSPropertyAttributes attributes = JSPropertyAttributes.None, void** exception = null);

	[GeneratedDllImport("WebCore")]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool JSObjectDeleteProperty(void* context, void* jsObject, void* propertyName, void** exception);

	[GeneratedDllImport("WebCore")]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool JSObjectHasPropertyForKey(void* context, void* jsObject, void* propertyKey, void** exception);

	[DllImport("WebCore")]
	public static extern void* JSObjectGetPropertyForKey(void* context, void* jsObject, void* propertyKey, void** exception);

	[DllImport("WebCore")]
	public static extern void JSObjectSetPropertyForKey(void* context, void* jsObject, void* propertyKey, void* value, JSPropertyAttributes attributes = JSPropertyAttributes.None, void** exception = null);

	[GeneratedDllImport("WebCore")]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool JSObjectDeletePropertyForKey(void* context, void* jsObject, void* propertyKey, void** exception);

	[DllImport("WebCore")]
	public static extern void* JSObjectGetPropertyAtIndex(void* context, void* jsObject, uint propertyIndex, void** exception);

	[DllImport("WebCore")]
	public static extern void JSObjectSetPropertyAtIndex(void* context, void* jsObject, uint propertyIndex, void* value, void** exception = null);

	[DllImport("WebCore")]
	public static extern void* JSObjectGetPrivate(void* jsObject);

	[GeneratedDllImport("WebCore")]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool JSObjectSetPrivate(void* jsObject, void* data);

	[GeneratedDllImport("WebCore")]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool JSObjectIsFunction(void* context, void* jsObject);

	[DllImport("WebCore")]
	public static extern void* JSObjectCallAsFunction(void* context, void* jsObject, void* thisObject, nuint argumentCount, void** arguments, void** exception);

	[GeneratedDllImport("WebCore")]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool JSObjectIsConstructor(void* context, void* jsObject);

	[DllImport("WebCore")]
	public static extern void* JSObjectCallAsConstructor(void* context, void* jsObject, nuint argumentCount, void** arguments, void** exception);

	[DllImport("WebCore")]
	public static extern JSPropertyNameArray* JSObjectCopyPropertyNames(void* context, void* jsObject);

	[DllImport("WebCore")]
	public static extern JSPropertyNameArray* JSPropertyNameArrayRetain(JSPropertyNameArray* array);

	[DllImport("WebCore")]
	public static extern void JSPropertyNameArrayRelease(JSPropertyNameArray* array);

	[DllImport("WebCore")]
	public static extern nuint JSPropertyNameArrayGetCount(JSPropertyNameArray* array);

	[DllImport("WebCore")]
	public static extern void* JSPropertyNameArrayGetNameAtIndex(JSPropertyNameArray* array, nuint index);

	[DllImport("WebCore")]
	public static extern void JSPropertyNameAccumulatorAddName(JSPropertyNameAccumulatorRef accumulator, void* propertyName);
}
public readonly ref struct JSObjectN
{
	public JSObjectN() => JavaScriptMethods.ThrowUnsupportedConstructor();
}
public unsafe class JSObject : JSValue
{
	public JSObject(void* context, void* handle) : base(context, handle) { }
	public JSObject(object managed) : base(managed) { }

	override protected void ConvertToNativeJSThing()
	{
		base.ConvertToNativeJSThing();
		if (handle is not null) return;
		if (managed is null) handle = JavaScriptMethods.JSValueMakeNull(Context);
		else if (managed is nint funcPtr)
		{
			handle = JavaScriptMethods.JSObjectMakeFunctionWithCallback(Context, ((JSString)"c#_js_stub").Handle, (delegate* unmanaged[Cdecl]<void*, void*, void*, nuint, void**, void**, void*>)funcPtr);
		}
	}

	public JSObject this[JSString key]
	{
		set
		{
			SetProperty(key, value);
		}
		get
		{
			void* exception;
			JSObject obj = new(Context, JavaScriptMethods.JSObjectGetProperty(Context, Handle, key.Handle, &exception));
			JSValue exceptionOOP = new(Context, exception);
			if (exceptionOOP.IsString) throw new Exception(((string)exceptionOOP));
			return obj;
		}
	}

	public void SetProperty(JSString name, JSObject obj)
	{
		if (obj.Context is null && Context is null) throw new NotImplementedException("Not supported yet.");
		if (obj.Context is null && Context is not null) obj.Context = Context;
		void* exception;
		JavaScriptMethods.JSObjectSetProperty(Context, Handle, name.Handle, obj.Handle, JSPropertyAttributes.None, &exception);
		var exceptionOOP = new JSValue(Context, Handle);
		if (exceptionOOP.IsString) throw new((string)exceptionOOP);
	}

	public static implicit operator JSObject(delegate* unmanaged[Cdecl]<void*, void*, void*, nuint, void**, void**, void*> func)
	{
		return new((object)(nint)func);
	}
}
