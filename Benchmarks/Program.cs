using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
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

		private IntPtr cache;

		public MyBenchmark()
		{
			cache = Marshal.AllocHGlobal(4*1024+1);
			AppCoreMethods.ulEnablePlatformFileSystem("./");
			AppCoreMethods.ulEnablePlatformFontLoader();
			//renderer = ULPlatform.CreateRenderer();
			//view = renderer.CreateView(512, 512);
		}

		~MyBenchmark()
		{
			Marshal.FreeHGlobal(cache);
			//view.Dispose();
			//renderer.Dispose();
		}

		[Benchmark]
		[Arguments(0)]
		[Arguments(1)]
		[Arguments(8)]
		[Arguments(64)]
		[Arguments(1024)]
		[SkipLocalsInit]
		public void GlobalAllocEncodingBytes(int length){
			string str = new('а', length);
			int byteLen = Encoding.UTF8.GetByteCount(str);
			byte* bytes = (byte*) Marshal.AllocHGlobal(byteLen+1);
			bytes[Encoding.UTF8.GetBytes(str.AsSpan(), new Span<byte>(bytes, byteLen))+1] = 0;

			ULString ul = new(){data=bytes, length = (nuint) length};
			Methods.ulStringGetDataPtr(&ul);
			Marshal.FreeHGlobal((IntPtr)bytes);
		}

		[Benchmark]
		[Arguments(0)]
		[Arguments(1)]
		[Arguments(8)]
		[Arguments(64)]
		[Arguments(1024)]
		[SkipLocalsInit]
		public void CachedAllocEncodingBytes(int length){
			string str = new('а', length);
			int byteLen = Encoding.UTF8.GetByteCount(str);
			byte* bytes = (byte*) cache;
			bytes[Encoding.UTF8.GetBytes(str.AsSpan(), new Span<byte>(bytes, byteLen))+1] = 0;

			ULString ul = new(){data=bytes, length = (nuint) length};
			Methods.ulStringGetDataPtr(&ul);
		}

		[Benchmark]
		[Arguments(0)]
		[Arguments(1)]
		[Arguments(8)]
		[Arguments(64)]
		[SkipLocalsInit]
		public void StackAllocEncodingBytes(int length){
			string str = new('а', length);
			int byteLen = Encoding.UTF8.GetByteCount(str);
			byte* bytes = stackalloc byte[byteLen+1];
			bytes[Encoding.UTF8.GetBytes(str, new Span<byte>(bytes, byteLen))+1] = 0;

			ULString ul = new(){data=bytes, length = (nuint) length};
			Methods.ulStringGetDataPtr(&ul);
		}

		[Benchmark]
		[Arguments(0)]
		[Arguments(1)]
		[Arguments(8)]
		[Arguments(64)]
		[Arguments(1024)]
		[SkipLocalsInit]
		public unsafe void UltralightUTF16(int length){
			string str = new('а', length);
			ULString* ul = Methods.ulCreateStringUTF16(str, (nuint)str.Length);
			Methods.ulStringGetDataPtr(ul);
			Methods.ulDestroyString(ul);
		}
	}
}
