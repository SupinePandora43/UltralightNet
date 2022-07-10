// JSBase.h

using System;
using System.Runtime.InteropServices;

namespace UltralightNet.JavaScript;

public static unsafe partial class JavaScriptMethods
{
	public const string LibWebCore = "WebCore";

	internal static void ThrowUnsupportedConstructor() => throw new NotSupportedException("Creation of this type is disallowed.");

	[DllImport(LibWebCore)]
	public static extern JSValueRef JSEvaluateScript(JSContextRef context, JSStringRef script, JSObjectRef thisObject, JSStringRef sourceURL, int startingLineNumber, JSValueRef* exception);
	public static extern JSValueRef JSEvaluateScript(JSContextRef context, JSStringRef script, JSObjectRef thisObject, JSStringRef sourceURL, int startingLineNumber, out JSValueRef exception)
	{
		fixed (JSValueRef* exceptionPtr)
			return JSEvaluateScript(context, script, thisObject, sourceURL, startingLineNumber, exceptionPtr);
	}

	[GeneratedDllImport(LibWebCore)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool JSCheckScriptSyntax(JSContextRef context, JSStringRef script, JSStringRef sourceURL, int startingLineNumber, JSValueRef* exception);
	public static partial bool JSCheckScriptSyntax(JSContextRef context, JSStringRef script, JSStringRef sourceURL, int startingLineNumber, out JSValueRef exception)
	{
		fixed (JSValueRef* exceptionPtr = exception)
			return JSCheckScriptSyntax(context, script, sourceURL, startingLineNumber, exceptionPtr);
	}

	[DllImport(LibWebCore)]
	public static extern void JSGarbageCollect(JSContextRef context);
}
