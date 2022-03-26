// JSBase.h

using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	unsafe partial class JavaScriptMethods
	{
		public const string LibWebCore = "WebCore";
		
		internal static void ThrowUnsupportedConstructor() => throw new NotSupportedException("Creation of this type is disallowed.");

		[DllImport(LibWebCore)]
		public static extern void* JSEvaluateScript(void* context, void* script, void* thisObject, void* sourceURL, int startingLineNumber, void** exception);

		[GeneratedDllImport(LibWebCore)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSCheckScriptSyntax(void* context, void* script, void* sourceURL, int startingLineNumber, void** exception);

		[DllImport(LibWebCore)]
		public static extern void JSGarbageCollect(void* context);
	}
}
