using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	unsafe partial class JavaScriptMethods
	{
		[DllImport("WebCore")]
		public static extern JSValue JSValueGetType(void* context, void* jsValue);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSValueIsUndefined(void* context, void* jsValue);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSValueIsNull(void* context, void* jsValue);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSValueIsBoolean(void* context, void* jsValue);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSValueIsNumber(void* context, void* jsValue);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSValueIsString(void* context, void* jsValue);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSValueIsSymbol(void* context, void* jsValue);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSValueIsObject(void* context, void* jsValue);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSValueIsObjectOfClass(void* context, void* jsValue, void* jsClass);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSValueIsArray(void* context, void* jsValue);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSValueIsDate(void* context, void* jsValue);

		[DllImport("WebCore")]
		public static extern void* JSValueGetTypedArrayType(void* context, void* jsValue, void** exception);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSValueIsEqual(void* context, void* a, void* b, void** exception);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSValueIsStrictEqual(void* context, void* a, void* b);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSValueIsInstanceOfConstructor(void* context, void* jsValue, void* constructor, void** exception);

		[DllImport("WebCore")]
		public static extern void* JSValueMakeUndefined(void* context);

		[DllImport("WebCore")]
		public static extern void* JSValueMakeNull(void* context);

		[GeneratedDllImport("WebCore")]
		public static partial void* JSValueMakeBoolean(void* context, [MarshalAs(UnmanagedType.I1)] bool boolean);

		[DllImport("WebCore")]
		public static extern void* JSValueMakeNumber(void* context, double number);

		[DllImport("WebCore")]
		public static extern void* JSValueMakeString(void* context, void* jsString);

		[DllImport("WebCore")]
		public static extern void* JSValueMakeSymbol(void* context, void* description);

		[DllImport("WebCore")]
		public static extern void* JSValueMakeFromJSONString(void* context, void* jsString);

		[DllImport("WebCore")]
		public static extern void* JSValueCreateJSONString(void* context, void* jsValue, uint indent, void** exception);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSValueToBoolean(void* context, void* jsValue);

		[DllImport("WebCore")]
		public static extern double JSValueToNumber(void* context, void* jsValue, void** exception);

		[DllImport("WebCore")]
		public static extern void* JSValueToStringCopy(void* context, void* jsValue, void** exception);

		[DllImport("WebCore")]
		public static extern void* JSValueToObject(void* context, void* jsValue, void** exception);

		[DllImport("WebCore")]
		public static extern void JSValueProtect(void* context, void* jsValue);

		[DllImport("WebCore")]
		public static extern void JSValueUnprotect(void* context, void* jsValue);
	}

	public unsafe class JSValue
	{
		private void* handle;
		private void* context;
		private bool isManaged;
		private object managed;

		public JSValue(void* context, void* jsValue)
		{
			this.context = context;
			handle = jsValue;
		}
	}

}
