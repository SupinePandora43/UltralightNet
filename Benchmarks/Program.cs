using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Runtime.InteropServices;
using UltralightNet;

namespace Benchmarks
{
    public class Program
    {
        static void Main(string[] args)
        {
			var summary = BenchmarkRunner.Run<MyBenchmark>();
		}
	}

	public unsafe class MyBenchmark
	{
		private ULString* str;

		public MyBenchmark()
		{
			str = (ULString*)Methods.ulCreateStringUTF16("тесто", 5);
		}

		[Benchmark]
		public string First() // wins
		{
			return ULString.NativeToManaged(str);
		}
		[Benchmark]
		public string Second()
		{
			return ULString.NativeToManaged2(str);
		}

		[Benchmark]
		public string MF() // wins
		{
			// String::.ctor(Char*, int, int)
			return ULString.NativeToManaged(str);
		}
		[Benchmark]
		public string MM()
		{
			return Marshal.PtrToStringUni((IntPtr)str->data, (int)str->length);
		}
	}
}
