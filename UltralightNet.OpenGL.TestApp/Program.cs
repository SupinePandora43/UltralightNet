using System;
using System.Numerics;
using Silk.NET.Maths;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using UltralightNet.AppCore;

namespace UltralightNet.OpenGL.TestApp;

public static class Program
{
	const string VertexShaderSource = @"
#version 420 core //Using version GLSL version 3.3
layout (location = 0) in vec4 vPos;
layout (location = 0) out vec2 uv;
void main()
{
	uv = vec2(vPos.z, vPos.w);
    gl_Position = vec4(vPos.x, vPos.y, 0, 1.0);
}
";

	const string FragmentShaderSource = @"
#version 420 core
layout (location = 0) in vec2 uv;
uniform sampler2D rt;
out vec4 FragColor;
void main()
{
	FragColor = texture(rt, uv);
    //FragColor = vec4(uv.x, uv.y, 0.0f, 1.0f);
    //FragColor = vec4(1.0f,1.0f,0.0f,1.0f);
}
";

	static float[] VertexBuffer = new float[]{
		-1f, 1f, 0f, 1f,
		1f, 1f, 1f, 1f,
		1f,-1f, 1f, 0f,
		-1f,-1f, 0f, 0f
	};

	static uint[] IndexBuffer = new uint[]
	{
		0,1,2,
		2,3,0
	};

	static IWindow window;
	static GL gl;

	static uint vao = 0, vbo = 0, ebo = 0;
	static uint quadProgram = 0;

	static Renderer renderer;
	static View view;

	static OpenGLGPUDriver gpuDriver;

	public static void Main()
	{
		AppCoreMethods.ulEnablePlatformFontLoader();
		AppCoreMethods.ulEnablePlatformFileSystem("./");
		AppCoreMethods.ulEnableDefaultLogger("./log123as.txt");

		Window.PrioritizeSdl();

		window = Window.Create(WindowOptions.Default with
		{
			Size = new Vector2D<int>(800, 600),
			API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(3, 3))
		});

		window.Load += OnLoad;
		window.Update += OnUpdate;
		window.Render += OnRender;
		window.FramebufferResize += s =>
		{
			gl.Viewport(s);
			view.Resize((uint)s.X, (uint)s.Y);
		};

