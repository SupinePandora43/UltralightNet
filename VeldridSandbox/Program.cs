using Supine.UltralightSharp.Enums;
using Supine.UltralightSharp.Safe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

namespace VeldridSandbox
{
	class Program
	{
		public static void Main()
		{
			Ultralight.SetLogger(new Logger
			{
				LogMessage = (LogLevel logLevel, string msg) =>
				{
					switch (logLevel)
					{
						case LogLevel.Error:
						case LogLevel.Warning:
							Console.Error.WriteLine(msg);
							break;
						case LogLevel.Info:
						default:
							Console.WriteLine(msg);
							break;
					}
				}
			});

			AppCore.EnablePlatformFontLoader();
			AppCore.EnablePlatformFileSystem("./");
			Program program = new();
			program.Run();
		}
		private int width = 512;
		private int height = 512;

		private Stopwatch stopwatch;

		private Renderer renderer;
		private View view;

		private GraphicsDevice graphicsDevice;
		private ResourceFactory factory;

		private Veldrid.CommandList commandList;
		private DeviceBuffer _vertexBuffer;
		private DeviceBuffer _indexBuffer;
		private Shader[] _shaders;
		private Pipeline _pipeline;
		private ResourceSet resourceSet;

		private Queue<Command> queuedCommands = new Queue<Command>();
		private DeviceBuffer uniformBuffer;

		public class GeometryEntry
		{
			//public Vector3[] VertexArray { get; set; }
			public DeviceBuffer VertexBuffer { get; set; }
			public DeviceBuffer IndicesBuffer { get; set; }
		}
		public class RenderBufferEntry
		{
			public Framebuffer FrameBuffer { get; set; }
			public TextureEntry TextureEntry { get; set; } = null!;
		}

		public class TextureEntry
		{
			public Texture Texure { get; set; }
			public uint Width { get; set; }
			public uint Height { get; set; }
		}

		private static readonly SlottingList<GeometryEntry> GeometryEntries = new(32, 8);
		private static readonly SlottingList<TextureEntry> TextureEntries = new(32, 8);
		private static readonly SlottingList<RenderBufferEntry> RenderBufferEntries = new(8, 2);

		public Program()
		{
			GpuDriver gpuDriver = new()
			{
				BeginSynchronize = BeginSynchronize,
				EndSynchronize = EndSynchronize,
				NextGeometryId = NextGeometryId,
				CreateGeometry = CreateGeometry,
				UpdateGeometry = UpdateGeometry,
				DestroyGeometry = DestroyGeometry,
				NextRenderBufferId = NextRenderBufferId,
				CreateRenderBuffer = CreateRenderBuffer,
				DestroyRenderBuffer = DestroyRenderBuffer,
				NextTextureId = NextTextureId,
				CreateTexture = CreateTexture,
				UpdateTexture = UpdateTexture,
				DestroyTexture = DestroyTexture,
				UpdateCommandList = UpdateCommandList
			};
			Ultralight.SetGpuDriver(gpuDriver);

			Config cfg = new();
			cfg.SetResourcePath("resources");
			cfg.SetUseGpuRenderer(true);
			renderer = new(cfg);
			Session session = renderer.GetDefaultSession();
			view = new(renderer, 512, 512, false, session, false);

			view.LoadUrl("https://github.com");
		}

		private void UpdateCommandList(Supine.UltralightSharp.Safe.CommandList list)
		{
			Console.WriteLine("GpuDriver.UpdateCommandList(LIST)");
			foreach (var cmd in list)
				queuedCommands.Enqueue(cmd);
		}

		private void DestroyTexture(uint textureId)
		{
			Console.WriteLine($"GpuDriver.DestroyTexture({textureId})");
			var index = (int)textureId - 1;
			var entry = TextureEntries.RemoveAt(index);

			entry.Texure.Dispose();
		}

