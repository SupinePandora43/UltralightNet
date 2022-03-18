using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using UltralightNet;
using UltralightNet.AppCore;

namespace Benchmarks
{
	public class Program
	{
		static void Main()
		{
			BenchmarkRunner.Run<MyBenchmark>();
			//BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(null, new DebugInProcessConfig());
		}
	}
	public unsafe class MyBenchmark
	{
		private ULString str;
		public MyBenchmark(){
			str = ULString.CreateOpaque(new string('б',1000000));
		}
		~MyBenchmark(){
			ULString.FreeOpaque(str);
		}
		[Benchmark]
		public string UsingMarshal(){
			return Marshal.PtrToStringUTF8((IntPtr)str.data, (int)str.length);
		}
		[Benchmark]
		public string UsingCtor(){
			return new string((sbyte*)str.data, 0, (int)str.length);
		}
		[Benchmark]
		public string UsingEncoding(){
			return Encoding.UTF8.GetString(str.data, (int)str.length);
		}
	}
}
