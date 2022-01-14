// JSObjectRefPrivate.h

using System.Runtime.InteropServices;

namespace UltralightNet
{
	unsafe partial class JavaScriptMethods
	{
		[GeneratedDllImport("WebCore")]
		public static partial bool JSObjectSetPrivateProperty(void* context, void* jsObject, void* propertyName, void* value);

		[DllImport("WebCore")]
		public static extern void* JSObjectGetPrivateProperty(void* context, void* jsObject, void* propertyName);

		[GeneratedDllImport("WebCore")]
		public static partial bool JSObjectDeletePrivateProperty(void* context, void* jsObject, void* propertyName);

		/// <summary>
		/// TODO: may not work
		/// </summary>
		[DllImport("WebCore")]
		public static extern void* JSObjectGetProxyTarget();

		[DllImport("WebCore")]
		public static extern void* JSObjectGetGlobalContext(void* jsObject);
	}
}
