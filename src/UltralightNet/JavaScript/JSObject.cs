// JSObjectRef.h

using System.Runtime.InteropServices;

namespace UltralightNet.JavaScript
{
	namespace Low
	{
		unsafe partial class JavaScriptMethods
		{
			[LibraryImport(LibWebCore)]
			public static partial JSClassRef JSClassCreate(in JSClassDefinition jsClassDefinition);

			[LibraryImport(LibWebCore)]
			public static partial JSClassRef JSClassRetain(JSClassRef jsClass);

			[LibraryImport(LibWebCore)]
			public static partial void JSClassRelease(JSClassRef jsClass);

			[LibraryImport(LibWebCore)]
			public static partial nint JSClassGetPrivate(JSClassRef jsClass);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSClassSetPrivate(JSClassRef jsClass, nint data);

			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSObjectMake(JSContextRef ctx, JSClassRef jsClass, nint privateData);

			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSObjectMakeFunctionWithCallback(JSContextRef ctx, JSStringRef name, delegate* unmanaged[Cdecl]<JSContextRef /*ctx*/, JSObjectRef /*function*/, JSObjectRef /*thisObject*/, nuint /*argumentCount*/, JSValueRef* /*arguments[]*/, JSValueRef* /*exception*/, JSValueRef> func);

			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSObjectMakeConstructor(JSContextRef ctx, JSClassRef jsClass, delegate* unmanaged[Cdecl]<JSContextRef /*ctx*/, JSObjectRef /*constructor*/, nuint /*argumentCount*/, JSValueRef* /*arguments[]*/, JSValueRef* /*exception*/, JSObjectRef> func);

			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSObjectMakeArray(JSContextRef ctx, nuint argumentCount, JSValueRef* arguments, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSObjectMakeDate(JSContextRef ctx, nuint argumentCount, JSValueRef* arguments, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSObjectMakeError(JSContextRef ctx, nuint argumentCount, JSValueRef* arguments, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSObjectMakeRegExp(JSContextRef ctx, nuint argumentCount, JSValueRef* arguments, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSObjectMakeDeferredPromise(JSContextRef ctx, JSObjectRef* resolve, JSObjectRef* reject, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSObjectMakeFunction(JSContextRef ctx, JSStringRef name, uint parameterCount, JSStringRef* parameterNames, JSStringRef body, JSStringRef sourceURL, int startingLineNumber, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			public static partial JSValueRef JSObjectGetPrototype(JSContextRef ctx, JSObjectRef jsObject);

			[LibraryImport(LibWebCore)]
			public static partial void JSObjectSetPrototype(JSContextRef ctx, JSObjectRef jsObject, JSValueRef jsValue);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSObjectHasProperty(JSClassRef jsClass, JSObjectRef jsObject, JSStringRef propertyName);

			[LibraryImport(LibWebCore)]
			public static partial JSValueRef JSObjectGetProperty(JSContextRef ctx, JSObjectRef jsObject, JSStringRef propertyName, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			public static partial void JSObjectSetProperty(JSContextRef ctx, JSObjectRef jsObject, JSStringRef propertyName, JSValueRef value, JSPropertyAttributes attributes = JSPropertyAttributes.None, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSObjectDeleteProperty(JSContextRef ctx, JSObjectRef jsObject, JSStringRef propertyName, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSObjectHasPropertyForKey(JSContextRef ctx, JSObjectRef jsObject, JSValueRef propertyKey, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			public static partial JSValueRef JSObjectGetPropertyForKey(JSContextRef ctx, JSObjectRef jsObject, JSValueRef propertyKey, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			public static partial void JSObjectSetPropertyForKey(JSContextRef ctx, JSObjectRef jsObject, JSValueRef propertyKey, JSValueRef value, JSPropertyAttributes attributes = JSPropertyAttributes.None, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSObjectDeletePropertyForKey(JSContextRef ctx, JSObjectRef jsObject, JSValueRef propertyKey, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			public static partial JSValueRef JSObjectGetPropertyAtIndex(JSContextRef ctx, JSObjectRef jsObject, uint propertyIndex, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			public static partial void JSObjectSetPropertyAtIndex(JSContextRef ctx, JSObjectRef jsObject, uint propertyIndex, JSValueRef value, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			public static partial nint JSObjectGetPrivate(JSObjectRef jsObject);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSObjectSetPrivate(JSObjectRef jsObject, nint data);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSObjectIsFunction(JSContextRef ctx, JSObjectRef jsObject);

			[LibraryImport(LibWebCore)]
			public static partial JSValueRef JSObjectCallAsFunction(JSContextRef ctx, JSObjectRef jsObject, JSObjectRef thisObject, nuint argumentCount, JSValueRef* arguments, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSObjectIsConstructor(JSContextRef ctx, JSObjectRef jsObject);

			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSObjectCallAsConstructor(JSContextRef ctx, JSObjectRef jsObject, nuint argumentCount, JSValueRef* arguments, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			public static partial JSPropertyNameArrayRef JSObjectCopyPropertyNames(JSContextRef ctx, JSObjectRef jsObject);

			[LibraryImport(LibWebCore)]
			public static partial JSPropertyNameArrayRef JSPropertyNameArrayRetain(JSPropertyNameArrayRef array);

			[LibraryImport(LibWebCore)]
			public static partial void JSPropertyNameArrayRelease(JSPropertyNameArrayRef array);

			[LibraryImport(LibWebCore)]
			public static partial nuint JSPropertyNameArrayGetCount(JSPropertyNameArrayRef array);

			[LibraryImport(LibWebCore)]
			public static partial JSStringRef JSPropertyNameArrayGetNameAtIndex(JSPropertyNameArrayRef array, nuint index);

			[LibraryImport(LibWebCore)]
			public static partial void JSPropertyNameAccumulatorAddName(JSPropertyNameAccumulatorRef accumulator, JSStringRef propertyName);
		}
		public readonly struct JSObjectRef
		{
			private readonly nuint _handle;
			public JSObjectRef() => JavaScriptMethods.ThrowUnsupportedConstructor();
			public override int GetHashCode() => throw JavaScriptMethods.UnsupportedMethodException;
			public override bool Equals(object? o) => throw JavaScriptMethods.UnsupportedMethodException;

			public static bool operator ==(JSObjectRef left, JSObjectRef right) => left._handle == right._handle;
			public static bool operator !=(JSObjectRef left, JSObjectRef right) => left._handle != right._handle;

			public static implicit operator JSValueRef(JSObjectRef @object) => JavaScriptMethods.BitCast<JSObjectRef, JSValueRef>(@object);
		}
		public readonly struct JSClassRef
		{
			private readonly nuint _handle;
			public JSClassRef() => JavaScriptMethods.ThrowUnsupportedConstructor();
			public override int GetHashCode() => throw JavaScriptMethods.UnsupportedMethodException;
			public override bool Equals(object? o) => throw JavaScriptMethods.UnsupportedMethodException;

			public static bool operator ==(JSClassRef left, JSClassRef right) => left._handle == right._handle;
			public static bool operator !=(JSClassRef left, JSClassRef right) => left._handle != right._handle;
		}
		public readonly struct JSPropertyNameArrayRef
		{
			private readonly nuint _handle;
			public JSPropertyNameArrayRef() => JavaScriptMethods.ThrowUnsupportedConstructor();
			public override int GetHashCode() => throw JavaScriptMethods.UnsupportedMethodException;
			public override bool Equals(object? o) => throw JavaScriptMethods.UnsupportedMethodException;

			public static bool operator ==(JSPropertyNameArrayRef left, JSPropertyNameArrayRef right) => left._handle == right._handle;
			public static bool operator !=(JSPropertyNameArrayRef left, JSPropertyNameArrayRef right) => left._handle != right._handle;
		}
		public readonly struct JSPropertyNameAccumulatorRef
		{
			private readonly nuint _handle;
			public JSPropertyNameAccumulatorRef() => JavaScriptMethods.ThrowUnsupportedConstructor();
			public override int GetHashCode() => throw JavaScriptMethods.UnsupportedMethodException;
			public override bool Equals(object? o) => throw JavaScriptMethods.UnsupportedMethodException;

			public static bool operator ==(JSPropertyNameAccumulatorRef left, JSPropertyNameAccumulatorRef right) => left._handle == right._handle;
			public static bool operator !=(JSPropertyNameAccumulatorRef left, JSPropertyNameAccumulatorRef right) => left._handle != right._handle;
		}

		[Flags]
		public enum JSPropertyAttributes : uint
		{
			None = 0,
			ReadOnly = 1 << 1,
			DontEnum = 1 << 2,
			DontDelete = 1 << 3
		}
		[Flags]
		public enum JSClassAttributes : uint
		{
			None = 0,
			NoAutomaticPrototype = 1 << 1
		}
	}
	/*
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
	}*/
}
