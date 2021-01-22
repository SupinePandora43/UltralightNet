using ImpromptuNinjas.UltralightSharp.Enums;
using ImpromptuNinjas.UltralightSharp.Safe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
							Console.WriteLine("");
							break;
					}
				}
			});

			AppCore.EnablePlatformFontLoader();
			AppCore.EnablePlatformFileSystem("./");
			Program program = new Program();
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

		private DeviceBuffer uniformBuffer;

		public class GeometryEntry
		{
			public uint VertexArray { get; set; }
			public uint Vertices { get; set; }
			public uint Indices { get; set; }
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

		private static readonly SlottingList<GeometryEntry> GeometryEntries = new SlottingList<GeometryEntry>(32, 8);
		private static readonly SlottingList<TextureEntry> TextureEntries = new SlottingList<TextureEntry>(32, 8);
		private static readonly SlottingList<RenderBufferEntry> RenderBufferEntries = new SlottingList<RenderBufferEntry>(8, 2);

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

		private void UpdateCommandList(ImpromptuNinjas.UltralightSharp.Safe.CommandList list)
		{
			throw new NotImplementedException();
		}

		private void DestroyTexture(uint textureId)
		{
			throw new NotImplementedException();
		}

		private void UpdateTexture(uint textureId, Bitmap bitmap)
		{
			throw new NotImplementedException();
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
			entry.Texure = factory.CreateTexture(TextureDescription.Texture2D(texWidth, texHeight, 1, 1, PixelFormat.R8_G8_B8_A8_UInt, TextureUsage.Staging));
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
			/*var tex = _gl.GenTexture();
			entry.Texure = tex;
			_gl.ActiveTexture(TextureUnit.Texture0);
			_gl.BindTexture(TextureTarget.Texture2D, tex);

			var linear = (int)GLEnum.Linear;
			_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref linear);
			_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref linear);
			CheckGl();

			var clampToEdge = (int)GLEnum.ClampToEdge;
			_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ref clampToEdge);
			_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ref clampToEdge);
			CheckGl();

			if (bitmap.IsEmpty())
			{
				LabelObject(ObjectIdentifier.Texture, tex, $"Ultralight Texture {id} (RT)");
				CheckGl();

				_gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba8, texWidth, texHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, default);
				CheckGl();

				_gl.GenerateMipmap(TextureTarget.Texture2D);
				CheckGl();
			}
			else
			{
				LabelObject(ObjectIdentifier.Texture, tex, $"Ultralight Texture {id}");
				CheckGl();

				_gl.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
				_gl.PixelStore(PixelStoreParameter.UnpackRowLength, (int)(bitmap.GetRowBytes() / bitmap.GetBpp()));
				CheckGl();

				var format = bitmap.GetFormat();

				var pixels = (void*)bitmap.LockPixels();

				switch (format)
				{
					case BitmapFormat.A8UNorm:
						{
							_gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.R8, texWidth, texHeight, 0, PixelFormat.Red, PixelType.UnsignedByte, pixels);
							if (RenderAnsiTexturePreviews)
								Utilities.RenderAnsi<L8>(pixels, texWidth, texHeight, 1, 20);
							break;
						}
					case BitmapFormat.Bgra8UNormSrgb:
						{
							_gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Srgb8Alpha8, texWidth, texHeight, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
							if (RenderAnsiTexturePreviews)
								Utilities.RenderAnsi<Rgba32>(pixels, texWidth, texHeight, 1, 20);
							break;
						}
					default: throw new ArgumentOutOfRangeException(nameof(BitmapFormat));
				}

				CheckGl();

				bitmap.UnlockPixels();
				_gl.GenerateMipmap(TextureTarget.Texture2D);
				CheckGl();
			}*/
		}

		private uint NextTextureId()
		{
			var id = TextureEntries.Add(new TextureEntry()) + 1;
			Console.WriteLine($"GpuDriver.NextTextureId() = {id}");
			return (uint)id;
		}

		private void DestroyRenderBuffer(uint renderBufferId)
		{
			throw new NotImplementedException();
		}

		private void CreateRenderBuffer(uint renderBufferId, RenderBuffer buffer)
		{
			throw new NotImplementedException();
		}

		private uint NextRenderBufferId()
		{
			var id = RenderBufferEntries.Add(new RenderBufferEntry()) + 1;
			Console.WriteLine($"GpuDriver.NextRenderBufferId() = {id}");
			return (uint)id;
		}

		private void DestroyGeometry(uint geometryId)
		{
			throw new NotImplementedException();
		}

		private void UpdateGeometry(uint geometryId, VertexBuffer safeVertices, IndexBuffer indices)
		{
			throw new NotImplementedException();
		}

		private void CreateGeometry(uint geometryId, VertexBuffer safeVertices, IndexBuffer indices)
		{
			throw new NotImplementedException();
		}

		private uint NextGeometryId()
		{
			var id = GeometryEntries.Add(new GeometryEntry()) + 1;
			Console.WriteLine($"GpuDriver.NextGeometryId() = {id}");
			return (uint)id;
		}

		private void EndSynchronize()
		{

		}

		private void BeginSynchronize()
		{

		}

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
			Uniforms uniforms = new() {
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
			string extension;
			switch (graphicsDevice.BackendType)
			{
				case GraphicsBackend.Direct3D11:
					extension = "hlsl.bytes";
					break;
				case GraphicsBackend.Vulkan:
					extension = "450.glsl";
					break;
				case GraphicsBackend.OpenGL:
					extension = "330.glsl";
					break;
				case GraphicsBackend.Metal:
					extension = "metallib";
					break;
				case GraphicsBackend.OpenGLES:
					extension = "300.glsles";
					break;
				default: throw new InvalidOperationException();
			}

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
			BufferDescription ibDescription = new BufferDescription(
				4 * sizeof(ushort),
				BufferUsage.IndexBuffer);
			_indexBuffer = factory.CreateBuffer(ibDescription);
			graphicsDevice.UpdateBuffer(_indexBuffer, 0, quadIndices);

			VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
				new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
				new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));


			ShaderDescription vertexShaderDesc = new ShaderDescription(
				ShaderStages.Vertex,
				Encoding.UTF8.GetBytes(VertexCode),//LoadShaderBytes("FillPath-vertex"),
				"main");
			ShaderDescription fragmentShaderDesc = new ShaderDescription(
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
				/*renderer.Update();
				renderer.Render();
				*/
			}
		}
	}
}
