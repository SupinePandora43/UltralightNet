// JSContextRef.h

using System.Runtime.InteropServices;
using UltralightNet.JavaScript.Low;
using UltralightNet.JavaScript.LowStuff;

namespace UltralightNet.JavaScript
{
	namespace Low
	{
		unsafe partial class JavaScriptMethods
		{
			[LibraryImport(LibWebCore)]
			public static partial JSContextGroupRef JSContextGroupCreate();

			[LibraryImport(LibWebCore)]
			public static partial JSContextGroupRef JSContextGroupRetain(JSContextGroupRef contextGroup);

			[LibraryImport(LibWebCore)]
			public static partial void JSContextGroupRelease(JSContextGroupRef contextGroup);

			[LibraryImport(LibWebCore)]
			public static partial JSGlobalContextRef JSGlobalContextCreate(JSClassRef globalObjectClass = default);

			[LibraryImport(LibWebCore)]
			public static partial JSGlobalContextRef JSGlobalContextCreateInGroup(JSContextGroupRef contextGroup = default, JSClassRef globalObjectClass = default);

			[LibraryImport(LibWebCore)]
			public static partial JSGlobalContextRef JSGlobalContextRetain(JSGlobalContextRef globalContext);

			[LibraryImport(LibWebCore)]
			public static partial void JSGlobalContextRelease(JSGlobalContextRef globalContext);

			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSContextGetGlobalObject(JSContextRef context);

			[LibraryImport(LibWebCore)]
			public static partial JSContextGroupRef JSContextGetGroup(JSContextRef context);

			[LibraryImport(LibWebCore)]
			public static partial JSGlobalContextRef JSContextGetGlobalContext(JSContextRef context);

			[LibraryImport(LibWebCore)]
			public static partial JSStringRef JSGlobalContextCopyName(JSGlobalContextRef globalContext);

			[LibraryImport(LibWebCore)]
			public static partial void JSGlobalContextSetName(JSGlobalContextRef globalContext, JSStringRef name);
		}

		public readonly struct JSContextGroupRef
		{
			private readonly nuint _handle;
			public JSContextGroupRef() => JavaScriptMethods.ThrowUnsupportedConstructor();
			public override int GetHashCode() => throw JavaScriptMethods.UnsupportedMethodException;
			public override bool Equals(object? o) => throw JavaScriptMethods.UnsupportedMethodException;

			public static bool operator ==(JSContextGroupRef left, JSContextGroupRef right) => left._handle == right._handle;
			public static bool operator !=(JSContextGroupRef left, JSContextGroupRef right) => left._handle != right._handle;
		}
		public readonly struct JSGlobalContextRef
		{
			private readonly nuint _handle;
			public JSGlobalContextRef() => JavaScriptMethods.ThrowUnsupportedConstructor();
			public override int GetHashCode() => throw JavaScriptMethods.UnsupportedMethodException;
			public override bool Equals(object? o) => throw JavaScriptMethods.UnsupportedMethodException;

			public static bool operator ==(JSGlobalContextRef left, JSGlobalContextRef right) => left._handle == right._handle;
			public static bool operator !=(JSGlobalContextRef left, JSGlobalContextRef right) => left._handle != right._handle;

			public static implicit operator JSContextRef(JSGlobalContextRef globalContextRef) => JavaScriptMethods.BitCast<JSGlobalContextRef, JSContextRef>(globalContextRef);
			public static explicit operator JSGlobalContextRef(JSContextRef contextRef) => JavaScriptMethods.BitCast<JSContextRef, JSGlobalContextRef>(contextRef);
		}
		public readonly struct JSContextRef
		{
			private readonly nuint _handle;
			public JSContextRef() => JavaScriptMethods.ThrowUnsupportedConstructor();
			public override int GetHashCode() => throw JavaScriptMethods.UnsupportedMethodException;
			public override bool Equals(object? o) => throw JavaScriptMethods.UnsupportedMethodException;

			public static bool operator ==(JSContextRef left, JSContextRef right) => left._handle == right._handle;
			public static bool operator !=(JSContextRef left, JSContextRef right) => left._handle != right._handle;
		}
	}

