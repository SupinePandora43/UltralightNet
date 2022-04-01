using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace UltralightNet
{
	public static partial class Methods
	{
		/// <summary>Create string from null-terminated ASCII C-string.</summary>
		[GeneratedDllImport("Ultralight")]
		public unsafe static partial ULString* ulCreateString([MarshalUsing(typeof(UTF8Marshaller))] string str);

		/// <summary>Create string from UTF-8 buffer.</summary>
		[DllImport("Ultralight")]
		[Obsolete("Unexpected behaviour")]
		public static unsafe extern ULString* ulCreateStringUTF8(
			byte* data,
			nuint len
		);

		/// <summary>Create string from UTF-16 buffer.</summary>
		[GeneratedDllImport("Ultralight")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe partial ULString* ulCreateStringUTF16([MarshalAs(UnmanagedType.LPWStr)] string str, nuint len);

		// <summary>Create string from copy of existing string.</summary>
		[DllImport("Ultralight")]
		public static unsafe extern ULString* ulCreateStringFromCopy(ULString* str);

		/// <summary>Destroy string (you should destroy any strings you explicitly Create).</summary>
		[DllImport("Ultralight")]
		public static unsafe extern void ulDestroyString(ULString* str);

		/// <summary>Get internal UTF-8 buffer data.</summary>
		[DllImport("Ultralight", EntryPoint = "ulStringGetData")]
		public static unsafe extern byte* ulStringGetDataPtr(ULString* str);

		/// <summary>Get length in UTF-8 characters.</summary>
		[DllImport("Ultralight")]
		public static unsafe extern nuint ulStringGetLength(ULString* str);

		/// <summary>Whether this string is empty or not.</summary>
		[GeneratedDllImport("Ultralight")]
		[return: MarshalAs(UnmanagedType.I1)]
		public static unsafe partial bool ulStringIsEmpty(ULString* str);

		/// <summary>Replaces the contents of 'str' with the contents of 'new_str'</summary>
		[DllImport("Ultralight")]
		public static unsafe extern void ulStringAssignString(ULString* str, ULString* newStr);

		[DllImport("Ultralight")]
		public static unsafe extern void ulStringAssignCString(ULString* str, byte* c_str);
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
	[CustomTypeMarshaller(typeof(string), CustomTypeMarshallerKind.Value, Features = CustomTypeMarshallerFeatures.UnmanagedResources)]
	public unsafe ref struct ULStringGeneratedDllImportMarshaler
	{
		private bool allocated = false; // INTEROPTODO: UNNECESSARYCHECK

		[SkipLocalsInit]
		public ULStringGeneratedDllImportMarshaler(string str)
		{
			if (str is null) throw new ArgumentNullException(nameof(str)); // INTEROPTODO: C#VER
			Value = Methods.ulCreateStringUTF16(str, (nuint)str.Length);
			allocated = true;
		}

		[SkipLocalsInit]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToManaged() =>
#if NETFRAMEWORK || NETSTANDARD2_0
			new((sbyte*)ulstringPtr->data, 0, (int)ulstringPtr->length, Encoding.UTF8);
#else
			Marshal.PtrToStringUTF8((IntPtr)Value->data, (int)Value->length);
#endif

		public unsafe ULString* Value
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[SkipLocalsInit]
		public void FreeNative()
		{
			if (allocated) Methods.ulDestroyString(Value);
		}
	}

	[DebuggerDisplay("{ToManaged(),raw}")]
	[BlittableType]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe readonly struct ULString
	{
		public readonly byte* data;
		public readonly nuint length;

		public ULString(byte* data, nuint length)
		{
#if NET5_0_OR_GREATER
			Unsafe.SkipInit(out this);
#endif
			this.data = data;
			this.length = length;
		}

		/// <remarks>Do not use on pointers</remarks>
		[SkipLocalsInit]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToManaged() =>
#if NETSTANDARD2_0
			Encoding.UTF8.GetString(data, (int)length);
#elif NETFRAMEWORK
			new string((sbyte*)data, 0, (int)length);
#else
			Marshal.PtrToStringUTF8((IntPtr)data, (int)length);
#endif

		[SkipLocalsInit]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe string NativeToManaged(ULString* ulString)
		{
#if NETSTANDARD2_1 || NETCOREAPP1_1_OR_GREATER
			return Marshal.PtrToStringUTF8((IntPtr)ulString->data, (int)ulString->length);
#else
			return new((sbyte*)ulString->data, 0, (int)ulString->length, Encoding.UTF8);
#endif
		}

		/// <summary>Used for getting **opaque** <see cref="ULString" /> struct.<br />Performs worse than <see cref="ULStringGeneratedDllImportMarshaler" /></summary>
		/// <remarks>Requires manual freeing</remarks>
		[SkipLocalsInit]
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
		[SkipLocalsInit]
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
