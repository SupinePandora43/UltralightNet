// JSTypedArray.h

using System.Runtime.InteropServices;

namespace UltralightNet.JavaScript
{
	namespace Low
	{
		unsafe partial class JavaScriptMethods
		{
			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSObjectMakeTypedArray(JSContextRef ctx, JSTypedArrayType arrayType, nuint length, JSValueRef* exception = null);
			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSObjectMakeTypedArrayWithBytesNoCopy(JSContextRef ctx, JSTypedArrayType arrayType, void* bytes, nuint byteLength, delegate* unmanaged[Cdecl]<void*, void*, void>/*JSTypedArrayBytesDeallocator*/ bytesDeallocator, void* deallocatorContext, JSValueRef* exception = null);
			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSObjectMakeTypedArrayWithArrayBuffer(JSContextRef ctx, JSTypedArrayType arrayType, JSObjectRef buffer, JSValueRef* exception = null);
			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSObjectMakeTypedArrayWithArrayBufferAndOffset(JSContextRef ctx, JSTypedArrayType arrayType, JSObjectRef buffer, nuint byteOffset, nuint length, JSValueRef* exception = null);
			[LibraryImport(LibWebCore)]
			public static partial void* JSObjectGetTypedArrayBytesPtr(JSContextRef ctx, JSObjectRef @object, JSValueRef* exception = null);
			[LibraryImport(LibWebCore)]
			public static partial nuint JSObjectGetTypedArrayLength(JSContextRef ctx, JSObjectRef @object, JSValueRef* exception = null);
			[LibraryImport(LibWebCore)]
			public static partial nuint JSObjectGetTypedArrayByteLength(JSContextRef ctx, JSObjectRef @object, JSValueRef* exception = null);
			[LibraryImport(LibWebCore)]
			public static partial nuint JSObjectGetTypedArrayByteOffset(JSContextRef ctx, JSObjectRef @object, JSValueRef* exception = null);
			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSObjectGetTypedArrayBuffer(JSContextRef ctx, JSObjectRef @object, JSValueRef* exception = null);
			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSObjectMakeArrayBufferWithBytesNoCopy(JSContextRef ctx, void* bytes, nuint byteLength, delegate* unmanaged[Cdecl]<void*, void*, void>/*JSTypedArrayBytesDeallocator*/ bytesDeallocator, void* deallocatorContext, JSValueRef* exception = null);
			[LibraryImport(LibWebCore)]
			public static partial void* JSObjectGetArrayBufferBytesPtr(JSContextRef ctx, JSObjectRef @object, JSValueRef* exception = null);
			[LibraryImport(LibWebCore)]
			public static partial nuint JSObjectGetArrayBufferByteLength(JSContextRef ctx, JSObjectRef @object, JSValueRef* exception = null);
		}
	}
}
