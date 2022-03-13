// JSStringRef.h

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UltralightNet
{

	unsafe partial class JavaScriptMethods
	{
		[DllImport("WebCore")]
		public static extern void* JSStringCreateWithCharacters(ushort* characters, nuint length);

		[DllImport("WebCore")]
		public static extern void* JSStringCreateWithUTF8CString(byte* characters);

		/// <summary>Increases ref count</summary>
		[DllImport("WebCore")]
		public static extern void* JSStringRetain(void* @string);

		/// <summary>Decreases ref count</summary>
		[DllImport("WebCore")]
		public static extern void JSStringRelease(void* @string);

		[DllImport("WebCore")]
		public static extern nuint JSStringGetLength(void* @string);

		[DllImport("WebCore")]
		public static extern ushort* JSStringGetCharactersPtr(void* @string);

		[DllImport("WebCore")]
		public static extern nuint JSStringGetMaximumUTF8CStringSize(void* @string);

		[DllImport("WebCore")]
		public static extern nuint JSStringGetUTF8CString(void* @string, byte* buffer, nuint bufferSize);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSStringIsEqual(void* a, void* b);

		[GeneratedDllImport("WebCore")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSStringIsEqualToUTF8CString(void* str, byte* characters);
	}

	[DebuggerDisplay("{ToString(),raw}")]
	public unsafe sealed class JSString : IDisposable, IEquatable<JSString>, ICloneable
	{
		private readonly void* handle;
		private readonly bool dispose = true;
		private bool isDisposed = false;

		public JSString(void* handle, bool dispose = false)
		{
			this.handle = handle;
			this.dispose = dispose;
		}
		public JSString(string str)
		{
			fixed (char* characters = str)
			{
				handle = JavaScriptMethods.JSStringCreateWithCharacters((ushort*)characters, (nuint)str.Length);
			}
		}

		public void* Handle => handle;

		public JSString Clone() => new(JavaScriptMethods.JSStringRetain(Handle), true);
		object ICloneable.Clone() => Clone();

		public nuint Length => JavaScriptMethods.JSStringGetLength(Handle);

		public ReadOnlySpan<ushort> UTF16Data => new(JavaScriptMethods.JSStringGetCharactersPtr(Handle), (int)Length); // INTEROPTODO: INT64
		public ushort* UTF16DataRaw => JavaScriptMethods.JSStringGetCharactersPtr(Handle);

		public nuint MaximumUTF8CStringSize => JavaScriptMethods.JSStringGetMaximumUTF8CStringSize(Handle);

		public nuint GetUTF8(byte* buffer, nuint bufferSize) => JavaScriptMethods.JSStringGetUTF8CString(Handle, buffer, bufferSize);
		public nuint GetUTF8(Span<byte> buffer) { fixed (byte* bufferPtr = buffer) { return JavaScriptMethods.JSStringGetUTF8CString(Handle, bufferPtr, (nuint)buffer.Length); } } // INTEROPTODO: INT64

		public override string? ToString() => new((char*)UTF16DataRaw, 0, (int)Length);

		public override bool Equals(object? other) => Equals(other is not null and string ? (JSString)((string)other) : other as JSString);
		public bool Equals(JSString? other)
		{
			if (other is null) return false;
			return JavaScriptMethods.JSStringIsEqual(Handle, other.Handle);
		}
		public static bool operator ==(JSString left, JSString right) => left.Equals(right);
		public static bool operator !=(JSString left, JSString right) => !(left == right);

		public static bool ReferenceEquals(JSString s1, JSString s2) => s1.UTF16DataRaw == s2.UTF16DataRaw && s1.Length == s2.Length;

		public override int GetHashCode()
		{
			HashCode hash = new();
			hash.AddBytes(new ReadOnlySpan<byte>((byte*)UTF16DataRaw, (int)(Length * 2))); // INTEROPTODO: INT64
			return hash.ToHashCode();
		}

		public bool Equals(byte* other) => JavaScriptMethods.JSStringIsEqualToUTF8CString(Handle, other);
		public bool Equals(ReadOnlySpan<byte> other) { fixed (byte* bytes = other) { return Equals(bytes); } }

		public static implicit operator JSString(string obj) => new(obj ?? string.Empty);
		public static explicit operator string(JSString str) => str?.ToString()!;

		~JSString() => Dispose();

		public void Dispose()
		{
			if (isDisposed) return;

			if (dispose)
			{
				JavaScriptMethods.JSStringRelease(handle);
			}

			isDisposed = true;
			GC.SuppressFinalize(this);
		}
	}

}
