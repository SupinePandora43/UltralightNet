// JSBase.h

using System.Runtime.InteropServices;

namespace UltralightNet
{
	unsafe partial class JavaScriptMethods
	{
		[DllImport("WebCore")]
		public static extern void* JSEvaluateScript(void* context, void* script, void* thisObject, void* sourceURL, int startingLineNumber, void** exception);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSCheckScriptSyntax(void* context, void* script, void* sourceURL, int startingLineNumber, void** exception);

		[DllImport("WebCore")]
		public static extern void JSGarbageCollect(void* context);
	}
}
