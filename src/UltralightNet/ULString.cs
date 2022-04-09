using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using LibraryImportAttribute = System.Runtime.InteropServices.GeneratedDllImportAttribute;

namespace UltralightNet
{
	public static unsafe partial class Methods
	{
		/// <summary>Create string from null-terminated ASCII C-string.</summary>
		[LibraryImport(LibUltralight)]
		public static partial ULString* ulCreateString([MarshalUsing(typeof(UTF8Marshaller))] string str);

		/// <summary>Create string from UTF-8 buffer.</summary>
		[LibraryImport(LibUltralight)]
		[Obsolete("Unexpected behaviour")]
		public static partial ULString* ulCreateStringUTF8(
			byte* data,
			nuint len
		);

		/// <summary>Create string from UTF-16 buffer.</summary>
		[LibraryImport(LibUltralight)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static partial ULString* ulCreateStringUTF16([MarshalAs(UnmanagedType.LPWStr)] string str, nuint len);

		/// <summary>Create string from UTF-16 buffer.</summary>
		[LibraryImport(LibUltralight)]
		public static partial ULString* ulCreateStringUTF16(ushort* str, nuint len);

		// <summary>Create string from copy of existing string.</summary>
		public static ULString* ulCreateStringFromCopy(ULString* str){
			var ulString = (ULString*) NativeMemory.Alloc((nuint)sizeof(ULString));
			ulString->data = (byte*) NativeMemory.Alloc(str->length + 1);
			if(str->length < (nuint)int.MaxValue) new ReadOnlySpan<byte>(str->data, (int)str->length).CopyTo(new Span<byte>(ulString->data, (int)ulString->length));
			else for(nuint i = 0; i<str->length; i++)ulString->data[i] = str->data[i];
			ulString->length = str->length;
			return ulString;
		}

		/// <summary>Destroy string (you should destroy any strings you explicitly Create).</summary>
		[LibraryImport(LibUltralight)]
		public static partial void ulDestroyString(ULString* str);

		/// <summary>Get internal UTF-8 buffer data.</summary>
		[LibraryImport(LibUltralight)]
		public static partial byte* ulStringGetData(ULString* str);

		/// <summary>Get length in UTF-8 characters.</summary>
		[LibraryImport(LibUltralight)]
		public static partial nuint ulStringGetLength(ULString* str);

		/// <summary>Whether this string is empty or not.</summary>
		[LibraryImport(LibUltralight)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static partial bool ulStringIsEmpty(ULString* str);

		/// <summary>Replaces the contents of 'str' with the contents of 'new_str'</summary>
		[LibraryImport(LibUltralight)]
		public static partial void ulStringAssignString(ULString* str, ULString* newStr);

		[LibraryImport(LibUltralight)]
		public static partial void ulStringAssignCString(ULString* str, byte* c_str);
	}

	/// <remarks>Must be used with <code>using</code> statement</remarks
	public unsafe ref struct ULStringDisposableStackToNativeMarshaller
	{
		public ULStringDisposableStackToNativeMarshaller(in string str)
		{
			if (str is null) throw new ArgumentNullException(nameof(str)); // INTEROPTODO: C#VER
			Native = Methods.ulCreateStringUTF16(str, (nuint)str.Length);
		}
		public ULString* Native { get; }
		public void Dispose() => Methods.ulDestroyString(Native);
	}
	// INTEROPTODO: CUSTOMTYPEMARSHALLER
	[StructLayout(LayoutKind.Auto)]
	public unsafe ref struct ULStringToNativeMarshaller
	{
#if !UL_CONSERVATIVESTACK
		[StructLayout(LayoutKind.Explicit, Size = BufferSize)]
		private struct Memory { }
#endif

		private const int BufferSize = 0x400;

		private ULString native;
		private byte* allocatedMemory = null;

#if !UL_CONSERVATIVESTACK
		private Memory stack;
#endif

		// INTEROPTODO: INT64
		public ULStringToNativeMarshaller(ReadOnlySpan<char> str)
		{
			nuint byteCount =
#if UL_CONSERVATIVEHEAP
				(nuint)Encoding.UTF8.GetByteCount(str) + 1; // zero-byte end
#else
				// EXTREMELY fast length check with cost of memory usage
				((nuint)str.Length + 1) * 3 + 1; // 3 because UTF16 can't produce more when converting to UTF8
#endif
#if !UL_CONSERVATIVESTACK
			if (byteCount <= (nuint)BufferSize)
			{
				var stackPtr = (byte*)Unsafe.AsPointer(ref stack);
				nuint written
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1
					= (nuint)Encoding.UTF8.GetBytes(str, new Span<byte>(stackPtr, BufferSize));
#else
					;
				fixed(char* chars = str)
					written = (nuint) Encoding.UTF8.GetBytes(chars, str.Length, stackPtr, BufferSize);
#endif

				stackPtr[written] = 0; // zero-byte end
				native = new(stackPtr, (nuint)written);
				allocatedMemory = null;
			}
			else
#endif
			if (byteCount <= (nuint)int.MaxValue)
			{
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1
				nuint written = (nuint)Encoding.UTF8.GetBytes(str, new Span<byte>(allocatedMemory = (byte*)NativeMemory.Alloc((nuint)byteCount), (int)byteCount));
#else
				nuint written;
				fixed(char* chars = str)
					written = (nuint)Encoding.UTF8.GetBytes(chars, str.Length, allocatedMemory, (int)byteCount);
#endif
				allocatedMemory[written] = 0;
				//
				// NativeMemory.Realloc(allocatedMemory, written);
				native = new(allocatedMemory, (nuint)written);
			}
			else
			{
				// i can optimize it, but i will not, until you ask me to do so.
				// (do not copy from temp and just use it instead)
				ULString* temp;
				fixed (char* strPtr = str)
					temp = Methods.ulCreateStringUTF16((ushort*)strPtr, (nuint)str.Length);
				try
				{
					nuint len = temp->length;
					native.data = allocatedMemory = (byte*)NativeMemory.Alloc(len);
#if NETCOREAPP1_0_OR_GREATER || NET46_OR_GREATER || NETSTANDARD1_3_OR_GREATER
					Buffer.MemoryCopy(temp->data, allocatedMemory, len, len);
#else
					for(nuint i = 0; i < len ; i++) allocatedMemory[i] = temp->data[i];
#endif
					native.length = len;
				}
				finally { Methods.ulDestroyString(temp); }
			}
		}
		public ULStringToNativeMarshaller()
		{
			native.data = allocatedMemory = (byte*)NativeMemory.Alloc(1); // prevent potetntial errors
			native.data[0] = 0; // zero-byte end
			native.length = 0; // zero length
		}

		public ULString* ToNativeValue() => (ULString*)Unsafe.AsPointer(ref native); // INTEROPTODO: C#VER
		public int ToManagedValue(Span<char> span)
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1
			=> Encoding.UTF8.GetChars(new ReadOnlySpan<byte>(native.data, (int)native.length), span);
#else
		{
			fixed(char* chars = span)
				return Encoding.UTF8.GetChars(native.data, (int)native.length, chars, span.Length);
		}
#endif
		public string ToManagedValue()
		{
			Span<char> chars = new char[native.length];
			int written = ToManagedValue(chars);
			return chars.Slice(0, written).ToString();
		}
		// INTEROPTODO: CUSTOMTYPEMARSHALLER
		public ULString* Value => ToNativeValue();

		public void FreeNative()
		{
			if (allocatedMemory is not null)
			{
				if (allocatedMemory == native.data)
					NativeMemory.Free(allocatedMemory);
				else
					NativeMemory.Free(native.data);
				allocatedMemory = null;
				native.data = null;
			}
			native.length = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose() => FreeNative();

		public ULString Opaque => native;
	}
	// INTEROPTODO: CUSTOMTYPEMARSHALLER
	[CustomTypeMarshaller(typeof(string), CustomTypeMarshallerKind.Value, Features = CustomTypeMarshallerFeatures.UnmanagedResources)]
	public unsafe ref struct ULStringGeneratedDllImportMarshaler
	{
		private bool allocated = false; // INTEROPTODO: UNNECESSARYCHECK

		public ULStringGeneratedDllImportMarshaler(string str)
		{
			if (str is null) throw new ArgumentNullException(nameof(str)); // INTEROPTODO: C#VER
			Value = Methods.ulCreateStringUTF16(str, (nuint)str.Length);
			allocated = true;
		}

		public string ToManaged() =>
#if NETFRAMEWORK || NETSTANDARD2_0
			new((sbyte*)Value->data, 0, (int)Value->length, Encoding.UTF8);
#else
			Marshal.PtrToStringUTF8((IntPtr)Value->data, (int)Value->length);
#endif

		public unsafe ULString* Value
		{
			get;
			set;
		}

		public void FreeNative()
		{
			if (allocated) Methods.ulDestroyString(Value);
		}
	}

	[DebuggerDisplay("{ToManaged(),raw}")]
	[BlittableType]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ULString
	{
		internal const bool Conservative = false;

		public byte* data;
		public nuint length;

		public ULString(byte* data, nuint length)
		{
#if NET5_0_OR_GREATER
			Unsafe.SkipInit(out this);
#endif
			this.data = data;
			this.length = length;
		}

		/// <remarks>Do not use on pointers</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToManaged() =>
#if NETSTANDARD2_0
			Encoding.UTF8.GetString(data, (int)length);
#elif NETFRAMEWORK
			new string((sbyte*)data, 0, (int)length);
#else
			Marshal.PtrToStringUTF8((IntPtr)data, (int)length);
#endif

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string NativeToManaged(ULString* ulString)
		{
#if NETSTANDARD2_1 || NETCOREAPP1_1_OR_GREATER
			return Marshal.PtrToStringUTF8((IntPtr)ulString->data, (int)ulString->length);
#else
			return new((sbyte*)ulString->data, 0, (int)ulString->length, Encoding.UTF8);
#endif
		}

		/// <summary>Used for getting **opaque** <see cref="ULString" /> struct.<br />Performs worse than <see cref="ULStringGeneratedDllImportMarshaler" /></summary>
		/// <remarks>Requires manual freeing</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ULString CreateOpaque(string str)
		{
			ULStringGeneratedDllImportMarshaler marshaler = new(str);
			ULString* marshalled = marshaler.Value;
			byte* data =
#if NET6_0_OR_GREATER
				(byte*)NativeMemory.Alloc(marshalled->length);
#else
				(byte*)Marshal.AllocHGlobal((int)marshalled->length); // INTEROPTODO: INT64
#endif
			new ReadOnlySpan<byte>(marshalled->data, (int)marshalled->length).CopyTo(new Span<byte>(data, (int)marshalled->length)); // INTEROPTODO: INT64
			ULString ulString = new(data, marshalled->length);
			marshaler.FreeNative();
			return ulString;
		}
		/// <summary>Free **opaque** <see cref="ULString" /> instance.</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void FreeOpaque(ULString ulString)
		{
#if NET6_0_OR_GREATER
			NativeMemory.Free(ulString.data);
#else
			Marshal.FreeHGlobal((IntPtr)ulString.data);
#endif
		}
	}
}