		private void UpdateTexture(uint textureId, Bitmap bitmap)
		{
			Console.WriteLine($"GpuDriver.UpdateTexture: {textureId}");
			var index = (int)textureId - 1;
			var entry = TextureEntries[index];

			var tex = entry.Texure;
			var texWidth = entry.Width;
			var texHeight = entry.Height;

			//var pixels = bitmap.LockPixels();

			var format = bitmap.GetFormat();
			/*switch (format)
			{
				case BitmapFormat.A8UNorm:
					{
						graphicsDevice.UpdateTexture(tex, bitmap.LockPixels(),
							texWidth * texHeight * 4,
							0,
							0,
							0,
							texWidth,
							texHeight,
							1,
							0,
							0
						);
						break;
					}
				case BitmapFormat.Bgra8UNormSrgb:
					{
						_gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Srgb8Alpha8, texWidth, texHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, (void*)pixels);
						break;
					}
				default: throw new ArgumentOutOfRangeException(nameof(BitmapFormat));
			}*/

		}

		private void CreateTexture(uint textureId, Bitmap bitmap)
		{
			var index = (int)textureId - 1;
			var entry = TextureEntries[index];
			var texWidth = bitmap.GetWidth();
			entry.Width = texWidth;
			var texHeight = bitmap.GetHeight();
			entry.Height = texHeight;
			Console.WriteLine($"GpuDriver.CreateTexture({textureId} {texWidth}x{texHeight})");

			if (bitmap.IsEmpty())
			{
				entry.Texure = factory.CreateTexture(
					TextureDescription.Texture2D(texWidth,
						texHeight,
						1,
						1,
						PixelFormat.R8_G8_B8_A8_UInt,
						TextureUsage.RenderTarget | TextureUsage.Sampled | TextureUsage.Storage
					)
				);
				Console.WriteLine("Bitmap.IsEmtpy()==true");
			}
			else
			{
				entry.Texure = factory.CreateTexture(
					TextureDescription.Texture2D(
						texWidth,
						texHeight,
						1,
						1,
						PixelFormat.R8_G8_B8_A8_UInt,
						TextureUsage.RenderTarget | TextureUsage.Sampled | TextureUsage.Storage
					)
				);
				Console.WriteLine(texWidth * texHeight * 4);
				graphicsDevice.UpdateTexture(entry.Texure,
					bitmap.LockPixels(),
					texWidth * texHeight * 4,
					0,
					0,
					0,
					texWidth,
					texHeight,
					1,
					0,
					0);
				bitmap.UnlockPixels();
			}
		}

		private uint NextTextureId()
		{
			var id = TextureEntries.Add(new TextureEntry()) + 1;
			Console.WriteLine($"GpuDriver.NextTextureId() = {id}");
			return (uint)id;
		}

		private void DestroyRenderBuffer(uint renderBufferId)
		{
			Console.WriteLine($"GpuDriver.DestroyRenderBuffer({renderBufferId})");
			var index = (int)renderBufferId - 1;
			var entry = RenderBufferEntries.RemoveAt(index);

			entry.FrameBuffer.Dispose();
		}

		private void CreateRenderBuffer(uint renderBufferId, RenderBuffer buffer)
		{

			Console.WriteLine($"CreateRenderBuffer: {renderBufferId}");

			var index = (int)renderBufferId - 1;
			var entry = RenderBufferEntries[index];

			var texIndex = (int)buffer.TextureId - 1;
			var texEntry = TextureEntries[texIndex];
			var tex = texEntry.Texure;

			/*Texture offscreenDepth = factory.CreateTexture(TextureDescription.Texture2D(
				tex.Width, tex.Height, 1, 1, PixelFormat.R16_UNorm, TextureUsage.DepthStencil));

			FramebufferDescription framebufferDescription = new(offscreenDepth, tex);
			
			entry.FrameBuffer = factory.CreateFramebuffer(framebufferDescription);
			*/
			entry.FrameBuffer = FrameBufferHelper.CreateFramebuffer(
				graphicsDevice,
				tex.Width,
				tex.Height,
				PixelFormat.R8_G8_B8_A8_UInt);
			//entry.FrameBuffer.ColorTargets.Append(new(tex, 0));
			entry.TextureEntry = texEntry;
		}

		private uint NextRenderBufferId()
		{
			var id = RenderBufferEntries.Add(new RenderBufferEntry()) + 1;
			Console.WriteLine($"GpuDriver.NextRenderBufferId() = {id}");
			return (uint)id;
		}

