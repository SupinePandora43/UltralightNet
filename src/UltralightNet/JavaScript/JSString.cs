// JSStringRef.h

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UltralightNet.LowStuff;

namespace UltralightNet.JavaScript;

unsafe partial class JavaScriptMethods
{
	[DllImport(LibWebCore)]
	public static extern Handle<JSString> JSStringCreateWithCharacters(char* characters, nuint length);

	[DllImport(LibWebCore)]
	public static extern Handle<JSString> JSStringCreateWithUTF8CString(byte* characters);

	/// <summary>Increases ref count</summary>
	[DllImport(LibWebCore)]
	public static extern Handle<JSString> JSStringRetain(Handle<JSString> @string);

	/// <summary>Decreases ref count</summary>
	[DllImport(LibWebCore)]
	public static extern void JSStringRelease(Handle<JSString> @string);

	[DllImport(LibWebCore)]
	public static extern nuint JSStringGetLength(Handle<JSString> @string);

	[DllImport(LibWebCore)]
	public static extern ushort* JSStringGetCharactersPtr(Handle<JSString> @string);

	[DllImport(LibWebCore)]
	public static extern nuint JSStringGetMaximumUTF8CStringSize(Handle<JSString> @string);

	[DllImport(LibWebCore)]
	public static extern nuint JSStringGetUTF8CString(Handle<JSString> @string, byte* buffer, nuint bufferSize);

	[GeneratedDllImport(LibWebCore)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool JSStringIsEqual(Handle<JSString> a, Handle<JSString> b);

	[GeneratedDllImport(LibWebCore)]
	[return: MarshalAs(UnmanagedType.I1)]
	public static partial bool JSStringIsEqualToUTF8CString(Handle<JSString> str, byte* characters);
}

public readonly struct JSStringRef {

}

[DebuggerDisplay("{ToString(),raw}")]
public unsafe sealed class JSString : INativeContainer<JSString>, INativeContainerInterface<JSString>, IEquatable<JSString>, ICloneable
{
	private JSString() { }

	public JSString(char* characters, nuint length) => Handle = JavaScriptMethods.JSStringCreateWithCharacters(characters, length);
	public JSString(byte* utf8Bytes) => Handle = JavaScriptMethods.JSStringCreateWithUTF8CString(utf8Bytes);
	public JSString(ReadOnlySpan<char> chars)
	{
		fixed (char* characters = chars)
			Handle = JavaScriptMethods.JSStringCreateWithCharacters(characters, (nuint)chars.Length);
	}
	public JSString(ReadOnlySpan<byte> utf8)
	{
		fixed (byte* characters = utf8)
			Handle = JavaScriptMethods.JSStringCreateWithUTF8CString(characters);
	}

	public JSString Clone()
	{
		JSString returnValue = JSString.FromHandle(JavaScriptMethods.JSStringRetain(Handle), true);
		GC.KeepAlive(this);
		return returnValue;
	}
	object ICloneable.Clone() => Clone();

	public nuint Length
	{
		get
		{
			nuint returnValue = JavaScriptMethods.JSStringGetLength(Handle);
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
			char* returnValue = (char*)JavaScriptMethods.JSStringGetCharactersPtr(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}
	// do not implement GetPinnableReference because there are UTF16DataRaw

	public nuint MaximumUTF8CStringSize
	{
		get
		{
			nuint returnValue = JavaScriptMethods.JSStringGetMaximumUTF8CStringSize(Handle);
			GC.KeepAlive(this);
			return returnValue;
		}
	}
	public nuint GetUTF8(byte* buffer, nuint bufferSize)
	{
		nuint returnValue = JavaScriptMethods.JSStringGetUTF8CString(Handle, buffer, bufferSize);
		GC.KeepAlive(this);
		return returnValue;
	}
	public nuint GetUTF8(Span<byte> buffer) { fixed (byte* bufferPtr = buffer) { return GetUTF8(bufferPtr, (nuint)buffer.Length); } }

	public override string ToString()
	{
		string returnValue = new((char*)UTF16DataRaw, 0, checked((int)Length));
		GC.KeepAlive(this);
		return returnValue;
	}
	public override bool Equals(object? obj)
	{
		if (obj is string s) return Equals(new JSString(s.AsSpan()));
		if (obj is not JSString) return false;
		return Equals((JSString)obj);
	}
	public bool Equals(JSString? other)
	{
		if (other is null) return false;
		if (ReferenceEquals(this, other)) return true;
		return JavaScriptMethods.JSStringIsEqual(Handle, other.Handle);
	}
	public static bool operator ==(JSString? left, JSString? right) => left is null ? right is null : left.Equals(right);
	public static bool operator !=(JSString? left, JSString? right) => !(left == right);

	public static bool ReferenceEquals(JSString s1, JSString s2) => s1 is null ? s2 is null : s1.Handle == s2.Handle;

#if NETSTANDARD2_1 || NETCOREAPP2_1_OR_GREATER
	public override int GetHashCode() => HashCode.Combine((nuint)_ptr, Owns, IsDisposed);
#else
		public override int GetHashCode() => unchecked((int)((nuint)_ptr ^ (Unsafe.As<bool, byte>(ref Unsafe.AsRef(IsDisposed)))));
#endif

	public bool Equals(byte* other)
	{
		bool returnValue = JavaScriptMethods.JSStringIsEqualToUTF8CString(Handle, other);
		GC.KeepAlive(this);
		return returnValue;
	}
	public bool Equals(ReadOnlySpan<byte> other) { fixed (byte* bytes = other) { return Equals(bytes); } }

	public static implicit operator JSString(string? str) => new(string.IsNullOrEmpty(str) ? ReadOnlySpan<char>.Empty : str.AsSpan());
	public static explicit operator string(JSString str) => str.ToString();

	public static JSString FromHandle(Handle<JSString> handle, bool dispose) => new() { Handle = handle, Owns = dispose };

	public override void Dispose()
	{
		if (!IsDisposed && Owns) JavaScriptMethods.JSStringRelease(Handle);
		base.Dispose();
	}
}
