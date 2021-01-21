using ImpromptuNinjas.UltralightSharp.Enums;
using ImpromptuNinjas.UltralightSharp.Safe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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

		private Renderer renderer;
		private View view;

		private GraphicsDevice graphicsDevice;
		private ResourceFactory factory;

		private Veldrid.CommandList _commandList;
		private DeviceBuffer _vertexBuffer;
		private DeviceBuffer _indexBuffer;
		private Shader[] _shaders;
		private Pipeline _pipeline;

		public class GeometryEntry
		{
			public uint VertexArray { get; set; }
			public uint Vertices { get; set; }
			public uint Indices { get; set; }
		}
		public class RenderBufferEntry
		{
			public uint FrameBuffer { get; set; }
			public TextureEntry TextureEntry { get; set; } = null!;
		}

		public class TextureEntry
		{
			public uint Texure { get; set; }
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
			/*var index = (int)textureId - 1;
			var entry = TextureEntries[index];
			var texWidth = bitmap.GetWidth();
			entry.Width = texWidth;
			var texHeight = bitmap.GetHeight();
			entry.Height = texHeight;
			Console.WriteLine($"GpuDriver.CreateTexture({textureId} {texWidth}x{texHeight})");
			factory.CreateTexture(TextureDescription.Texture2D(texWidth, texHeight, 1, 1, PixelFormat.R8_G8_B8_A8_UInt, TextureUsage.Sampled));
			var tex = _gl.GenTexture();
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
			ShaderDescription shaderDescription = new(shaderStages, Encoding.UTF8.GetBytes(shaderCode), "main");
			return factory.CreateFromSpirv(shaderDescription);
		}

		private void Run()
		{
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
			graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, options, GraphicsBackend.OpenGLES);

			factory = graphicsDevice.ResourceFactory;

			Shader vertexShader = GetShader("embedded.shader_v2f_c4f_t2f_t2f_d28f.vert.glsl", ShaderStages.Vertex);
			Shader fragmentShader = GetShader("embedded.shader_fill.frag.glsl", ShaderStages.Fragment);
			Shader pathVertexShader = GetShader("embedded.shader_v2f_c4f_t2f.vert.glsl", ShaderStages.Vertex);
			Shader pathFragmentShader = GetShader("embedded.shader_fill_path.frag.glsl", ShaderStages.Fragment);

			// Create pipeline
			GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
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
			pipelineDescription.ResourceLayouts = System.Array.Empty<ResourceLayout>();
			pipelineDescription.ShaderSet = new ShaderSetDescription(
				vertexLayouts: new VertexLayoutDescription[] { },
				shaders: _shaders);
			pipelineDescription.Outputs = graphicsDevice.SwapchainFramebuffer.OutputDescription;
			_pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

			_commandList = factory.CreateCommandList();

			while (window.Exists)
			{
				window.PumpEvents();
				renderer.Update();
				renderer.Render();
			}
		}
	}
}
