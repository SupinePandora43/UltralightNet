// JSStringRef.h

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UltralightNet
{
	unsafe partial class JavaScriptMethods
	{
		[DllImport(LibWebCore)]
		public static extern void* JSStringCreateWithCharacters(ushort* characters, nuint length);

		[DllImport(LibWebCore)]
		public static extern void* JSStringCreateWithUTF8CString(byte* characters);

		/// <summary>Increases ref count</summary>
		[DllImport(LibWebCore)]
		public static extern void* JSStringRetain(void* @string);

		/// <summary>Decreases ref count</summary>
		[DllImport(LibWebCore)]
		public static extern void JSStringRelease(void* @string);

		[DllImport(LibWebCore)]
		public static extern nuint JSStringGetLength(void* @string);

		[DllImport(LibWebCore)]
		public static extern ushort* JSStringGetCharactersPtr(void* @string);

		[DllImport(LibWebCore)]
		public static extern nuint JSStringGetMaximumUTF8CStringSize(void* @string);

		[DllImport(LibWebCore)]
		public static extern nuint JSStringGetUTF8CString(void* @string, byte* buffer, nuint bufferSize);

		[GeneratedDllImport(LibWebCore)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSStringIsEqual(void* a, void* b);

		[GeneratedDllImport(LibWebCore)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool JSStringIsEqualToUTF8CString(void* str, byte* characters);
	}

	[DebuggerDisplay("{ToString(),raw}")]
	public unsafe sealed class JSString : INativeContainer<JSString>, IEquatable<JSString>, ICloneable
	{
		private readonly void* handle;
		public void* Handle
		{
			get
			{
				static void Throw() => throw new ObjectDisposedException(nameof(JSString));
				if (IsDisposed) Throw();
				return handle;
			}
			init => handle = value;
		}
		private readonly bool dispose = true;
		public bool IsDisposed { get; private set; } = false;

		internal JSString(void* handle)
		{
			Handle = handle;
		}
		public JSString(void* handle, bool dispose) : this(handle) { this.dispose = dispose; }
		public JSString(ushort* characters, nuint length) : this(JavaScriptMethods.JSStringCreateWithCharacters(characters, length)) { }
		public JSString(byte* utf8Bytes) : this(JavaScriptMethods.JSStringCreateWithUTF8CString(utf8Bytes)) { }
		public JSString(ReadOnlySpan<ushort> chars)
		{
			fixed (ushort* characters = chars)
				handle = JavaScriptMethods.JSStringCreateWithCharacters(characters, (nuint)chars.Length);
		}
		public JSString(ReadOnlySpan<char> characters) : this(MemoryMarshal.Cast<char, ushort>(characters)) { }

		public JSString Clone() => new(JavaScriptMethods.JSStringRetain(Handle));
		object ICloneable.Clone() => Clone();

		public nuint Length => JavaScriptMethods.JSStringGetLength(Handle);

		public ReadOnlySpan<char> UTF16Data => new(JavaScriptMethods.JSStringGetCharactersPtr(Handle), (int)Length); // INTEROPTODO: INT64
		public char* UTF16DataRaw => (char*)JavaScriptMethods.JSStringGetCharactersPtr(Handle);
		// do not implement GetPinnableReference because there are UTF16DataRaw

		public nuint MaximumUTF8CStringSize => JavaScriptMethods.JSStringGetMaximumUTF8CStringSize(Handle);

		public nuint GetUTF8(byte* buffer, nuint bufferSize) => JavaScriptMethods.JSStringGetUTF8CString(Handle, buffer, bufferSize);
		public nuint GetUTF8(Span<byte> buffer) { fixed (byte* bufferPtr = buffer) { return JavaScriptMethods.JSStringGetUTF8CString(Handle, bufferPtr, (nuint)buffer.Length); } } // INTEROPTODO: INT64

		public override string ToString() => new((char*)UTF16DataRaw, 0, (int)Length);

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
		public override int GetHashCode() => HashCode.Combine((nint)handle, dispose, IsDisposed);
#else
		public override int GetHashCode() => unchecked((int)((nuint)handle ^ (Unsafe.As<bool, byte>(ref Unsafe.AsRef(dispose)))));
#endif

		public bool Equals(byte* other) => JavaScriptMethods.JSStringIsEqualToUTF8CString(Handle, other);
		public bool Equals(ReadOnlySpan<byte> other) { fixed (byte* bytes = other) { return Equals(bytes); } }

		public static implicit operator JSString(string obj) => new(obj is null ?
#if NETSTANDARD2_1 || NETCOREAPP2_1_OR_GREATER
		MemoryMarshal.CreateReadOnlySpan(
#if NET5_0_OR_GREATER
			ref Unsafe.NullRef<char>()
#else
			ref Unsafe.AsRef<char>(null)
#endif
			, 0)
#else
		new ReadOnlySpan<char>((char*)null, 0)
#endif
		: obj.AsSpan());
		public static explicit operator string(JSString str) => str.ToString()!;

		public JSString FromPointer(void* ptr) => new(ptr);

		~JSString() => Dispose();

		public void Dispose()
		{
			if (IsDisposed) return;

			if (dispose)
			{
				JavaScriptMethods.JSStringRelease(handle);
			}

			IsDisposed = true;
			GC.SuppressFinalize(this);
		}
	}

}
