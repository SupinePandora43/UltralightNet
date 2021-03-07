using Supine.UltralightSharp.Enums;
using Supine.UltralightSharp.Safe;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace VeldridSandbox
{
	public partial class Program
	{
		const GraphicsBackend BACKEND = GraphicsBackend.Direct3D11;


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
			stopwatch = new Stopwatch();
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
			bool loaded = false;
			view.SetFinishLoadingCallback((IntPtr userData, View caller, ulong frameId, bool isMainFrame,
	  string? url) =>
			{ loaded = true; }, default);
			view.LoadUrl("https://github.com"); //https://github.com
			Stream html = assembly.GetManifestResourceStream("VeldridSandbox.embedded.index.html");
			StreamReader htmlReader = new(html, Encoding.UTF8);
			//view.LoadHtml(htmlReader.ReadToEnd());
			while (!loaded)
			{
				renderer.Update();
				Thread.Sleep(10);
			}
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
				BACKEND);

			window.Resized += () =>
			{
				Console.WriteLine("~~~~~RESIZED~~~~~");
				graphicsDevice.ResizeMainWindow((uint)window.Width, (uint)window.Height);
				width = window.Width;
				height = window.Height;
				view.Resize((uint)window.Width, (uint)window.Height);
			};
			window.MouseDown += (me) =>
			{
				view.FireMouseEvent(new Supine.UltralightSharp.Safe.MouseEvent(MouseEventType.MouseDown, (int)window.MouseDelta.X, (int)window.MouseDelta.Y, Supine.UltralightSharp.Enums.MouseButton.Left));
			};
			window.MouseUp += (me) =>
			{
				view.FireMouseEvent(new Supine.UltralightSharp.Safe.MouseEvent(MouseEventType.MouseUp, (int)window.MouseDelta.X, (int)window.MouseDelta.Y, Supine.UltralightSharp.Enums.MouseButton.Left));
			};
			window.MouseMove += (mouseMove) =>
			{
				view.FireMouseEvent(new Supine.UltralightSharp.Safe.MouseEvent(MouseEventType.MouseMoved, (int)mouseMove.MousePosition.X, (int)mouseMove.MousePosition.Y, Supine.UltralightSharp.Enums.MouseButton.None));
			};
			window.MouseWheel += (mw) =>
			{
				view.FireScrollEvent(new ScrollEvent(ScrollEventType.ScrollByPage, 0, (int)mw.WheelDelta));
			};
			factory = graphicsDevice.ResourceFactory;
		}

		private void Run()
		{
			stopwatch.Start();
			ulong frames = 0;
			while (window.Exists)
			{
				if (stopwatch.ElapsedMilliseconds >= 1000)
				{
					Console.WriteLine(frames);
					frames = 0;
					stopwatch.Restart();
				}
				renderer.Update();
				renderer.Render();
				Render();
				window.PumpEvents();
				frames++;
			}
		}
	}
}