		private void DestroyGeometry(uint geometryId)
		{
			Console.WriteLine($"GpuDriver.DestroyGeometry({geometryId})");
			var index = (int)geometryId - 1;
			var entry = GeometryEntries.RemoveAt(index);

			entry.IndicesBuffer.Dispose();
			entry.VertexBuffer.Dispose();
		}

		private unsafe void UpdateGeometry(uint geometryId, VertexBuffer safeVertices, IndexBuffer indices)
		{
			Console.WriteLine($"GpuDriver.UpdateGeometry({geometryId})");
			var index = (int)geometryId - 1;
			var entry = GeometryEntries[index];

			graphicsDevice.UpdateBuffer(entry.VertexBuffer, 0, (IntPtr)safeVertices.AsUnsafe().Data, safeVertices.Size);
			graphicsDevice.UpdateBuffer(entry.IndicesBuffer, 0, (IntPtr)indices.AsUnsafe().Data, indices.Size);

		}

		private unsafe void CreateGeometry(uint geometryId, VertexBuffer safeVertices, IndexBuffer indices)
		{
			Console.WriteLine($"GpuDriver.CreateGeometry: {geometryId}");
			var index = (int)geometryId - 1;
			var entry = GeometryEntries[index];

			entry.VertexBuffer = factory.CreateBuffer(new BufferDescription(safeVertices.Size, BufferUsage.VertexBuffer));
			graphicsDevice.UpdateBuffer(entry.VertexBuffer, 0, (IntPtr)safeVertices.AsUnsafe().Data, safeVertices.Size);

			switch (safeVertices.Format)
			{
				case VertexBufferFormat._2F4Ub2F2F28F:
					{

						break;
					}
				case VertexBufferFormat._2F4Ub2F:
					{

						break;
					}
				default: throw new NotImplementedException(safeVertices.Format.ToString());
			}
			entry.IndicesBuffer = factory.CreateBuffer(new(
				indices.Size,
				BufferUsage.IndexBuffer));
			graphicsDevice.UpdateBuffer(entry.IndicesBuffer, 0, (IntPtr)indices.AsUnsafe().Data, indices.Size);
		}

		private uint NextGeometryId()
		{
			var id = GeometryEntries.Add(new GeometryEntry()) + 1;
			Console.WriteLine($"GpuDriver.NextGeometryId() = {id}");
			return (uint)id;
		}

		private void EndSynchronize() { }
		private void BeginSynchronize() { }

