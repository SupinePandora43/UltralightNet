// JSStringRef.h

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UltralightNet.LowStuff;

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
	public unsafe sealed class JSString : INativeContainer<JSString>, INativeContainerInterface<JSString>, IEquatable<JSString>, ICloneable
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

		[Obsolete]
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

		public JSString Clone()
		{
			JSString returnValue = new(JavaScriptMethods.JSStringRetain(Handle));
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
		public override int GetHashCode() => HashCode.Combine((nint)handle, dispose, IsDisposed);
#else
		public override int GetHashCode() => unchecked((int)((nuint)handle ^ (Unsafe.As<bool, byte>(ref Unsafe.AsRef(dispose)))));
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

		public static JSString FromHandle(Handle<JSString> handle, bool dispose) => new((void*)handle, dispose);

		public override void Dispose()
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
