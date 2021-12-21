using System;
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
    gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
}
";

	const string FragmentShaderSource = @"
#version 420 core
layout (location = 0) in vec2 uv;
out vec4 FragColor;
void main()
{
    FragColor = vec4(uv.x, uv.y, 0.0f, 1.0f);
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
	static GL gl = null;

	static uint vao = 0, vbo = 0, ebo = 0;
	static uint quadProgram = 0;

	static Renderer renderer;
	static View view;

	static OpenGLGPUDriver gpuDriver;

	public static void Main()
	{
		AppCoreMethods.ulEnablePlatformFontLoader();
		AppCoreMethods.ulEnablePlatformFileSystem("./");

		Window.PrioritizeSdl();

		window = Window.Create(WindowOptions.Default with
		{
			Size = new Vector2D<int>(800, 600),
			API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(3, 3))
		});

		window.Load += OnLoad;
		window.Update += OnUpdate;
		window.Render += OnRender;
		window.FramebufferResize += s => gl.Viewport(s);

		window.Run();
	}

	static unsafe void OnLoad()
	{
		IInputContext input = window.CreateInput();
		for (int i = 0; i < input.Keyboards.Count; i++)
		{
			//input.Keyboards[i].KeyDown += KeyDown;
		}

		//Getting the opengl api for drawing to the screen.
		gl = GL.GetApi(window);

		gl.Enable(GLEnum.FramebufferSrgb);

		gl.Enable(GLEnum.CullFace);
		gl.CullFace(GLEnum.Back);
		gl.FrontFace(GLEnum.CW);
		gl.Disable(GLEnum.Depth);

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

		gl.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), null);
		gl.EnableVertexAttribArray(0);

		gpuDriver = new(gl);

		ULPlatform.GPUDriver = gpuDriver.GetGPUDriver();

		renderer = ULPlatform.CreateRenderer();

		view = renderer.CreateView(800, 600, new ULViewConfig { IsAccelerated = true });
		view.URL = "https://github.com";

		bool loaded = false;

		view.OnFinishLoading += (_, _, _) => loaded = true;

		while (!loaded)
		{
			renderer.Update();
			System.Threading.Thread.Sleep(10);
		}

		window.SwapBuffers();
	}

	static unsafe void OnRender(double obj)
	{
		renderer.Render();

		gl.Clear((uint)ClearBufferMask.ColorBufferBit);
		gl.BindVertexArray(vao);
		gl.UseProgram(quadProgram);
		gl.DrawElements(PrimitiveType.Triangles, (uint)IndexBuffer.Length, DrawElementsType.UnsignedInt, null);
	}

	static void OnUpdate(double obj)
	{
		renderer.Update();
	}
}
