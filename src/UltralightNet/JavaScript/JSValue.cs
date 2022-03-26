using System;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	unsafe partial class JavaScriptMethods
	{
		[DllImport("WebCore")]
		public static extern JSType JSValueGetType(void* context, void* jsValue);

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
		public static extern JSTypedArrayType JSValueGetTypedArrayType(void* context, void* jsValue, void** exception);

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
		protected void* handle = null;
		private void* context = null;
		protected object? managed;

		public JSValue(void* context, void* handle)
		{
			this.context = context;
			this.handle = handle;
		}
		public JSValue(object managed)
		{
			this.managed = managed;
		}
		protected JSValue() { }

		public void* Context
		{
			get => context;
			set
			{
				context = value;
			}
		}

		public void* Handle
		{
			get
			{
				if (handle is null && managed is not null)
				{
					if (context is null) throw new Exception("JSValue.Context is not set.");
					ConvertToNativeJSThing();
				}
				return handle;
			}
			protected set => handle = value;
		}

		virtual protected void ConvertToNativeJSThing()
		{
			if (managed is string str)
			{
				handle = JavaScriptMethods.JSValueMakeString(Context, new JSString(str.AsSpan()).Handle);
			}
		}

		public JSType Type => JavaScriptMethods.JSValueGetType(Context, Handle);
		public bool IsUndefined => JavaScriptMethods.JSValueIsUndefined(Context, Handle);
		public bool IsNull => JavaScriptMethods.JSValueIsNull(Context, Handle);
		public bool IsBoolean => JavaScriptMethods.JSValueIsBoolean(Context, Handle);
		public bool IsNumber => JavaScriptMethods.JSValueIsNumber(Context, Handle);
		public bool IsString => JavaScriptMethods.JSValueIsString(Context, Handle);
		public bool IsSymbol => JavaScriptMethods.JSValueIsSymbol(Context, Handle);
		public bool IsObject => JavaScriptMethods.JSValueIsObject(Context, Handle);
		public bool IsObjectOfClass(void* jsClass) => JavaScriptMethods.JSValueIsObjectOfClass(Context, Handle, jsClass);
		public bool IsArray => JavaScriptMethods.JSValueIsArray(Context, Handle);
		public bool IsDate => JavaScriptMethods.JSValueIsDate(Context, Handle);
		public JSTypedArrayType TypedArrayType
		{
			get
			{
				void* exception;
				var result = JavaScriptMethods.JSValueGetTypedArrayType(Context, Handle, &exception);
				var exceptionOOP = new JSValue(Context, exception);
				if (exceptionOOP.IsString)
				{
					throw new Exception((string)exceptionOOP);
				}
				return result;
			}
		}
		public bool IsEqual(JSValue other)
		{
			void* exception;
			var result = JavaScriptMethods.JSValueIsEqual(Context, Handle, other.Handle, &exception);
			var exceptionOOP = new JSValue(Context, exception);
			if (exceptionOOP.IsString)
			{
				throw new Exception((string)exceptionOOP);
			}
			return result;
		}
		public bool IsStrictEqual(JSValue other) => JavaScriptMethods.JSValueIsStrictEqual(Context, Handle, other.Handle);
		public bool IsInstanceOfConstructor(JSObject constructor)
		{
			void* exception;
			var result = JavaScriptMethods.JSValueIsInstanceOfConstructor(Context, Handle, constructor.Handle, &exception);
			var exceptionOOP = new JSValue(Context, exception);
			if (exceptionOOP.IsString)
			{
				throw new Exception((string)exceptionOOP);
			}
			return result;
		}

		public static explicit operator bool(JSValue jsValue) => JavaScriptMethods.JSValueToBoolean(jsValue.Context, jsValue.Handle);
		public static explicit operator double(JSValue jsValue)
		{
			void* exception;
			var result = JavaScriptMethods.JSValueToNumber(jsValue.Context, jsValue.Handle, &exception);
			var exceptionOOP = new JSValue(jsValue.Context, exception);
			if (exceptionOOP.IsString)
			{
				throw new Exception((string)exceptionOOP);
			}
			return result;
		}
		public static explicit operator string(JSValue jsValue)
		{
			void* exception;
			var result = JavaScriptMethods.JSValueToStringCopy(jsValue.Context, jsValue.Handle, &exception);
			var exceptionOOP = new JSValue(jsValue.Context, exception);
			JSString jsString = new(result, true);
			if (exceptionOOP.IsString)
			{
				jsString.Dispose();
				throw new Exception((string)exceptionOOP);
			}
			string resultActual = jsString.ToString();
			jsString.Dispose();
			return resultActual;
		}
		public JSObject ToJSObject()
		{
			void* exception;
			var result = JavaScriptMethods.JSValueToObject(Context, Handle, &exception);
			var exceptionOOP = new JSValue(Context, exception);
			if (exceptionOOP.IsString)
			{
				throw new Exception((string)exceptionOOP);
			}
			return new JSObject(Context, result);
		}
		public string ToJSON(uint indent = 4)
		{
			void* exception;
			var result = JavaScriptMethods.JSValueCreateJSONString(Context, Handle, indent, &exception);
			var exceptionOOP = new JSValue(Context, exception);
			if (exceptionOOP.IsString)
			{
				throw new Exception((string)exceptionOOP);
			}
			JSString jsString = new(result, true);
			string finalResult = jsString.ToString();
			jsString.Dispose();
			return finalResult;
		}
		public void Protect() => JavaScriptMethods.JSValueProtect(Context, Handle);
		public void Unprotect() => JavaScriptMethods.JSValueUnprotect(Context, Handle);

		public override string ToString() => (string)this;

		public static implicit operator JSValue(string obj) => new(obj);
	}

}
