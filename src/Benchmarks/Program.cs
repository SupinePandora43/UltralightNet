using System.Collections.Generic;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using UltralightNet;

namespace Benchmarks;

public class Program
{
	static void Main()
	{
		BenchmarkRunner.Run<StringBenchmark>();
		//BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(null, new DebugInProcessConfig());
	}
}

[MemoryDiagnoser]
public unsafe class StringBenchmark
{
	public StringBenchmark() { }

	private const int Step = 1024;
	private const int Steps = 10;

	public static IEnumerable<string> GetTestStrings()
	{
		for (int step = 0; step < Steps; step++)
		{
			yield return new('Ё', step * Step);
		}
	}

	[Benchmark]
	[ArgumentsSource(nameof(GetTestStrings))]
	public void ulCreateStringUTF16(string str)
	{
		ULString* n = Methods.ulCreateStringUTF16(str, (nuint)str.Length);
		Methods.ulDestroyString(n);
	}
	[Benchmark]
	[ArgumentsSource(nameof(GetTestStrings))]
	public void Managed(string str)
	{
		ULString* n = (ULString*)NativeMemory.Alloc((nuint)sizeof(ULString));

		*n = new(str);

		n->Dispose();

		NativeMemory.Free(n);
	}
}
