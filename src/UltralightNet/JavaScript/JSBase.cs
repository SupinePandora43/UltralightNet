// JSBase.h

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet.JavaScript
{
	namespace Low
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Those are 1:1 definitions")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1401:P/Invokes should not be visible", Justification = "Compatibility")]
		public static unsafe partial class JavaScriptMethods
		{
			static JavaScriptMethods() => Methods.Preload();

			const string LibWebCore = "WebCore";

			internal static void ThrowUnsupportedConstructor() => throw new NotSupportedException("Constructor is unsupported");
			internal static Exception UnsupportedMethodException => new NotSupportedException("Method is unsupported");

			// backported from net8.0 for compatibility
			internal static TTo BitCast<TFrom, TTo>(TFrom from) where TFrom : unmanaged where TTo : unmanaged => Unsafe.As<TFrom, TTo>(ref Unsafe.AsRef(from));

			[LibraryImport(LibWebCore)]
			public static partial JSValueRef JSEvaluateScript(JSContextRef context, JSStringRef script, JSObjectRef thisObject, JSStringRef sourceURL, int startingLineNumber, JSValueRef* exception);
			public static JSValueRef JSEvaluateScript(JSContextRef context, JSStringRef script, JSObjectRef thisObject, JSStringRef sourceURL, int startingLineNumber, out JSValueRef exception)
			{
				fixed (JSValueRef* exceptionPtr = &exception)
					return JSEvaluateScript(context, script, thisObject, sourceURL, startingLineNumber, exceptionPtr);
			}

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSCheckScriptSyntax(JSContextRef context, JSStringRef script, JSStringRef sourceURL, int startingLineNumber, JSValueRef* exception);
			public static bool JSCheckScriptSyntax(JSContextRef context, JSStringRef script, JSStringRef sourceURL, int startingLineNumber, out JSValueRef exception)
			{
				fixed (JSValueRef* exceptionPtr = &exception)
					return JSCheckScriptSyntax(context, script, sourceURL, startingLineNumber, exceptionPtr);
			}

			[LibraryImport(LibWebCore)]
			public static partial void JSGarbageCollect(JSContextRef context);
		}
	}
}