	public unsafe sealed class JSContextGroup : JSNativeContainer<JSContextGroupRef>, ICloneable
	{
		private JSContextGroup() { }

		public JSContextGroup Clone()
		{
			JSContextGroup returnValue = FromHandle(JavaScriptMethods.JSContextGroupRetain(JSHandle), true);
			GC.KeepAlive(this);
			return returnValue;
		}
		object ICloneable.Clone() => Clone();

		public JSGlobalContext CreateGlobalContext(JSNativeContainer<JSClassRef>? globalObject = null)
		{
			var handle = JavaScriptMethods.JSGlobalContextCreateInGroup(JSHandle, globalObject?.JSHandle ?? default);
			GC.KeepAlive(this);
			GC.KeepAlive(globalObject);
			return JSGlobalContext.FromHandle(handle, true);
		}

		public static JSContextGroup Create() => new() { JSHandle = JavaScriptMethods.JSContextGroupCreate() };
		public static JSContextGroup FromHandle(JSContextGroupRef handle, bool dispose) => new() { JSHandle = handle, Owns = dispose };

		public override void Dispose()
		{
			if (!IsDisposed && Owns) JavaScriptMethods.JSContextGroupRelease(JSHandle);
			base.Dispose();
		}
	}

	public unsafe class JSContext : JSNativeContainer<JSContextRef>
	{
		internal protected JSContext() { }

		/*internal void OnLocked(void* actualHandle)
		{
			handle = actualHandle;
		}

		public JSObject GlobalObject => new(Handle, JavaScriptMethods.JSContextGetGlobalObject(Handle));
		public JSContextGroup Group => JSContextGroup.CreateFromPointer(JavaScriptMethods.JSContextGetGroup(Handle));
		public JSContext GlobalContext => new() { isGlobalContext = true, handle = JavaScriptMethods.JSContextGetGlobalContext(Handle) };

		public JSString Name
		{
			get
			{
				if (!isGlobalContext) throw new Exception("context isn't GlobalContext");
				return new(JavaScriptMethods.JSGlobalContextCopyName(Handle), true);
			}
			set
			{
				if (!isGlobalContext) throw new Exception("context isn't GlobalContext");
				JavaScriptMethods.JSGlobalContextSetName(Handle, value.Handle);
			}
		}

		public JSValue EvaluateScript(JSString script, JSObject? thisObject, JSString? sourceURL, int startingLineNumber, out JSValue exception)
		{
			void* exceptionPointer;
			var resultNative = JavaScriptMethods.JSEvaluateScript(Handle, script.Handle, thisObject is null ? null : thisObject.Handle, sourceURL is null ? null : sourceURL.Handle, startingLineNumber, &exceptionPointer);
			exception = new JSValue(Handle, exceptionPointer);
			return new(Handle, resultNative);
		}
		public JSValue EvaluateScript(JSString script, JSObject? thisObject = null, JSString? sourceURL = null, int startingLineNumber = 0) => EvaluateScript(script, thisObject, sourceURL, startingLineNumber, out _);
		public bool CheckScriptSyntax(JSString script, JSString? sourceURL, int startingLineNumber, out JSValue exception)
		{
			void* exceptionPointer = null;
			var result = JavaScriptMethods.JSCheckScriptSyntax(Handle, script.Handle, sourceURL is null ? null : sourceURL.Handle, startingLineNumber, &exceptionPointer);
			exception = new(Handle, exceptionPointer);
			return result;
		}
		public bool CheckScriptSyntax(JSString script, JSString? sourceURL = null, int startingLineNumber = 0) => CheckScriptSyntax(script, sourceURL, startingLineNumber, out _);
		public void GarbageCollect() => JavaScriptMethods.JSGarbageCollect(Handle);

		// INTEROPTODO: TEST
		public static JSContext CreateGlobalContext(JSObject jsGlobalObject) => new() { isGlobalContext = true, handle = JavaScriptMethods.JSGlobalContextCreate(jsGlobalObject.Handle) };

		public static JSContext Retain(JSContext context)
		{
			if (!context.isGlobalContext) throw new ArgumentException("context isn't a GlobalContext", nameof(context));
			return new() { isGlobalContext = true, handle = JavaScriptMethods.JSGlobalContextRetain(context.Handle) };
		}

		public JSValue MakeUndefined() => new(Handle, JavaScriptMethods.JSValueMakeUndefined(Handle));
		public JSValue MakeNull() => new(Handle, JavaScriptMethods.JSValueMakeNull(Handle));
		public JSValue MakeBoolean(bool boolean) => new(Handle, JavaScriptMethods.JSValueMakeBoolean(Handle, boolean));
		public JSValue MakeNumber(double number) => new(Handle, JavaScriptMethods.JSValueMakeNumber(Handle, number));
		public JSValue MakeString(string str) => new(Handle, JavaScriptMethods.JSValueMakeString(Handle, new JSString(str.AsSpan()).Handle));
		public JSValue MakeSymbol(char chr) => new(Handle, JavaScriptMethods.JSValueMakeSymbol(Handle, new JSString(chr.ToString().AsSpan()).Handle));
		public JSValue MakeFromJSON(string json) => new(Handle, JavaScriptMethods.JSValueMakeFromJSONString(Handle, new JSString(json.AsSpan()).Handle));

		~JSContext() => Dispose();

		public void Dispose()
		{
			if (!isDisposed)
			{
				if (isGlobalContext && dispose)
				{
					JavaScriptMethods.JSGlobalContextRelease(handle);
				}
				isDisposed = true;
				GC.SuppressFinalize(this);
			}
		}*/

