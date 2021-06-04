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

		public static unsafe string NativeToManaged2(ULString* ulString)
		{
			ULString ported = *ulString;
			return new((char*)ported.data, 0, (int)ported.length);
		}

		[Benchmark]
		public string Second()
		{
			return NativeToManaged2(str);
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
