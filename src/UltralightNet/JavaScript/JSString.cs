// JSStringRef.h

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UltralightNet.JavaScript.Low;
using UltralightNet.JavaScript.LowStuff;
using UltralightNet.LowStuff;

namespace UltralightNet.JavaScript
{
	namespace Low
	{
		public unsafe partial class JavaScriptMethods
		{
			private const string LibWebCore = "WebCore";

			[LibraryImport(LibWebCore)]
			public static partial JSStringRef JSStringCreateWithCharacters(char* characters, nuint length);

			[LibraryImport(LibWebCore)]
			public static partial JSStringRef JSStringCreateWithUTF8CString(byte* characters);

			/// <summary>Increases ref count</summary>
			[LibraryImport(LibWebCore)]
			public static partial JSStringRef JSStringRetain(JSStringRef @string);

			/// <summary>Decreases ref count</summary>
			[LibraryImport(LibWebCore)]
			public static partial void JSStringRelease(JSStringRef @string);

			[LibraryImport(LibWebCore)]
			public static partial nuint JSStringGetLength(JSStringRef @string);

			[LibraryImport(LibWebCore)]
			public static partial ushort* JSStringGetCharactersPtr(JSStringRef @string);

			[LibraryImport(LibWebCore)]
			public static partial nuint JSStringGetMaximumUTF8CStringSize(JSStringRef @string);

			[LibraryImport(LibWebCore)]
			public static partial nuint JSStringGetUTF8CString(JSStringRef @string, byte* buffer, nuint bufferSize);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.I1)]
			public static partial bool JSStringIsEqual(JSStringRef a, JSStringRef b);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.I1)]
			public static partial bool JSStringIsEqualToUTF8CString(JSStringRef str, byte* characters);
		}

		public readonly struct JSStringRef
		{
			private readonly nuint _handle;
			public JSStringRef() => throw new NotSupportedException();
			public override int GetHashCode() => throw new NotSupportedException();
			public override bool Equals(object? o) => throw new NotSupportedException();

			public static bool operator ==(JSStringRef left, JSStringRef right) => left._handle == right._handle;
			public static bool operator !=(JSStringRef left, JSStringRef right) => left._handle == right._handle;
		}
	}
	namespace LowStuff
	{
		public abstract unsafe class JSNativeContainer<NativeHandle> : NativeContainer where NativeHandle : unmanaged
		{
			public NativeHandle JSHandle
			{
				get => Unsafe.As<nuint, NativeHandle>(ref Unsafe.AsRef((nuint)Handle));
				protected init => Handle = (void*)Unsafe.As<NativeHandle, nuint>(ref Unsafe.AsRef(value));
			}
		}
	}

	[DebuggerDisplay("{ToString(),raw}")]
	public unsafe sealed class JSString : JSNativeContainer<JSStringRef>, IEquatable<JSString>, ICloneable
	{
		private JSString() { }

		public JSString(char* characters, nuint length) => JSHandle = JavaScriptMethods.JSStringCreateWithCharacters(characters, length);
		public JSString(byte* utf8Bytes) => JSHandle = JavaScriptMethods.JSStringCreateWithUTF8CString(utf8Bytes);
		public JSString(ReadOnlySpan<char> chars)
		{
			fixed (char* characters = chars)
				JSHandle = JavaScriptMethods.JSStringCreateWithCharacters(characters, (nuint)chars.Length);
		}
		public JSString(ReadOnlySpan<byte> utf8)
		{
			if (utf8.Length is 0 || utf8[utf8.Length - 1] is not 0) throw new ArgumentException("UTF8 byte span must have null-terminator (\\0) at the end. (If you're sure what you're doing, use byte* overload instead.)", nameof(utf8));
			fixed (byte* characters = utf8)
				JSHandle = JavaScriptMethods.JSStringCreateWithUTF8CString(characters);
		}

		public JSString Clone()
		{
			JSString returnValue = FromHandle(JavaScriptMethods.JSStringRetain(JSHandle), true);
			GC.KeepAlive(this);
			return returnValue;
		}
		object ICloneable.Clone() => Clone();

		public nuint Length
		{
			get
			{
				nuint returnValue = JavaScriptMethods.JSStringGetLength(JSHandle);
				GC.KeepAlive(this);
				return returnValue;
			}
		}

		/// <remarks>Use <see cref="GC.KeepAlive(object?)" /> to keep <see cref="JSString" /> instance alive.</remarks>
		public ReadOnlySpan<char> UTF16Data => new(UTF16DataRaw, checked((int)Length));
		/// <remarks>Use <see cref="GC.KeepAlive(object?)" /> to keep <see cref="JSString" /> instance alive.</remarks>
		public char* UTF16DataRaw
		{
			get
			{
				char* returnValue = (char*)JavaScriptMethods.JSStringGetCharactersPtr(JSHandle);
				GC.KeepAlive(this);
				return returnValue;
			}
		}
		// do not implement GetPinnableReference because there is UTF16DataRaw

		public nuint MaximumUTF8CStringSize
		{
			get
			{
				nuint returnValue = JavaScriptMethods.JSStringGetMaximumUTF8CStringSize(JSHandle);
				GC.KeepAlive(this);
				return returnValue;
			}
		}
		public nuint GetUTF8(byte* buffer, nuint bufferSize)
		{
			nuint returnValue = JavaScriptMethods.JSStringGetUTF8CString(JSHandle, buffer, bufferSize);
			GC.KeepAlive(this);
			return returnValue;
		}
		public nuint GetUTF8(Span<byte> buffer) { fixed (byte* bufferPtr = buffer) { return GetUTF8(bufferPtr, checked((nuint)buffer.Length)); } }

		public override string ToString()
		{
			string returnValue = new((char*)UTF16DataRaw, 0, checked((int)Length));
			GC.KeepAlive(this);
			return returnValue;
		}
		public override bool Equals(object? obj)
		{
			if (obj is string s) return Equals(s);
			return obj is JSString jsString && Equals(jsString);
		}
		public bool Equals(string? other)
		{
			if (other is null) return false;
			var retVal = UTF16Data.SequenceEqual(other.AsSpan());
			GC.KeepAlive(this);
			return retVal;
		}
		public bool Equals(JSString? other)
		{
			if (other is null) return false;
			var retVal = Equals((NativeContainer)other) || JavaScriptMethods.JSStringIsEqual(JSHandle, other.JSHandle);
			GC.KeepAlive(this);
			return retVal;
		}
		public static bool operator ==(JSString? left, JSString? right) => left is not null ? left.Equals(right) : right is null;
		public static bool operator !=(JSString? left, JSString? right) => !(left == right);

		public bool Equals(byte* other)
		{
			bool returnValue = JavaScriptMethods.JSStringIsEqualToUTF8CString(JSHandle, other);
			GC.KeepAlive(this);
			return returnValue;
		}
		public bool Equals(ReadOnlySpan<byte> other) { fixed (byte* bytes = other) { return Equals(bytes); } }

		public static implicit operator JSString(string? str) => new(string.IsNullOrEmpty(str) ? ReadOnlySpan<char>.Empty : str.AsSpan());
		public static explicit operator string(JSString str) => str.ToString();

		public static JSString FromHandle(JSStringRef handle, bool dispose) => new() { JSHandle = handle, Owns = dispose };

		public override void Dispose()
		{
			if (!IsDisposed && Owns) JavaScriptMethods.JSStringRelease(JSHandle);
			base.Dispose();
		}
	}
}
