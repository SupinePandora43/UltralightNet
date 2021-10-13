using System;
using Xunit;

namespace UltralightNet.Test
{
	public class ULConfigTest
	{
		private ULConfig config = new();

		private static void ulConfigSetCachePath(IntPtr config, in string cache_path)
		{
			unsafe
			{
				global::UltralightNet.ULString __cache_path_gen = default;
				global::UltralightNet.ULString* __cache_path_gen_native = default;
				//
				// Pin
				//
				fixed(char* __cache_path_gen_native_pinned = cache_path)
				{
					__cache_path_gen = new ULString() { data = (ushort*)__cache_path_gen_native_pinned, length = (nuint)cache_path.Length };
					__cache_path_gen_native = &__cache_path_gen;
					//
					// Invoke
					//
					ulConfigSetCachePath__PInvoke__(config, __cache_path_gen_native);
				}
			}
		}

		[System.Runtime.InteropServices.DllImportAttribute("Ultralight", EntryPoint = "ulConfigSetCachePath")]
		extern private static unsafe void ulConfigSetCachePath__PInvoke__(global::System.IntPtr config, global::UltralightNet.ULString* cache_path);

		[Fact]
		public void CachePathTest()
		{
			config.CachePath = "./cache";

			ulConfigSetCachePath(config.Ptr, "./cache");

			Assert.Equal("./cache", config.CachePath);
		}
		[Fact]
		public void FaceWindingTest()
		{
			config.FaceWinding = ULFaceWinding.Clockwise;
			Assert.Equal(ULFaceWinding.Clockwise, config.FaceWinding);
			config.FaceWinding = ULFaceWinding.CounterClockwise;
			Assert.Equal(ULFaceWinding.CounterClockwise, config.FaceWinding);
		}
		[Fact]
		public void FontHintingTest()
		{
			config.FontHinting = ULFontHinting.Smooth;
			Assert.Equal(ULFontHinting.Smooth, config.FontHinting);
			config.FontHinting = ULFontHinting.Normal;
			Assert.Equal(ULFontHinting.Normal, config.FontHinting);
			config.FontHinting = ULFontHinting.Monochrome;
			Assert.Equal(ULFontHinting.Monochrome, config.FontHinting);
		}
		[Fact]
		public void FontGammaTest()
		{
			config.FontGamma = 1.8;
			Assert.Equal(1.8, config.FontGamma);
			config.FontGamma = 2.2;
			Assert.Equal(2.2, config.FontGamma);
		}
		[Fact]
		public void UserStylesheetTest()
		{
			config.UserStylesheet = "style";
			Assert.Equal("style", config.UserStylesheet);
			config.UserStylesheet = "not style";
			Assert.Equal("not style", config.UserStylesheet);
		}
		[Fact]
		public void ForceRepaintTest()
		{
			config.ForceRepaint = true;
			Assert.True(config.ForceRepaint);
			config.ForceRepaint = false;
			Assert.False(config.ForceRepaint);
		}
		[Fact]
		public void AnimationTimerDelayTest()
		{
			config.AnimationTimerDelay = 1.0 / 60.0;
			Assert.Equal(1.0 / 60.0, config.AnimationTimerDelay);
			config.AnimationTimerDelay = 1.0 / 120.0;
			Assert.Equal(1.0 / 120.0, config.AnimationTimerDelay);
		}
		[Fact]
		public void ScrollTimerDelayTest()
		{
			config.ScrollTimerDelay = 1.0 / 60.0;
			Assert.Equal(1.0 / 60.0, config.ScrollTimerDelay);
			config.ScrollTimerDelay = 1.0 / 120.0;
			Assert.Equal(1.0 / 120.0, config.ScrollTimerDelay);
		}
		[Fact]
		public void RecycleDelayTest()
		{
			config.RecycleDelay = 4.0;
			Assert.Equal(4.0, config.RecycleDelay);
			config.RecycleDelay = 1.0;
			Assert.Equal(1.0, config.RecycleDelay);
		}
		[Fact]
		public void MemoryCacheSizeTest()
		{
			config.MemoryCacheSize = 64u * 1024u * 1024u;
			Assert.Equal(64u * 1024u * 1024u, config.MemoryCacheSize);
			config.MemoryCacheSize = 128u * 1024u * 1024u;
			Assert.Equal(128u * 1024u * 1024u, config.MemoryCacheSize);
		}
		[Fact]
		public void PageCacheSizeTest()
		{
			config.PageCacheSize = 0u;
			Assert.Equal(0u, config.PageCacheSize);
			config.PageCacheSize = 5u;
			Assert.Equal(5u, config.PageCacheSize);
		}
		[Fact]
		public void OverrideRAMSize()
		{
			config.OverrideRAMSize = 0u;
			Assert.Equal(0u, config.OverrideRAMSize);
			config.OverrideRAMSize = 1024u;
			Assert.Equal(1024u, config.OverrideRAMSize);
		}
		[Fact]
		public void MinLargeHeapSizeTest()
		{
			config.MinLargeHeapSize = 32u * 1024u * 1024u;
			Assert.Equal(32u * 1024u * 1024u, config.MinLargeHeapSize);
			config.MinLargeHeapSize = 8u * 1024u * 1024u;
			Assert.Equal(8u * 1024u * 1024u, config.MinLargeHeapSize);
		}
		[Fact]
		public void MinSmallHeapSizeTest()
		{
			config.MinSmallHeapSize = 1u * 1024u * 1024u;
			Assert.Equal(1u * 1024u * 1024u, config.MinSmallHeapSize);
			config.MinSmallHeapSize = 512u * 1024u;
			Assert.Equal(512u * 1024u, config.MinSmallHeapSize);
		}
		[Fact]
		public void DisposeTest()
		{
			config.Dispose();
			config.Dispose();
		}
		[Fact]
		public void FinalizeTest()
		{
			config = null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}
	}
}
