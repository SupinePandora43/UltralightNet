using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading;
using UltralightNet.AppCore;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

namespace UltralightNet.Veldrid.TestApp
{
	class Program
	{
		private const GraphicsBackend BACKEND = GraphicsBackend.Vulkan;

		public static void Main()
		{
			WindowCreateInfo windowCI = new()
			{
				WindowWidth = 512,
				WindowHeight = 512,
				WindowTitle = "UltralightNet.Veldrid.TestApp",
				X = 100,
				Y = 100
			};

			Sdl2Window window = VeldridStartup.CreateWindow(ref windowCI);

			GraphicsDeviceOptions options = new()
			{
				PreferStandardClipSpaceYDirection = true,
				PreferDepthRangeZeroToOne = true,
				SwapchainSrgbFormat = true,
			};

			GraphicsDevice graphicsDevice = VeldridStartup.CreateGraphicsDevice(
				window,
				options,
				BACKEND);

			ResourceFactory factory = graphicsDevice.ResourceFactory;

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
				BlendStateDescription.SingleOverrideBlend,
				new DepthStencilStateDescription(
					depthTestEnabled: false,
					depthWriteEnabled: false,
					comparisonKind: ComparisonKind.Never),
				new RasterizerStateDescription(
					cullMode: FaceCullMode.Back,
					fillMode: PolygonFillMode.Solid,
					frontFace: FrontFace.Clockwise,
					depthClipEnabled: true,
					scissorTestEnabled: false),
				PrimitiveTopology.TriangleStrip,
				new ShaderSetDescription(
					new VertexLayoutDescription[] {
						new VertexLayoutDescription(
							new VertexElementDescription(
								"vPos",
								VertexElementSemantic.TextureCoordinate,
								VertexElementFormat.Float2
							),
							new VertexElementDescription(
								"fUv",
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

			Pipeline pipeline = factory.CreateGraphicsPipeline(mainPipelineDescription);

			VeldridGPUDriver gpuDriver = new(graphicsDevice);

			ULPlatform.SetLogger(new() { log_message = (lvl, msg) => Console.WriteLine(msg) }); ;
			AppCoreMethods.ulEnablePlatformFontLoader();
			ULPlatform.SetGPUDriver(gpuDriver.GetGPUDriver());

			Renderer renderer = new(new ULConfig()
			{
				ResourcePath = "./resources/",
				UseGpu = true,
				ForceRepaint = false
			});

			View view = new(renderer, 512, 512, false, Session.DefaultSession(renderer), false);

			view.URL = "https://github.com/SupinePandora43";
			bool loaded = false;
			view.SetFinishLoadingCallback((user_data, caller, frame_id, is_main_frame, url) =>
			{
				loaded = true;
			});
			while (!loaded)
			{
				renderer.Update();
				Thread.Sleep(10);
			}

			WebRequest request = WebRequest.CreateHttp("https://raw.githubusercontent.com/SupinePandora43/UltralightNet/ulPath_pipelines/SilkNetSandbox/assets/index.html");

			var response = request.GetResponse();
			var responseStream = response.GetResponseStream();
			StreamReader reader = new(responseStream);
			//view.HTML = reader.ReadToEnd();

			int x = 0;
			int y = 0;

			window.MouseMove += (mm) =>
			{
				x = (int)mm.MousePosition.X;
				y = (int)mm.MousePosition.Y;
				view.FireMouseEvent(new ULMouseEvent()
				{
					button = ULMouseEvent.Button.None,
					type = ULMouseEvent.Type.MouseMoved,
					x = x,
					y = y
				});
			};
			window.MouseDown += (md) =>
			{
				Console.WriteLine($"Mouse Down {md.Down} {md.MouseButton}");
				view.FireMouseEvent(new ULMouseEvent()
				{
					button = md.MouseButton == MouseButton.Left ? ULMouseEvent.Button.Left : ULMouseEvent.Button.Right,
					type = ULMouseEvent.Type.MouseDown,
					x = x,
					y = y
				});
			};
			window.MouseUp += (mu) =>
			{
				Console.WriteLine($"Mouse up {mu.Down} {mu.MouseButton}");
				view.FireMouseEvent(new ULMouseEvent()
				{
					button = mu.MouseButton == MouseButton.Left ? ULMouseEvent.Button.Left : ULMouseEvent.Button.Right,
					type = ULMouseEvent.Type.MouseUp,
					x = x,
					y = y
				});
			};
			window.MouseWheel += (mw) =>
			{
				view.FireScrollEvent(new ULScrollEvent()
				{
					type = ULScrollEvent.Type.ByPixel,
					deltaY = (int)mw.WheelDelta * 100
				});
			};
			window.Resized += () =>
			{
				graphicsDevice.ResizeMainWindow((uint)window.Width, (uint)window.Height);
				view.Resize((uint)window.Width, (uint)window.Height);
			};
			DeviceBuffer quadV = factory.CreateBuffer(new(4 * 4 * 4, BufferUsage.VertexBuffer));
			graphicsDevice.UpdateBuffer(quadV, 0, new Vector4[]
			{
				new(-1, 1f, 0, 0 ),
				new(1, 1, 1, 0 ),
				new(-1, -1, 0, 1 ),
				new(1, -1, 1, 1 ),
			});

			DeviceBuffer quadI = factory.CreateBuffer(new(sizeof(short) * 4, BufferUsage.IndexBuffer));
			graphicsDevice.UpdateBuffer(quadI, 0, new short[] { 0, 1, 2, 3 });

			CommandList commandList = factory.CreateCommandList();
			Stopwatch stopwatch = new();
			stopwatch.Start();
			while (window.Exists)
			{
				renderer.Update();
				renderer.Render();
				gpuDriver.Render(stopwatch.ElapsedTicks / 1000f);

				commandList.Begin();

				commandList.SetPipeline(pipeline);

				commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
				commandList.SetFullViewports();
				commandList.ClearColorTarget(0, new RgbaFloat(MathF.Sin(stopwatch.Elapsed.Milliseconds / 100f), 255, 0, 255));
				//commandList.ClearColorTarget(0, RgbaFloat.Blue);

				commandList.SetVertexBuffer(0, quadV);
				commandList.SetIndexBuffer(quadI, IndexFormat.UInt16);

				commandList.SetGraphicsResourceSet(0, gpuDriver.GetRenderTarget(view));

				commandList.DrawIndexed(
					4,
					1,
					0,
					0,
					0
				);

				commandList.End();

				graphicsDevice.SubmitCommands(commandList);
				graphicsDevice.SwapBuffers();
				window.PumpEvents();
			}
		}
	}
}
