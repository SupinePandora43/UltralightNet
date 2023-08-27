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
			public static partial bool JSObjectSetPrivateProperty(JSContextRef ctx, JSObjectRef jsObject, JSStringRef propertyName, JSValueRef value);

			[LibraryImport(LibWebCore)]
			public static partial JSValueRef JSObjectGetPrivateProperty(JSContextRef ctx, JSObjectRef jsObject, JSStringRef propertyName);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSObjectDeletePrivateProperty(JSContextRef ctx, JSObjectRef jsObject, JSStringRef propertyName);

			/// <summary>
			/// TODO: may not work
			/// </summary>
			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSObjectGetProxyTarget(JSObjectRef jsObject);

			[LibraryImport(LibWebCore)]
			public static partial JSGlobalContextRef JSObjectGetGlobalContext(JSObjectRef jsObject);
		}
		unsafe partial class Crazy
		{
			public static bool TrySetPrivateProperty(this (JSContextRef ctx, JSObjectRef jsObject) pair, JSString propertyName, JSValueRef value){
				var returnValue = 	JavaScriptMethods.JSObjectSetPrivateProperty(pair.ctx, pair.jsObject, propertyName.JSHandle, value);
				GC.KeepAlive(propertyName);
				return returnValue;
			}
			public static JSValueRef GetPrivateProperty(this (JSContextRef ctx, JSObjectRef jsObject) pair, JSString propertyName){
				var returnValue = 	JavaScriptMethods.JSObjectGetPrivateProperty(pair.ctx, pair.jsObject, propertyName.JSHandle);
				GC.KeepAlive(propertyName);
				return returnValue;
			}
			public static bool TryDeletePrivateProperty(this (JSContextRef ctx, JSObjectRef jsObject) pair, JSString propertyName){
				var returnValue = 	JavaScriptMethods.JSObjectDeletePrivateProperty(pair.ctx, pair.jsObject, propertyName.JSHandle);
				GC.KeepAlive(propertyName);
				return returnValue;
			}
		}
	}
}