		private readonly Assembly assembly = typeof(Program).Assembly;
		private Shader GetShader(string path, ShaderStages shaderStages)
		{
			Stream resourceStream = assembly.GetManifestResourceStream("VeldridSandbox." + path) ?? throw new FileNotFoundException(path);
			StreamReader resourceStreamReader = new(resourceStream, Encoding.UTF8, false, 16, true);
			string shaderCode = resourceStreamReader.ReadToEnd();
			SpirvCompilationResult spirv = SpirvCompilation.CompileGlslToSpirv(shaderCode, "VertexFill", shaderStages, GlslCompileOptions.Default);
			//ShaderDescription shaderDescription = new(shaderStages, Encoding.UTF8.GetBytes(shaderCode), "main");
			ShaderDescription shaderDescription = new(shaderStages, spirv.SpirvBytes, "main");
			return factory.CreateFromSpirv(shaderDescription);
		}
		private Random random;
		private void Render()
		{
			commandList.Begin();
			commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
			commandList.SetFullViewports();
			commandList.ClearColorTarget(0, RgbaFloat.DarkRed);

			// Update uniforms
			Uniforms uniforms = new()
			{
				State = new(width)
			};
			graphicsDevice.UpdateBuffer(uniformBuffer, 0, uniforms);

			commandList.SetVertexBuffer(0, _vertexBuffer);
			commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
			commandList.SetPipeline(_pipeline);

			commandList.SetGraphicsResourceSet(0, resourceSet);

			commandList.DrawIndexed(
				indexCount: 4,
				instanceCount: 1,
				indexStart: 0,
				vertexOffset: 0,
				instanceStart: 0);

			commandList.End();
			graphicsDevice.SubmitCommands(commandList);
			graphicsDevice.SwapBuffers();
		}
		private const string VertexCode = @"
#version 450

layout(set=0, binding=0) uniform Uniforms {
	uniform vec4 State;
	uniform mat4 Transform;
	uniform vec4 Scalar4[2];
	uniform vec4 Vector[8];
	uniform float fClipSize;
	uniform mat4 Clip[8];
};

layout(location = 0) in vec2 Position;
layout(location = 1) in vec4 Color;
layout(location = 0) out vec4 fsin_Color;
void main()
{
	gl_Position = vec4(Position.x, Position.y - ((State.y - 512)/512), 0, 1);
    //gl_Position = vec4(Position, 0, 1);
    fsin_Color = Color;
}";
		private const string FragmentCode = @"
#version 450
layout(location = 0) in vec4 fsin_Color;
layout(location = 0) out vec4 fsout_Color;
void main()
{
    fsout_Color = fsin_Color;
}";
		struct VertexPositionColor
		{
			public const uint SizeInBytes = 24;
			public Vector2 Position;
			public RgbaFloat Color;
			public VertexPositionColor(Vector2 position, RgbaFloat color)
			{
				Position = position;
				Color = color;
			}
		}
		/// <summary>
		/// Uniforms
		/// </summary>
#pragma warning disable CS0649
		public struct Uniforms
		{
			public Vector4 State; // 16
			public Matrix4x4 Transform; // 64

			public Vector4 Scalar4_0; // 16
			public Vector4 Scalar4_1; // 16

			public Vector4 Vector_0; // 16
			public Vector4 Vector_1; // 16
			public Vector4 Vector_2; // 16
			public Vector4 Vector_3; // 16
			public Vector4 Vector_4; // 16
			public Vector4 Vector_5; // 16
			public Vector4 Vector_6; // 16
			public Vector4 Vector_7; // 16

			public float fClipSize;  // 4

