using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UltralightNet.AppCore;
using UltralightNet.Veldrid.SDL2;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

namespace UltralightNet.Veldrid.TestApp
{
	class Program
	{
		private const GraphicsBackend BACKEND = GraphicsBackend.Direct3D11;
		private const bool WaitForLoad = false;

		private const uint Width = 1024;
		private const uint Height = 512;

		private static readonly ULConfig config = new()
		{
			ForceRepaint = false,
			CachePath = "./cache/",
			//BitmapAlignment = 0 // improves performance
			FaceWinding = ULFaceWinding.Clockwise
		};
		private static readonly ULViewConfig viewConfig = new()
		{
			IsAccelerated = true,
			IsTransparent = false
		};

		[DllImport("Ultralight", EntryPoint = "?GetKeyIdentifierFromVirtualKeyCode@ultralight@@YAXHAEAVString@1@@Z")]
		private static extern void GetKey(int i, IntPtr id);

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		public static void Main()
		{
			Stopwatch framerateStopwatch = new();
			WindowCreateInfo windowCI = new()
			{
				WindowWidth = (int)Width,
				WindowHeight = (int)Height,
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

			VeldridGPUDriver gpuDriver = new(graphicsDevice);

			gpuDriver.CommandList = commandList;

			AppCoreMethods.ulEnablePlatformFileSystem("./");
			ULPlatform.Logger = new ULLogger() { LogMessage = (ULLogLevel lvl, in string msg) => Console.WriteLine(msg) };
			AppCoreMethods.ulEnablePlatformFontLoader();
			ULPlatform.GPUDriver = gpuDriver.GetGPUDriver();

			Renderer renderer = ULPlatform.CreateRenderer(config);
			Session session = renderer.CreateSession(true, "Cookies_please");
			View view = renderer.CreateView(Width, Height, viewConfig, session);

			//View cpuView = new(renderer, Width, Height, TRANSPARENT, Session.DefaultSession(renderer), true);

			const string url = "https://en.key-test.ru/";//*/"https://github.com/SupinePandora43";

			//view.URL = url;

			//view.HTML = "<html><body><p>123</p></body></html>";
			//view.URL = "https://github.com";
			view.URL = "https://youtu.be/YNL692WN6EE";
			//cpuView.URL = url;

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
					X = x,
					Y = y,
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
					X = x,
					Y = y,
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
					X = x,
					Y = y,
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
				ULScrollEvent scrollEvent = new()
				{
					Type = ULScrollEventType.ByPixel,
					DeltaY = (int)mw.WheelDelta * 100
				};
				view.FireScrollEvent(scrollEvent);
				//cpuView.FireScrollEvent(scrollEvent);
			};

			// idk why
			view.Focus();
			//cpuView.Focus();

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


			view.OnDomReady += ((frame_id, is_main_frame, url) =>
			{
				Console.WriteLine("Dom is ready");

				//view.EvaluateScript("window.location = \"https://heeeeeeeey.com/\"", out string exception);
			});

			IntPtr rendererPtr = renderer.Ptr;

			uint frame_of_second = 0;

			framerateStopwatch.Restart();

			uint fps = 0;

			while (window.Exists)
			{
				if (framerateStopwatch.ElapsedMilliseconds / 1000 >= 1)
				{
					fps = frame_of_second;
					Console.WriteLine(fps);
					frame_of_second = 0;
					framerateStopwatch.Restart();
				}

				commandList.Begin();
				//Console.WriteLine("Update");
				Methods.ulUpdate(rendererPtr);
				gpuDriver.time = stopwatch.ElapsedTicks / 1000f;
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

			GC.KeepAlive(session);
			renderer.Dispose();
		}
	}
}
