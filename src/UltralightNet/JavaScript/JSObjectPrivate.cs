// JSObjectRefPrivate.h

using System.Runtime.InteropServices;

namespace UltralightNet.JavaScript
{
	namespace Low
	{
		unsafe partial class JavaScriptMethods
		{
			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSObjectSetPrivateProperty(JSContextRef ctx, JSObjectRef @object, JSStringRef propertyName, JSValueRef value);

			[LibraryImport(LibWebCore)]
			public static partial JSValueRef JSObjectGetPrivateProperty(JSContextRef ctx, JSObjectRef @object, JSStringRef propertyName);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSObjectDeletePrivateProperty(JSContextRef ctx, JSObjectRef @object, JSStringRef propertyName);

			/// <summary>
			/// TODO: may not work
			/// </summary>
			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSObjectGetProxyTarget(JSObjectRef @object);

			[LibraryImport(LibWebCore)]
			public static partial JSGlobalContextRef JSObjectGetGlobalContext(JSObjectRef @object);
		}
	}
}
