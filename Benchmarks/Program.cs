using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using System;
using System.Runtime.InteropServices;
using UltralightNet;

namespace Benchmarks
{
    public class Program
    {
        static void Main()
        {
			BenchmarkRunner.Run<MyBenchmark>();
			//BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());
		}
	}
	public unsafe class MyBenchmark
	{
		public MyBenchmark()
		{

		}
	}
}
