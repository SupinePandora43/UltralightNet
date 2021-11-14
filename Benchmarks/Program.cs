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

			return;

			bool loaded = false;

			view.OnFinishLoading += (_, _, _) => loaded = true;

			view.URL = "https://github.com";

			while (!loaded)
			{
				renderer.Update();
				Thread.Sleep(100);
			}

			renderer.Render();
		}

		[Benchmark]
		public void Manually()
		{
			ULScrollEvent @event = new() { type = ULScrollEventType.ByPixel, deltaX = 0, deltaY = 0 };
			view.FireScrollEvent(@event);
		}

		[Benchmark]
		public void Constructor()
		{
			ULScrollEvent @event = new(ULScrollEventType.ByPixel, 0, 0);
			view.FireScrollEvent(@event);
		}

		~MyBenchmark()
		{
			view.Dispose();
			renderer.Dispose();
		}
	}
}