		public JSObjectRef GlobalObject => JavaScriptMethods.JSContextGetGlobalObject(JSHandle); // TODO retain it.
		public JSContextGroup Group
		{
			get
			{
				var returnValue = JSContextGroup.FromHandle(JavaScriptMethods.JSContextGroupRetain(JavaScriptMethods.JSContextGetGroup(JSHandle)), true);
				GC.KeepAlive(this);
				return returnValue;
			}
		}
		public JSGlobalContext GlobalContext
		{
			get
			{
				var returnValue = JSGlobalContext.FromHandle(JavaScriptMethods.JSGlobalContextRetain(JavaScriptMethods.JSContextGetGlobalContext(JSHandle)), true);
				GC.KeepAlive(this);
				return returnValue;
			}
		}

		public static JSContext FromHandle(JSContextRef handle, bool dispose) => new() { JSHandle = handle, Owns = dispose };
	}
	public unsafe sealed class JSGlobalContext : JSContext
	{
		private JSGlobalContext() { }

		public new JSGlobalContextRef JSHandle
		{
			get => (JSGlobalContextRef)base.JSHandle;
			private init => base.JSHandle = value;
		}

		public JSString Name
		{
			get
			{
				var returnValue = JSString.FromHandle(JavaScriptMethods.JSGlobalContextCopyName(JSHandle), true);
				GC.KeepAlive(this);
				return returnValue;
			}
			set
			{
				JavaScriptMethods.JSGlobalContextSetName(JSHandle, value.JSHandle);
				GC.KeepAlive(this);
				GC.KeepAlive(value);
			}
		}

		public static JSGlobalContext FromHandle(JSGlobalContextRef handle, bool dispose) => new() { JSHandle = handle, Owns = dispose };

		public override void Dispose()
		{
			if (!IsDisposed && Owns) JavaScriptMethods.JSGlobalContextRelease(JSHandle);
			base.Dispose();
		}
	}
}
