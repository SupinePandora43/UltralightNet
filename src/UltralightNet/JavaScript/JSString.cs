// JSStringRef.h

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UltralightNet.JavaScript.Low;
using UltralightNet.JavaScript.LowStuff;
using UltralightNet.LowStuff;

namespace UltralightNet.JavaScript
{
	namespace Low
	{
		unsafe partial class JavaScriptMethods
		{
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
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSStringIsEqual(JSStringRef a, JSStringRef b);

			[LibraryImport(LibWebCore)]
			[return: MarshalAs(UnmanagedType.U1)]
			public static partial bool JSStringIsEqualToUTF8CString(JSStringRef str, byte* characters);
		}

		public readonly struct JSStringRef
		{
			private readonly nuint _handle;
			public JSStringRef() => JavaScriptMethods.ThrowUnsupportedConstructor();
			public override int GetHashCode() => throw JavaScriptMethods.UnsupportedMethodException;
			public override bool Equals(object? o) => throw JavaScriptMethods.UnsupportedMethodException;

			public static bool operator ==(JSStringRef left, JSStringRef right) => left._handle == right._handle;
			public static bool operator !=(JSStringRef left, JSStringRef right) => left._handle != right._handle;
		}
	}
	namespace LowStuff
	{
		public abstract unsafe class JSNativeContainer<NativeHandle> : NativeContainer where NativeHandle : unmanaged
		{
			public NativeHandle JSHandle
			{
				get => JavaScriptMethods.BitCast<nuint, NativeHandle>((nuint)Handle);
				protected init => Handle = (void*)JavaScriptMethods.BitCast<NativeHandle, nuint>(value);
			}
		}
	}

	[DebuggerDisplay("{ToString(),raw}")]
	public unsafe sealed class JSString : JSNativeContainer<JSStringRef>, IEquatable<JSString>, IEquatable<string>, ICloneable
	{
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

		public bool EqualsNullTerminatedUTF8(byte* utf8)
		{
			bool returnValue = JavaScriptMethods.JSStringIsEqualToUTF8CString(JSHandle, utf8);
			GC.KeepAlive(this);
			return returnValue;
		}
		public bool EqualsNullTerminatedUTF8(ReadOnlySpan<byte> utf8)
		{
			if (utf8.Length is 0 || utf8[utf8.Length - 1] is not 0) throw new ArgumentException("UTF8 byte span must have null-terminator (\\0) at the end. (If you're sure what you're doing, use byte* overload instead.)", nameof(utf8));
			fixed (byte* bytes = utf8) return EqualsNullTerminatedUTF8(bytes);
		}

		public static implicit operator JSString(string? str) => CreateFromUTF16(str);
		public static explicit operator string(JSString str) => str.ToString();

		public static JSString FromHandle(JSStringRef handle, bool dispose) => new() { JSHandle = handle, Owns = dispose };

		public override void Dispose()
		{
			if (!IsDisposed && Owns) JavaScriptMethods.JSStringRelease(JSHandle);
			base.Dispose();
		}

		public static JSString CreateFromUTF16(char* chars, nuint length) => FromHandle(JavaScriptMethods.JSStringCreateWithCharacters(chars, length), true);
		public static JSString CreateFromUTF16(ReadOnlySpan<char> chars)
		{
			fixed (char* characters = chars)
				return FromHandle(JavaScriptMethods.JSStringCreateWithCharacters(characters, (nuint)chars.Length), true);
		}
		public static JSString CreateFromUTF16Cached(string? @string)
		{
			// on average, 23 times faster than without cache
			@string ??= string.Empty;
			if (Cache.TryGetValue(@string, out var js)) return js.Clone();
			js = CreateFromUTF16(@string.AsSpan());
			Cache.Add(@string, js);
			return js.Clone();
		}

		public static JSString CreateFromUTF8NullTerminated(byte* utf8Bytes) => FromHandle(JavaScriptMethods.JSStringCreateWithUTF8CString(utf8Bytes), true);
		public static JSString CreateFromUTF8NullTerminated(ReadOnlySpan<byte> utf8)
		{
			if (utf8.Length is 0 || utf8[utf8.Length - 1] is not 0) throw new ArgumentException("UTF8 byte span must have null-terminator (\\0) at the end. (If you're sure what you're doing, use byte* overload instead.)", nameof(utf8));
			fixed (byte* characters = utf8)
				return CreateFromUTF8NullTerminated(characters);
		}

		static readonly ConditionalWeakTable<string, JSString> Cache = new();
	}
}
