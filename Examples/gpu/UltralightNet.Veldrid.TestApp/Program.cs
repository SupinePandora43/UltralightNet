using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using TerraFX.Interop.Windows;
using UltralightNet.AppCore;
using UltralightNet.GPU.Veldrid;
using UltralightNet.Veldrid.SDL2;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

namespace UltralightNet.Veldrid.TestApp
{
	class Program
	{
		private const GraphicsBackend BACKEND = GraphicsBackend.Vulkan;
		private const bool WaitForLoad = false;

		private const uint Width = 512;
		private const uint Height = 512;

		private static readonly ULConfig config = new()
		{
			ForceRepaint = true,
			CachePath = "./cache/",
			BitmapAlignment = 1, // improves performance (veldrid only)
			FaceWinding = ULFaceWinding.CounterClockwise
		};
		private static ULViewConfig viewConfig = new()
		{
			IsAccelerated = true,
			IsTransparent = false
		};

		private static float scale = 1;

		[DllImport("Ultralight", EntryPoint = "?GetKeyIdentifierFromVirtualKeyCode@ultralight@@YAXHAEAVString@1@@Z")]
		private static extern void GetKey(int i, IntPtr id);

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		public unsafe static void Main()
		{
			Stopwatch framerateStopwatch = new();

			static bool EnableDPIAwareness()
			{
				if (OperatingSystem.IsWindowsVersionAtLeast(8, 1))
					Windows.SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE);
				else if (OperatingSystem.IsWindowsVersionAtLeast(6))
					Windows.SetProcessDPIAware();
				else return false;
				return true;
			}

			if (EnableDPIAwareness())
			{
				HMONITOR monitor = Windows.MonitorFromPoint(new TerraFX.Interop.Windows.POINT(1, 1), MONITOR.MONITOR_DEFAULTTONEAREST);
				if (OperatingSystem.IsWindowsVersionAtLeast(8, 1))
				{
					try
					{
						DEVICE_SCALE_FACTOR factor;
						Windows.GetScaleFactorForMonitor(monitor, &factor);
						scale = (float)factor / 100f;
					}
					catch
					{
						uint dpiX, dpiY;
						Windows.GetDpiForMonitor(monitor, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, &dpiX, &dpiY);
						scale = ((float)(dpiX + dpiY) / 2) / 96f;
					}
				}
				else if (OperatingSystem.IsWindows())
				{
					HDC dc = Windows.GetDC(HWND.NULL);
					int dpiX = Windows.GetDeviceCaps(dc, Windows.LOGPIXELSX), dpiY = Windows.GetDeviceCaps(dc, Windows.LOGPIXELSY);
					scale = ((float)(dpiX + dpiY) / 2) / 96f;
					Windows.ReleaseDC(HWND.NULL, dc);
				}
			}

			viewConfig.InitialDeviceScale = scale;

			WindowCreateInfo windowCI = new()
			{
				WindowWidth = (int)(Width * scale),
				WindowHeight = (int)(Height * scale),
				WindowTitle = "UltralightNet.Veldrid.TestApp",
				X = 100,
				Y = 100
			};

			Sdl2Window window = VeldridStartup.CreateWindow(ref windowCI);

			GraphicsDeviceOptions options = new()
			{
				PreferStandardClipSpaceYDirection = true,
				PreferDepthRangeZeroToOne = true,
				SwapchainSrgbFormat = false
			};

			GraphicsDevice graphicsDevice = VeldridStartup.CreateGraphicsDevice(
				window,
				options,
				BACKEND);
			ResourceFactory factory = graphicsDevice.ResourceFactory;

			CommandList commandList = factory.CreateCommandList();

