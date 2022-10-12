using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;

namespace UltralightNet.Test;

public unsafe class UnsafeMagic
{
	private delegate void AcceptingPointer(long* l);
	private delegate void AcceptingRef(ref long l);

	public long heapLong = -1;

	[Fact]
	public void StackPointerToRef()
	{
		AcceptingRef ar = (ref long l) => l = 10;
		AcceptingPointer ap = (l) => ar(ref Unsafe.AsRef<long>(l));
		long l = -1;
		ap(&l);
		Assert.Equal(10, l);
	}
	[Fact]
	public void HeapPointerToRef()
	{
		AcceptingRef ar = (ref long l) => l = 10;
		AcceptingPointer ap = (l) => ar(ref Unsafe.AsRef<long>(l));
		var lPtr = (long*)NativeMemory.Alloc(sizeof(long));
		try
		{
			*lPtr = -1;
			ap(lPtr);
			Assert.Equal(10, *lPtr);
		}
		finally
		{
			NativeMemory.Free(lPtr);
		}
	}
	[Fact]
	public void HeapRefToPointer()
	{
		AcceptingPointer ap = (l) => *l = 10;
		AcceptingRef ar = (ref long l) => ap((long*)Unsafe.AsPointer<long>(ref l));
		Assert.Equal(heapLong, -1);
		ar(ref heapLong);
		Assert.Equal(heapLong, 10);
	}
	[Fact]
	public void StackRefToPointer()
	{
		AcceptingPointer ap = (l) => *l = 10;
		AcceptingRef ar = (ref long l) => ap((long*)Unsafe.AsPointer<long>(ref l));
		long stackLong = -1;
		ar(ref stackLong);
		Assert.Equal(stackLong, 10);
	}
}