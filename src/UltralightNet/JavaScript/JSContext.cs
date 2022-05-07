using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "<Pending>")]
	public static unsafe partial class JavaScriptMethods
	{
		static JavaScriptMethods() => Methods.Preload();

		[DllImport("WebCore")]
		public static extern void* JSContextGroupCreate();

		[DllImport("WebCore")]
		public static extern void* JSContextGroupRetain(void* contextGroup);

		[DllImport("WebCore")]
		public static extern void JSContextGroupRelease(void* contextGroup);

		[DllImport("WebCore")]
		public static extern void* JSGlobalContextCreate(void* globalObjectClass);

		[DllImport("WebCore")]
		public static extern void* JSGlobalContextCreateInGroup(void* contextGroup, void* globalObjectClass);

		[DllImport("WebCore")]
		public static extern void* JSGlobalContextRetain(void* globalContext);

		[DllImport("WebCore")]
		public static extern void JSGlobalContextRelease(void* globalContext);

		[DllImport("WebCore")]
		public static extern void* JSContextGetGlobalObject(void* context);

		[DllImport("WebCore")]
		public static extern void* JSContextGetGroup(void* context);

		[DllImport("WebCore")]
		public static extern void* JSContextGetGlobalContext(void* context);

		[DllImport("WebCore")]
		public static extern void* JSGlobalContextCopyName(void* globalContext);

		[DllImport("WebCore")]
		public static extern void JSGlobalContextSetName(void* globalContext, void* name);
	}

	public readonly ref struct JSContextN
	{
		public JSContextN() => JavaScriptMethods.ThrowUnsupportedConstructor();
	}

	public unsafe class JSContext : IDisposable
	{
		public JSContext() { }

		internal void* handle;
		internal bool isGlobalContext = false;
		private bool isDisposed = false;
		private bool dispose = true;

		public void* Handle => handle;

		internal void OnLocked(void* actualHandle)
		{
			handle = actualHandle;
		}

		public JSObject GlobalObject => new(Handle, JavaScriptMethods.JSContextGetGlobalObject(Handle));
		public JSContextGroup Group => new(JavaScriptMethods.JSContextGetGroup(Handle));
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
		}
	}

}
