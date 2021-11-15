using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using System;
using System.Runtime.InteropServices;
using System.Threading;
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
		Renderer renderer;
		View view;

		public MyBenchmark()
		{
			AppCoreMethods.ulEnablePlatformFileSystem("./");
			AppCoreMethods.ulEnablePlatformFontLoader();
			renderer = ULPlatform.CreateRenderer();
			view = renderer.CreateView(512, 512);
		}

		~MyBenchmark()
		{
			view.Dispose();
			renderer.Dispose();
		}
	}
}