			ResourceLayout basicQuadResourceLayout = factory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("_texture",
						ResourceKind.TextureReadOnly,
						ShaderStages.Fragment
					)//,
				/*new ResourceLayoutElementDescription("_texture",
					ResourceKind.TextureReadOnly,
					ShaderStages.Fragment
				)*/
				)
			);

			GraphicsPipelineDescription mainPipelineDescription = new(
				BlendStateDescription.SingleAlphaBlend,
				new DepthStencilStateDescription(
					depthTestEnabled: false,
					depthWriteEnabled: false,
					comparisonKind: ComparisonKind.Never),
				new RasterizerStateDescription(
					cullMode: FaceCullMode.None,
					fillMode: PolygonFillMode.Solid,
					frontFace: FrontFace.Clockwise,
					depthClipEnabled: false,
					scissorTestEnabled: false),
				PrimitiveTopology.TriangleList,
				new ShaderSetDescription(
					new VertexLayoutDescription[] {
						new VertexLayoutDescription(
							new VertexElementDescription(
								"in_pos",
								VertexElementSemantic.TextureCoordinate,
								VertexElementFormat.Float2
							),
							new VertexElementDescription(
								"in_uv",
								VertexElementSemantic.TextureCoordinate,
								VertexElementFormat.Float2
							)
						)
					}, factory.CreateFromSpirv(new(ShaderStages.Vertex, Encoding.UTF8.GetBytes(@"
#version 450
precision highp float;

layout(location = 0) in vec2 in_pos;
layout(location = 1) in vec2 in_uv;

layout(location = 0) out vec2 out_uv;

void main()
{
    //gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
	gl_Position = vec4(in_pos, 0, 1);
	//fUv = 0.5 * vPos.xy + vec2(0.5,0.5);
	out_uv = in_uv;
}
"), "main"),
new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(@"
#version 450
precision highp float;

layout(set=0, binding = 0) uniform sampler2D _texture;
//layout(binding = 0) uniform sampler _sampler;
//layout(binding = 1) uniform texture2D _texture;

layout(location = 0) in vec2 out_uv;

layout(location = 0) out vec4 out_Color;

void main()
{
	//out_Color = texture(sampler2D(_texture, _sampler), out_uv);
	out_Color = texture(_texture, out_uv);
}
"), "main"))
				),
				new ResourceLayout[] {
					basicQuadResourceLayout
				},
				graphicsDevice.SwapchainFramebuffer.OutputDescription
			);

			Pipeline pipeline = factory.CreateGraphicsPipeline(ref mainPipelineDescription);


			AppCoreMethods.SetPlatformFontLoader();
			GPUDriver gpuDriver = new(graphicsDevice) { CommandList = commandList };
			SurfaceDefinition surfaceDefinition = new(graphicsDevice) { CommandList = commandList };
			ULPlatform.GPUDriver = gpuDriver;
			ULPlatform.SurfaceDefinition = surfaceDefinition;


			Renderer renderer = ULPlatform.CreateRenderer(config);
			using View view = renderer.CreateView((uint)(Width * scale), (uint)(Height * scale), viewConfig, renderer.CreateSession(true, "Cookies_please"));
			using View cpuView = renderer.CreateView((uint)(Width * scale), (uint)(Height * scale), viewConfig with { IsAccelerated = false }, renderer.CreateSession(true, "Cookies_please"));

			const string url = "https://en.key-test.ru/";//*/"https://github.com/SupinePandora43";

			view.URL = url;

			//view.HTML = "<html><body><p>123</p></body></html>";
			//view.URL = "https://github.com";
			//view.URL = "https://youtu.be/YNL692WN6EE";
			cpuView.URL = url;

			/*try
			{
				WebRequest request = WebRequest.CreateHttp("https://raw.githubusercontent.com/SupinePandora43/UltralightNet/ulPath_pipelines/SilkNetSandbox/assets/index.html");

				var response = request.GetResponse();
				var responseStream = response.GetResponseStream();
				StreamReader reader = new(responseStream);
				string htmlText = reader.ReadToEnd();
			}
			finally { }*/

			//view.HTML = "<html><body><p>123</p></body></html>";

			//view.HTML = htmlText;
			//cpuView.HTML = htmlText;

			/*Texture cpuTexture = null;
			ResourceSet cpuTextureResourceSet = null;

			void CreateCPUTexture()
			{
				cpuTexture = factory.CreateTexture(new TextureDescription(cpuView.Width, cpuView.Height, 1, 1, 1, PixelFormat.B8_G8_R8_A8_UNorm, TextureUsage.Sampled, TextureType.Texture2D));
				cpuTextureResourceSet = factory.CreateResourceSet(new ResourceSetDescription(basicQuadResourceLayout, cpuTexture));
			}

			CreateCPUTexture();
			*/
			int x = 0;
			int y = 0;

			bool cpu = false;

			window.MouseMove += (mm) =>
			{
				x = (int)mm.MousePosition.X;
				y = (int)mm.MousePosition.Y;

				ULMouseEvent mouseEvent = new()
				{
					Type = ULMouseEventType.MouseMoved,
					X = (int)(x / scale),
					Y = (int)(y / scale),
					Button = ULMouseEventButton.None
				};

				view.FireMouseEvent(mouseEvent);
				//cpuView.FireMouseEvent(mouseEvent);
			};
			window.MouseDown += (md) =>
			{
				Console.WriteLine($"Mouse Down {md.Down} {md.MouseButton}");
				if (md.MouseButton is MouseButton.Right) cpu = !cpu;
				if (md.MouseButton is MouseButton.Button1)
				{
					view.GoBack();
					//cpuView.GoBack();
				}
				else if (md.MouseButton is MouseButton.Button2)
				{
					view.GoForward();
					//cpuView.GoForward();
				}
				ULMouseEvent mouseEvent = new()
				{
					Type = ULMouseEventType.MouseDown,
					X = (int)(x / scale),
					Y = (int)(y / scale),
					Button = md.MouseButton switch
					{
						MouseButton.Left => ULMouseEventButton.Left,
						MouseButton.Right => ULMouseEventButton.Right,
						MouseButton.Middle => ULMouseEventButton.Middle,
						_ => ULMouseEventButton.None
					}
				};
				view.FireMouseEvent(mouseEvent);
				//cpuView.FireMouseEvent(mouseEvent);
			};
			window.MouseUp += (mu) =>
			{
				Console.WriteLine($"Mouse up {mu.Down} {mu.MouseButton}");
				ULMouseEvent mouseEvent = new()
				{
					Type = ULMouseEventType.MouseUp,
					X = (int)(x / scale),
					Y = (int)(y / scale),
					Button = mu.MouseButton switch
					{
						MouseButton.Left => ULMouseEventButton.Left,
						MouseButton.Right => ULMouseEventButton.Right,
						MouseButton.Middle => ULMouseEventButton.Middle,
						_ => ULMouseEventButton.None
					}
				};
				view.FireMouseEvent(mouseEvent);
				//cpuView.FireMouseEvent(mouseEvent);
			};
			window.MouseWheel += (mw) =>
			{
				Console.WriteLine(mw.WheelDelta);
				ULScrollEvent scrollEvent = new()
				{
					Type = ULScrollEventType.ByPixel,
					DeltaY = (int)(mw.WheelDelta * 32f)//16//(int)((mw.WheelDelta / scale)*0.8) // PixelsToScreen(Delta) * 0.8 from AppCore/win/WindowWin.cpp(h)
				};
				view.FireScrollEvent(scrollEvent);
				//cpuView.FireScrollEvent(scrollEvent);
			};

			window.KeyDown += (kd) =>
			{
				view.FireKeyEvent(kd.ToULKeyEvent());
				//cpuView.FireKeyEvent(kd.ToULKeyEvent());
			};

			window.KeyUp += (ku) =>
			{
				view.FireKeyEvent(ku.ToULKeyEvent());
				//cpuView.FireKeyEvent(ku.ToULKeyEvent());
			};

			window.Resized += () =>
			{
				graphicsDevice.ResizeMainWindow((uint)window.Width, (uint)window.Height);
				view.Resize((uint)window.Width, (uint)window.Height);
				//cpuView.Resize((uint)window.Width, (uint)window.Height);
				//CreateCPUTexture();
			};
			DeviceBuffer quadV = factory.CreateBuffer(new(4 * 4 * 4, BufferUsage.VertexBuffer));
			graphicsDevice.UpdateBuffer(quadV, 0, new Vector4[]
			{
				new(-1, 1f, 0, 0 ),
				new(1, 1, 1, 0 ),
				new(-1, -1, 0, 1 ),
				new(1, -1, 1, 1 ),
			});

			DeviceBuffer quadI = factory.CreateBuffer(new(sizeof(short) * 6, BufferUsage.IndexBuffer));
			graphicsDevice.UpdateBuffer(quadI, 0, new short[] { 0, 1, 2, 3, 2, 1 });

			Stopwatch stopwatch = new();
			stopwatch.Start();


			view.OnDOMReady += ((frame_id, is_main_frame, url) =>
			{
				Console.WriteLine("Dom is ready");

				//view.EvaluateScript("window.location = \"https://heeeeeeeey.com/\"", out string exception);
			});

			uint frame_of_second = 0;

			framerateStopwatch.Restart();

			uint fps = 0;

			while (window.Exists)
			{
				if (framerateStopwatch.ElapsedMilliseconds / 1000 >= 1)
				{
					fps = frame_of_second;
					//Console.WriteLine(fps);
					frame_of_second = 0;
					framerateStopwatch.Restart();
				}

				commandList.Begin();
				//Console.WriteLine("Update");
				renderer.Update();
				//Console.WriteLine("Render");
				renderer.Render();
				//Console.WriteLine("Done");

				/*ULSurface surface = cpuView.Surface;
				ULBitmap bitmap = surface.Bitmap;

				ULIntRect dirty = surface.DirtyBounds;

				IntPtr pixels = bitmap.LockPixels();
				uint rowBytes = bitmap.RowBytes;
				uint height = bitmap.Height;
				uint width = bitmap.Width;
				uint bpp = bitmap.Bpp;
				if (rowBytes == width * bpp)
				{
					graphicsDevice.UpdateTexture(cpuTexture, pixels, (uint)bitmap.Size, 0, 0, 0, width, height, 1, 0, 0);
				}
				else
				{
					Console.WriteLine("stride");
					graphicsDevice.UpdateTexture(cpuTexture, Unstrider.Unstride(pixels, width, cpuView.Height, bpp, rowBytes - (width * bpp)), 0, 0, 0, width, height, 1, 0, 0);
				}
				surface.ClearDirtyBounds();
				bitmap.UnlockPixels();
				*/
				//gpuDriver.Render();


				commandList.SetPipeline(pipeline);

				commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
				commandList.SetFullViewports();
				//commandList.ClearColorTarget(0, new RgbaFloat(MathF.Sin(stopwatch.Elapsed.Milliseconds / 100f), 255, 0, 255));
				//commandList.ClearColorTarget(0, RgbaFloat.Blue);

				/*if (TRANSPARENT)*/ //commandList.ClearColorTarget(0, RgbaFloat.Orange);

				commandList.SetVertexBuffer(0, quadV);
				commandList.SetIndexBuffer(quadI, IndexFormat.UInt16);

				//if (cpu)
				//{
				//commandList.SetGraphicsResourceSet(0, cpuTextureResourceSet);
				//}
				//else
				commandList.SetGraphicsResourceSet(0, gpuDriver.GetRenderTarget(view));

				commandList.DrawIndexed(
					6,
					1,
					0,
					0,
					0
				);

				commandList.End();

				graphicsDevice.SubmitCommands(commandList);
				graphicsDevice.SwapBuffers();
				window.PumpEvents();

				frame_of_second++;

				// Thread.Sleep(1000 / 60 / 10); // ~600 fps
			}

			renderer.Dispose();
		}
	}
}
