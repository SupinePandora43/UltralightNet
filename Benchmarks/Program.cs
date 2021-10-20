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
			//BenchmarkRunner.Run<MyBenchmark>();
			BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(null, new DebugInProcessConfig());
		}
	}
	public unsafe class MyBenchmark
	{
		View view;

		public MyBenchmark()
		{
			view = ULPlatform.CreateRenderer().CreateView(512, 512);
		}

		[Benchmark]
		public void WithIn()
		{
			ULScrollEvent @event = new() { type = ULScrollEvent.ScrollType.ByPixel, deltaX = 0, deltaY = 0 };
			view.FireScrollEvent(in @event);
		}

		[Benchmark]
		public void WithoutIn()
		{
			ULScrollEvent @event = new() { type = ULScrollEvent.ScrollType.ByPixel, deltaX = 0, deltaY = 0 };
			view.FireScrollEventWithoutIn(@event);
		}
	}
}
