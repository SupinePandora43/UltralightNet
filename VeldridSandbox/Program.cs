using Supine.UltralightSharp.Enums;
using Supine.UltralightSharp.Safe;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

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

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		private void Run()
		{
			while (window.Exists)
			{
				renderer.Update();
				renderer.Render();
				Render();
				window.PumpEvents();
			}
		}
	}
}
