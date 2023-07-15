#if !NET7_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Runtime.InteropServices.Marshalling;

[CustomMarshaller(typeof(string), MarshalMode.Default, typeof(Utf8StringMarshaller))]
[CustomMarshaller(typeof(string), MarshalMode.ManagedToUnmanagedIn, typeof(ManagedToUnmanagedIn))]
internal static unsafe class Utf8StringMarshaller
{
	public static string? ConvertToManaged(byte* unmanaged) =>
#if NETSTANDARD2_1 || NETCOREAPP1_1_OR_GREATER
		Marshal.PtrToStringUTF8((IntPtr)unmanaged);
#else
        new((sbyte*)unmanaged);
#endif
	public static byte* ConvertToUnmanaged(string? managed)
	{
		if (managed is null) return null;
		int len = managed.Length;
		int byteCount = checked((len + 1) * 3 + 1);
		var bytes = (byte*)NativeMemory.Alloc((nuint)byteCount);

#if NETSTANDARD2_1 || NET
		int written = Encoding.UTF8.GetBytes(managed.AsSpan(), new Span<byte>(bytes, byteCount));
#else
        int written;
        fixed (char* characterPtr = managed)
            written = Encoding.UTF8.GetBytes(characterPtr, len, bytes, byteCount);

#endif
		bytes[written] = 0;
		return bytes;
	}
	public static void Free(byte* unmanaged) => NativeMemory.Free(unmanaged);
	public ref struct ManagedToUnmanagedIn
	{
		public static int BufferSize => 128;

		private byte* unmanaged = null;
		private bool allocated = false;

		public ManagedToUnmanagedIn() { }

		public void Free()
		{
			if (!allocated) return;
			NativeMemory.Free(unmanaged);
			unmanaged = null;
			allocated = false;
		}
		public void FromManaged(string? managed, Span<byte> buffer)
		{
			if (managed is null) return;

			ReadOnlySpan<char> managedSpan = managed;
			int len = managedSpan.Length;
			int byteCount = checked((len + 1) * 3 + 1);

			Span<byte> unmanaged;
			if (byteCount <= buffer.Length) unmanaged = buffer;
			else
			{
				unmanaged = new(NativeMemory.Alloc((nuint)byteCount), byteCount);
				allocated = true;
			}

#if NETSTANDARD2_1 || NET
			int written = Encoding.UTF8.GetBytes(managedSpan, unmanaged);
#else
			int written;
			fixed (char* characterPtr = managed)
			fixed (byte* unmanagedPtr = unmanaged)
				written = Encoding.UTF8.GetBytes(characterPtr, len, unmanagedPtr, byteCount);
#endif
			unmanaged[written] = 0;

			this.unmanaged = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(unmanaged));
		}
		public readonly byte* ToUnmanaged() => unmanaged;
	}
}
#endif
