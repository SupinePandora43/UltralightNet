using System;
using Xunit;

namespace UltralightNet.Test
{
	public class ULViewConfigTest
	{
		private ULViewConfig viewConfig = new();

		[Fact]
		public void FinalizeTest(){
			viewConfig = null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}
		[Fact]
        public void DisposeTest(){
			viewConfig.Dispose();
			viewConfig.Dispose();
            viewConfig = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
	}
}