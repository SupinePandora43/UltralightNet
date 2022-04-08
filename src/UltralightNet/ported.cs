using System.Runtime.CompilerServices;

#if !NET6_0_OR_GREATER
namespace System.Runtime.InteropServices
{
	internal static unsafe class NativeMemory
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void* Alloc(nuint size) => (void*)Marshal.AllocHGlobal(new IntPtr((void*)size));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Free(void* memory) => Marshal.FreeHGlobal((IntPtr)memory);
	}
}
#endif
#if !(NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1)
namespace System.Diagnostics.CodeAnalysis
{
	internal sealed class MaybeNullWhenAttribute : Attribute
	{
		public MaybeNullWhenAttribute(bool returnValue)
		{
			ReturnValue = returnValue;
		}

		public bool ReturnValue { get; }
	}
}
#endif