using Supine.UltralightSharp.Enums;
using Supine.UltralightSharp.Safe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
using Bitmap = Supine.UltralightSharp.Safe.Bitmap;
using DBitmap = System.Drawing.Bitmap;
using PixelFormat = Veldrid.PixelFormat;

namespace VeldridSandbox
{
	public partial class Program
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

			program.Init();
			program.CreatePipeline();
			program.InitUltralight();
			program.Run();

			program = null;
		}

		private int width = 512;
		private int height = 512;

		private Sdl2Window window;
		private Stopwatch stopwatch;

		private Renderer renderer;
		private View view;

		public Program()
		{
			stopwatch = Stopwatch.StartNew();
		}

		~Program()
		{
			stopwatch = null;
		}

		private readonly Assembly assembly = typeof(Program).Assembly;

		private byte[] GetShaderBytes(string path)
		{
			Stream resourceStream = assembly.GetManifestResourceStream("VeldridSandbox." + path) ?? throw new FileNotFoundException(path);
			StreamReader resourceStreamReader = new(resourceStream, Encoding.UTF8, false, 16, true);
			string shaderCode = resourceStreamReader.ReadToEnd();
			return Encoding.UTF8.GetBytes(shaderCode);
		}

		private Shader GetShader(string path, ShaderStages shaderStages)
		{
			//SpirvCompilationResult spirv = SpirvCompilation.CompileGlslToSpirv(shaderCode, "VertexFill", shaderStages, GlslCompileOptions.Default);
			ShaderDescription shaderDescription = new(shaderStages, GetShaderBytes(path), "main");
			//ShaderDescription shaderDescription = new(shaderStages, spirv.SpirvBytes, "main");
			return factory.CreateFromSpirv(shaderDescription);
			//return factory.CreateShader(shaderDescription);
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

		public void InitUltralight()
		{
			#region GpuDriver
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
			#endregion
			#region Ultralight Initialization

			Config cfg = new();
			cfg.SetResourcePath("resources");
			cfg.SetUseGpuRenderer(true);
			renderer = new(cfg);
			Session session = renderer.GetDefaultSession();
			view = new(renderer, 512, 512, false, session, false);
			view.LoadUrl("https://github.com");
			#endregion
		}

		private void Init()
		{
			WindowCreateInfo windowCI = new()
			{
				X = 100,
				Y = 100,
				WindowWidth = 512,
				WindowHeight = 512,
				WindowTitle = "Veldrid Ultralight"
			};
			window = VeldridStartup.CreateWindow(ref windowCI);

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
		}

		private void Run()
		{
			while (window.Exists)
			{
				window.PumpEvents();
				renderer.Update();
				renderer.Render();
				Render();
			}
		}
	}
}
