using UltralightNet.JavaScript;
using UltralightNet.JavaScript.Low;

namespace Benchmarks;

public unsafe class JSStringBenchmark : IDisposable
{
	readonly JSGlobalContextRef ctx;

	public JSStringBenchmark()
	{
		ctx = JavaScriptMethods.JSGlobalContextCreate();
	}
	public void Dispose()
	{
		JavaScriptMethods.JSGlobalContextRelease(ctx);
	}

	public static IEnumerable<string> GetStrings()
	{
		yield return string.Empty;
		yield return new('c', 2 ^ 6);
		yield return new('c', 2 ^ 10);
		yield return new('Ы', 2 ^ 10);
	}

	void ProcessJSString(JSString str)
	{
		JavaScriptMethods.JSGlobalContextSetName(ctx, str.JSHandle);
		GC.KeepAlive(str);
	}

	[Benchmark(Baseline = true)]
	[ArgumentsSource(nameof(GetStrings))]
	public void CreateFromUTF16(string str) => ProcessJSString(JSString.CreateFromUTF16(str));

	[Benchmark]
	[ArgumentsSource(nameof(GetStrings))]
	public void CreateFromUTF16NOGC(string str)
	{
		using var jsString = JSString.CreateFromUTF16(str);
		ProcessJSString(jsString);
	}

	[Benchmark]
	[ArgumentsSource(nameof(GetStrings))]
	public void CreateFromUTF16Cached(string str) => ProcessJSString(JSString.CreateFromUTF16Cached(str));

	[Benchmark]
	[ArgumentsSource(nameof(GetStrings))]
	public void CreateFromUTF16CachedNOGC(string str)
	{
		using var jsString = JSString.CreateFromUTF16Cached(str);
		ProcessJSString(jsString);
	}
}