		window.Run();
	}

	static unsafe void OnLoad()
	{
		IInputContext input = window.CreateInput();
		for (int i = 0; i < input.Keyboards.Count; i++)
		{
			//input.Keyboards[i].KeyDown += KeyDown;
		}
		input.Mice[0].Scroll += OnScroll;
		input.Mice[0].MouseDown += OnMouseDown;
		input.Mice[0].MouseUp += OnMouseUp;
		input.Mice[0].MouseMove += OnMouseMove;

		//Getting the opengl api for drawing to the screen.
		gl = GL.GetApi(window);

		//gl.Enable(GLEnum.FramebufferSrgb);

		gl.CullFace(GLEnum.None);
		gl.Disable(GLEnum.CullFace);
		gl.FrontFace(GLEnum.Ccw);
		gl.Disable(EnableCap.DepthTest);
		gl.DepthFunc(DepthFunction.Never);
		//gl.Disable(GLEnum.StencilTest);

		//Creating a vertex array.
		vao = gl.GenVertexArray();
		gl.BindVertexArray(vao);

		//Initializing a vertex buffer that holds the vertex data.
		vbo = gl.GenBuffer(); //Creating the buffer.
		gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo); //Binding the buffer.
		fixed (void* v = &VertexBuffer[0])
		{
			gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(VertexBuffer.Length * sizeof(float)), v, BufferUsageARB.StaticDraw); //Setting buffer data.
		}

		//Initializing a element buffer that holds the index data.
		ebo = gl.GenBuffer(); //Creating the buffer.
		gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo); //Binding the buffer.
		fixed (void* i = &IndexBuffer[0])
		{
			gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(IndexBuffer.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw); //Setting buffer data.
		}

		//Creating a vertex shader.
		uint vertexShader = gl.CreateShader(ShaderType.VertexShader);
		gl.ShaderSource(vertexShader, VertexShaderSource);
		gl.CompileShader(vertexShader);

		//Checking the shader for compilation errors.
		string infoLog = gl.GetShaderInfoLog(vertexShader);
		if (!string.IsNullOrWhiteSpace(infoLog))
		{
			Console.WriteLine($"Error compiling vertex shader {infoLog}");
		}

		//Creating a fragment shader.
		uint fragmentShader = gl.CreateShader(ShaderType.FragmentShader);
		gl.ShaderSource(fragmentShader, FragmentShaderSource);
		gl.CompileShader(fragmentShader);

		//Checking the shader for compilation errors.
		infoLog = gl.GetShaderInfoLog(fragmentShader);
		if (!string.IsNullOrWhiteSpace(infoLog))
		{
			Console.WriteLine($"Error compiling fragment shader {infoLog}");
		}

		//Combining the shaders under one shader program.
		quadProgram = gl.CreateProgram();
		gl.AttachShader(quadProgram, vertexShader);
		gl.AttachShader(quadProgram, fragmentShader);
		gl.BindAttribLocation(quadProgram, 0, "vPos");
		gl.LinkProgram(quadProgram);

		//Checking the linking for errors.
		gl.GetProgram(quadProgram, GLEnum.LinkStatus, out var status);
		if (status == 0)
		{
			Console.WriteLine($"Error linking shader {gl.GetProgramInfoLog(quadProgram)}");
		}

		//Delete the no longer useful individual shaders;
		gl.DetachShader(quadProgram, vertexShader);
		gl.DetachShader(quadProgram, fragmentShader);
		gl.DeleteShader(vertexShader);
		gl.DeleteShader(fragmentShader);

		//Tell opengl how to give the data to the shaders.

		gl.UseProgram(quadProgram);
		gl.Uniform1(gl.GetUniformLocation(quadProgram, "rt"), 0);

		gl.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), null);
		gl.EnableVertexAttribArray(0);

		window.SwapBuffers();

		gpuDriver = new(gl);

		window.SwapBuffers();

		ULPlatform.GPUDriver = gpuDriver.GetGPUDriver();

		renderer = ULPlatform.CreateRenderer(new ULConfig { FaceWinding = ULFaceWinding.Clockwise, ForceRepaint = true });

		view = renderer.CreateView(800, 600, new ULViewConfig { IsAccelerated = true, IsTransparent = false });
		view.URL = "https://youtube.com";
		//view.HTML = "<html><body><p>123</p></body></html>";
		/*bool loaded = false;

		view.OnFinishLoading += (_, _, _) => loaded = true;

		while (!loaded)
		{
			renderer.Update();
			System.Threading.Thread.Sleep(10);
		}*/

		window.SwapBuffers();

		renderer.Render();

		window.SwapBuffers();
	}

	static unsafe void OnRender(double obj)
	{
		gpuDriver.Check();
		renderer.Update();
		renderer.Render();
		gpuDriver.Check();

		uint rtId = gpuDriver.textures[view.RenderTarget.texture_id].textureId;
		//window.ClearContext();
		gl.Clear((uint)ClearBufferMask.ColorBufferBit);
		gpuDriver.Check();

		gl.ActiveTexture(GLEnum.Texture0);
		gpuDriver.Check();
		gl.BindTexture(GLEnum.Texture2D, rtId);
		gpuDriver.Check();

		gl.BindVertexArray(vao);
		gpuDriver.Check();
		gl.UseProgram(quadProgram);
		gpuDriver.Check();
		gl.DrawElements(PrimitiveType.Triangles, (uint)IndexBuffer.Length, DrawElementsType.UnsignedInt, null);
		gpuDriver.Check();
	}

	static void OnUpdate(double obj)
	{
		//renderer.Update();
	}

	static void OnScroll(IMouse _, ScrollWheel scroll)
	{
		view.FireScrollEvent(new ULScrollEvent { type = ULScrollEventType.ByPage, deltaX = (int)scroll.X, deltaY = (int)scroll.Y });
	}

	static void OnMouseDown(IMouse mouse, MouseButton button)
	{
		view.FireMouseEvent(new() { type = ULMouseEventType.MouseDown, button = button is MouseButton.Left ? ULMouseEventButton.Left : (button is MouseButton.Right ? ULMouseEventButton.Right : ULMouseEventButton.Middle), x = (int)mouse.Position.X, y = (int)mouse.Position.Y });
	}

	static void OnMouseUp(IMouse mouse, MouseButton button)
	{
		view.FireMouseEvent(new() { type = ULMouseEventType.MouseUp, button = button is MouseButton.Left ? ULMouseEventButton.Left : (button is MouseButton.Right ? ULMouseEventButton.Right : ULMouseEventButton.Middle), x = (int)mouse.Position.X, y = (int)mouse.Position.Y });
	}

	static void OnMouseMove(IMouse mouse, Vector2 position)
	{
		view.FireMouseEvent(new() { type = ULMouseEventType.MouseMoved, x = (int)position.X, y = (int)position.Y });
	}
}