			public Matrix4x4 Clip_0; // 64
			public Matrix4x4 Clip_1; // 64
			public Matrix4x4 Clip_2; // 64
			public Matrix4x4 Clip_3; // 64
			public Matrix4x4 Clip_4; // 64
			public Matrix4x4 Clip_5; // 64
			public Matrix4x4 Clip_6; // 64
			public Matrix4x4 Clip_7; // 64
		}
		private byte[] LoadShaderBytes(string name)
		{
			string extension = graphicsDevice.BackendType switch
			{
				GraphicsBackend.Direct3D11 => "hlsl.bytes",
				GraphicsBackend.Vulkan => "450.glsl",
				GraphicsBackend.OpenGL => "330.glsl",
				GraphicsBackend.Metal => "metallib",
				GraphicsBackend.OpenGLES => "300.glsles",
				_ => throw new InvalidOperationException(),
			};
			return File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory ?? assembly.Location, "Shaders", $"{name}.{extension}"));
		}
		private void Run()
		{
			random = new();
			stopwatch = Stopwatch.StartNew();
			WindowCreateInfo windowCI = new()
			{
				X = 100,
				Y = 100,
				WindowWidth = 512,
				WindowHeight = 512,
				WindowTitle = "Veldrid Ultralight"
			};
			Sdl2Window window = VeldridStartup.CreateWindow(ref windowCI);

			GraphicsDeviceOptions options = new()
			{
				PreferStandardClipSpaceYDirection = true,
				PreferDepthRangeZeroToOne = true
			};
			graphicsDevice = VeldridStartup.CreateGraphicsDevice(
				window,
				options,
				GraphicsBackend.Vulkan);

			window.Resized += () =>
			{
				graphicsDevice.ResizeMainWindow((uint)window.Width, (uint)window.Height);
				width = window.Width;
				height = window.Height;
			};

			factory = graphicsDevice.ResourceFactory;

			#region idk
			VertexPositionColor[] quadVertices =
			{
				new VertexPositionColor(new Vector2(-.75f, .75f), RgbaFloat.Red),
				new VertexPositionColor(new Vector2(.75f, .75f), RgbaFloat.Green),
				new VertexPositionColor(new Vector2(-.75f,- .75f), RgbaFloat.Blue),
				new VertexPositionColor(new Vector2(.75f, -.75f), RgbaFloat.Yellow)
			};

			BufferDescription vbDescription = new(
				4 * VertexPositionColor.SizeInBytes,
				BufferUsage.VertexBuffer);

			_vertexBuffer = factory.CreateBuffer(vbDescription);
			graphicsDevice.UpdateBuffer(_vertexBuffer, 0, quadVertices);
			BufferDescription uniformBufferDescription = new(
				768, // actual size is 756
				BufferUsage.UniformBuffer
			);

			uniformBuffer = factory.CreateBuffer(uniformBufferDescription);
			ushort[] quadIndices = { 0, 1, 2, 3 };
			BufferDescription ibDescription = new(
				4 * sizeof(ushort),
				BufferUsage.IndexBuffer);
			_indexBuffer = factory.CreateBuffer(ibDescription);
			graphicsDevice.UpdateBuffer(_indexBuffer, 0, quadIndices);

			VertexLayoutDescription vertexLayout = new(
				new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
				new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));


			ShaderDescription vertexShaderDesc = new(
				ShaderStages.Vertex,
				Encoding.UTF8.GetBytes(VertexCode),//LoadShaderBytes("FillPath-vertex"),
				"main");
			ShaderDescription fragmentShaderDesc = new(
				ShaderStages.Fragment,
				Encoding.UTF8.GetBytes(FragmentCode),//LoadShaderBytes("FillPath-fragment"),
				"main");

			//_shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);
			#endregion
			Shader vertexShader = GetShader("embedded.shader_v2f_c4f_t2f_t2f_d28f.vert.glsl", ShaderStages.Vertex);
			Shader fragmentShader = GetShader("embedded.shader_fill.frag.glsl", ShaderStages.Fragment);
			Shader pathVertexShader = GetShader("embedded.shader_v2f_c4f_t2f.vert.glsl", ShaderStages.Vertex);
			Shader pathFragmentShader = GetShader("embedded.shader_fill_path.frag.glsl", ShaderStages.Fragment);

			_shaders = new[]
			{
				//GetShader("embedded.shader_v2f_c4f_t2f_t2f_d28f.vert.glsl", ShaderStages.Vertex),
				factory.CreateShader(vertexShaderDesc),
				factory.CreateShader(fragmentShaderDesc),
				/*__shaders[0],
				__shaders[1]*/
				/*vertexShader,
				fragmentShader,
				pathVertexShader,
				pathFragmentShader*/
			};

			// Create pipeline
			GraphicsPipelineDescription pipelineDescription = new();
			pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
			pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
				depthTestEnabled: true,
				depthWriteEnabled: true,
				comparisonKind: ComparisonKind.LessEqual);
			pipelineDescription.RasterizerState = new RasterizerStateDescription(
				cullMode: FaceCullMode.Back,
				fillMode: PolygonFillMode.Solid,
				frontFace: FrontFace.Clockwise,
				depthClipEnabled: true,
				scissorTestEnabled: false);
			pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
			pipelineDescription.ResourceLayouts = new[] {
				factory.CreateResourceLayout(
					new ResourceLayoutDescription(
						new ResourceLayoutElementDescription(
							"Uniforms",
							ResourceKind.UniformBuffer,
							ShaderStages.Vertex
						)
					)
				)
			};
			pipelineDescription.ShaderSet = new ShaderSetDescription(
				vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
				shaders: _shaders);
			pipelineDescription.Outputs = graphicsDevice.SwapchainFramebuffer.OutputDescription;

			_pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
			resourceSet = factory.CreateResourceSet(
				new ResourceSetDescription(
					pipelineDescription.ResourceLayouts[0],
					uniformBuffer
				)
			);
			commandList = factory.CreateCommandList();

			while (window.Exists)
			{
				Render();
				window.PumpEvents();
				renderer.Update();
				renderer.Render();

			}
		}
	}
}
