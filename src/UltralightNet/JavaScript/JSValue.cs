// JSValueRef.h

using System.Runtime.InteropServices;
using UltralightNet.JavaScript.LowLevel;

namespace UltralightNet.JavaScript
{
	namespace Low
	{
		unsafe partial class JavaScriptMethods
		{
			[LibraryImport(LibWebCore)]
			public static partial JSType JSValueGetType(JSContextRef context, JSValueRef jsValue);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSValueIsUndefined(JSContextRef context, JSValueRef jsValue);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSValueIsNull(JSContextRef context, JSValueRef jsValue);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSValueIsBoolean(JSContextRef context, JSValueRef jsValue);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSValueIsNumber(JSContextRef context, JSValueRef jsValue);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSValueIsString(JSContextRef context, JSValueRef jsValue);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSValueIsSymbol(JSContextRef context, JSValueRef jsValue);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSValueIsObject(JSContextRef context, JSValueRef jsValue);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSValueIsObjectOfClass(JSContextRef context, JSValueRef jsValue, JSClassRef jsClass);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSValueIsArray(JSContextRef context, JSValueRef jsValue);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSValueIsDate(JSContextRef context, JSValueRef jsValue);

			[LibraryImport(LibWebCore)]
			public static partial JSTypedArrayType JSValueGetTypedArrayType(JSContextRef context, JSValueRef jsValue, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSValueIsEqual(JSContextRef context, JSValueRef a, JSValueRef b, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSValueIsStrictEqual(JSContextRef context, JSValueRef a, JSValueRef b);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSValueIsInstanceOfConstructor(JSContextRef context, JSValueRef jsValue, JSObjectRef constructor, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			public static partial JSValueRef JSValueMakeUndefined(JSContextRef context);

			[LibraryImport(LibWebCore)]
			public static partial JSValueRef JSValueMakeNull(JSContextRef context);

			[LibraryImport(LibWebCore)]
			public static partial JSValueRef JSValueMakeBoolean(JSContextRef context, [MarshalAs(UnmanagedType.U1)] bool boolean);

			[LibraryImport(LibWebCore)]
			public static partial JSValueRef JSValueMakeNumber(JSContextRef context, double number);

			[LibraryImport(LibWebCore)]
			public static partial JSValueRef JSValueMakeString(JSContextRef context, JSStringRef jsString);

			[LibraryImport(LibWebCore)]
			public static partial JSValueRef JSValueMakeSymbol(JSContextRef context, JSStringRef description);

			[LibraryImport(LibWebCore)]
			public static partial JSValueRef JSValueMakeFromJSONString(JSContextRef context, JSStringRef jsString);

			[LibraryImport(LibWebCore)]
			public static partial JSStringRef JSValueCreateJSONString(JSContextRef context, JSValueRef jsValue, uint indent, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSValueToBoolean(JSContextRef context, JSValueRef jsValue);

			[LibraryImport(LibWebCore)]
			public static partial double JSValueToNumber(JSContextRef context, JSValueRef jsValue, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			public static partial JSStringRef JSValueToStringCopy(JSContextRef context, JSValueRef jsValue, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			public static partial JSObjectRef JSValueToObject(JSContextRef context, JSValueRef jsValue, JSValueRef* exception = null);

			[LibraryImport(LibWebCore)]
			public static partial void JSValueProtect(JSContextRef context, JSValueRef jsValue);

			[LibraryImport(LibWebCore)]
			public static partial void JSValueUnprotect(JSContextRef context, JSValueRef jsValue);
		}

		public readonly struct JSValueRef : IValueRef
		{
			private readonly nuint _handle;
			public JSValueRef() => JavaScriptMethods.ThrowUnsupportedConstructor();
			public override int GetHashCode() => throw JavaScriptMethods.UnsupportedMethodException;
			public override bool Equals(object? o) => throw JavaScriptMethods.UnsupportedMethodException;

			public static bool operator ==(JSValueRef left, JSValueRef right) => left._handle == right._handle;
			public static bool operator !=(JSValueRef left, JSValueRef right) => left._handle != right._handle;
		}

		unsafe partial class Crazy
		{
			public static JSString? ToString(this (JSContextRef ctx, JSValueRef jsValue) pair)
			{
				var handle = JavaScriptMethods.JSValueToStringCopy(pair.ctx, pair.jsValue);
				return handle != default ? JSString.FromHandle(handle, true) : null;
			}
		}
	}
	namespace LowLevel
	{
		public interface IValueRef
		{

		}
	}
	public enum JSType : int
	{
		Undefined,
		Null,
		Boolean,
		Number,
		String,
		Object,
		Symbol
	}
	public enum JSTypedArrayType : int
	{
		Int8Array,
		Int16Array,
		Int32Array,
		Uint8Array,
		Uint8ClampedArray,
		Uint16Array,
		Uint32Array,
		Float32Array,
		Float64Array,
		ArrayBuffer,
		None
	}
	/*

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
	}*/
}
