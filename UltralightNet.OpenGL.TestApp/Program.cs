using System;
using Silk.NET.Maths;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

Window.PrioritizeSdl();

IWindow window = Window.Create(WindowOptions.Default with
{
	Size = new Vector2D<int>(800, 600)
});

string VertexShaderSource = @"
#version 330 core //Using version GLSL version 3.3
layout (location = 0) in vec4 vPos;

void main()
{
    gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
}
";

string FragmentShaderSource = @"
#version 330 core
out vec4 FragColor;
void main()
{
    FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
}
";

//Vertex data, uploaded to the VBO.
float[] Vertices =
{
    //X    Y      Z
    0.5f,  0.5f, 0.0f,
	0.5f, -0.5f, 0.0f,
	-0.5f, -0.5f, 0.0f,
	-0.5f,  0.5f, 0.5f
};

//Index data, uploaded to the EBO.
uint[] Indices =
{
	0, 1, 3,
	1, 2, 3
};

GL gl = null;

uint vao = 0, vbo = 0, ebo = 0;
uint Shader = 0;

unsafe void OnLoad()
{
	IInputContext input = window.CreateInput();
	for (int i = 0; i < input.Keyboards.Count; i++)
	{
		//input.Keyboards[i].KeyDown += KeyDown;
	}

	//Getting the opengl api for drawing to the screen.
	gl = GL.GetApi(window);

	//Creating a vertex array.
	vao = gl.GenVertexArray();
	gl.BindVertexArray(vao);

	//Initializing a vertex buffer that holds the vertex data.
	vbo = gl.GenBuffer(); //Creating the buffer.
	gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo); //Binding the buffer.
	fixed (void* v = &Vertices[0])
	{
		gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(Vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw); //Setting buffer data.
	}

	//Initializing a element buffer that holds the index data.
	ebo = gl.GenBuffer(); //Creating the buffer.
	gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo); //Binding the buffer.
	fixed (void* i = &Indices[0])
	{
		gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(Indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw); //Setting buffer data.
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
	Shader = gl.CreateProgram();
	gl.AttachShader(Shader, vertexShader);
	gl.AttachShader(Shader, fragmentShader);
	gl.LinkProgram(Shader);

	//Checking the linking for errors.
	gl.GetProgram(Shader, GLEnum.LinkStatus, out var status);
	if (status == 0)
	{
		Console.WriteLine($"Error linking shader {gl.GetProgramInfoLog(Shader)}");
	}

	//Delete the no longer useful individual shaders;
	gl.DetachShader(Shader, vertexShader);
	gl.DetachShader(Shader, fragmentShader);
	gl.DeleteShader(vertexShader);
	gl.DeleteShader(fragmentShader);

	//Tell opengl how to give the data to the shaders.
	gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
	gl.EnableVertexAttribArray(0);
}

unsafe void OnRender(double obj)
{
	gl.Clear((uint)ClearBufferMask.ColorBufferBit);
	gl.BindVertexArray(vao);
	gl.UseProgram(Shader);
	gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
}

void OnUpdate(double obj)
{
	//Here all updates to the program should be done.
}

window.Load += OnLoad;
window.Render += OnRender;

window.Run();