using System;
using System.Threading;
using UltralightNet.AppCore;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace UltralightNet.Veldrid.TestApp
{
	class Program
	{
		private const GraphicsBackend BACKEND = GraphicsBackend.Direct3D11;

		static void Main(string[] args)
		{
			new Program().Run();
		}

		private void Run()
		{
			WindowCreateInfo windowCI = new()
			{
				WindowWidth = 512,
				WindowHeight = 512,
				WindowTitle = "UltralightNet.Veldrid.TestApp"
			};

			Sdl2Window window = VeldridStartup.CreateWindow(ref windowCI);
			//todo
			window.Resizable = false;

			GraphicsDeviceOptions options = new()
			{
				PreferStandardClipSpaceYDirection = true,
				PreferDepthRangeZeroToOne = true
			};

			GraphicsDevice graphicsDevice = VeldridStartup.CreateGraphicsDevice(
				window,
				options,
				BACKEND);

			ResourceFactory factory = graphicsDevice.ResourceFactory;

			VeldridGPUDriver gpuDriver = new(graphicsDevice);

			ULPlatform.SetLogger(new() { log_message = (lvl, msg) => Console.WriteLine(msg) }); ;
			AppCoreMethods.ulEnablePlatformFontLoader();
			ULPlatform.SetGPUDriver(gpuDriver.GetGPUDriver());

			Renderer renderer = new(new ULConfig()
			{
				ResourcePath = "./resources/",
				UseGpu = true
			});

			View view = new(renderer, 512, 512, false, Session.DefaultSession(renderer), false);

			view.URL = "https://github.com";
			bool loaded = false;
			view.SetFinishLoadingCallback((user_data, caller, frame_id, is_main_frame, url) => {
				loaded = true;
			});
			while (!loaded)
			{
				renderer.Update();
				Thread.Sleep(10);
			}

			window.MouseWheel += (mw) =>
			{
				view.FireScrollEvent(new ULScrollEvent()
				{
					type = ULScrollEvent.Type.ByPage,
					deltaY = (int)mw.WheelDelta
				});
			};

			while (window.Exists)
			{
				renderer.Update();
				renderer.Render();
				window.PumpEvents();
			}
		}
	}
}
