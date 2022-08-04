using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using UltralightNet;

namespace Benchmarks;

public class MyConfig : ManualConfig
{
	public MyConfig()
	{
		AddJob(Job.Default.WithRuntime(CoreRuntime.Core60));
		// AddJob(Job.Default.WithRuntime(CoreRuntime.CreateForNewVersion("net7.0", ".NET Core 7")));
	}
}

public class Program
{
	static void Main()
	{
		BenchmarkRunner.Run<SimdBenchmark>();
		//BenchmarkRunner.Run<StringBenchmark>();
		//BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(null, new DebugInProcessConfig());
	}
}

[Config(typeof(MyConfig))]
public class SimdBenchmark
{
	public static IEnumerable<ULIntRect> GetRects()
	{
		yield return new() { Left = 0, Top = 0, Right = 512, Bottom = 512 };
	}

	[Benchmark]
	[ArgumentsSource(nameof(GetRects))]
	public ULIntRect ToIntRect(ULRect rect) => (ULIntRect)rect;
}
