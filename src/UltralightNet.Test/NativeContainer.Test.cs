using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using UltralightNet.LowStuff;

namespace UltralightNet.Test;

public class NativeContainerTests
{
	static bool disposed = false;
	class TestingContainer : NativeContainer
	{
		[SuppressMessage("CodeAnalysis", "CA1822")]
		public void DoNothing() { }
		[SuppressMessage("CodeAnalysis", "CA1816")]
		public override void Dispose()
		{
			disposed = true;
			base.Dispose();
		}
	}

	[Fact]
	[SuppressMessage("CodeAnalysis", "CA2000")]
	public void FinalizerTest()
	{
		var stopwatch = Stopwatch.StartNew();
		while (!disposed)
		{
			TestingContainer container = new();
			container.DoNothing();
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			if (stopwatch.Elapsed.Minutes >= 1) throw new TimeoutException();
		}
		Assert.True(disposed);
	}
}
