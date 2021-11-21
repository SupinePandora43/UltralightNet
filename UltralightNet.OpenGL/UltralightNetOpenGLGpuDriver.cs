namespace UltralightNet.OpenGL;

using System;
using System.IO;
using Silk.NET.OpenGL;

public class UltralightNetOpenGLGpuDriver {
	private GL gl;

	private uint pathProgram;
	private uint fillProgram;

	private readonly Dictionary<uint, TextureEntry> textures = new();
	private readonly Dictionary<uint, GeometryEntry> geometries = new();
	private readonly Dictionary<uint, RenderBufferEntry> renderBuffers = new();

	private uint nextTextureId = 0;
	private uint nextGeometryId = 0;
	private uint nextRenderBufferId = 0;

	public UltralightNetOpenGLGpuDriver(GL glapi){
		gl = glapi;

		#region pathProgram
		{
			uint vert = gl.CreateShader(ShaderType.VertexShader);
			uint frag = gl.CreateShader(ShaderType.FragmentShader);

			gl.ShaderSource(vert, GetShader("shader_fill_path.vert.glsl"));
			gl.ShaderSource(frag, GetShader("shader_fill_path.frag.glsl"));

			gl.CompileShader(vert);
			gl.CompileShader(frag);

			string vertLog = gl.GetShaderInfoLog(vert);
            if (!string.IsNullOrWhiteSpace(vertLog))
            {
                throw new Exception($"Error compiling shader of type, failed with error {vertLog}");
            }
			string fragLog = gl.GetShaderInfoLog(frag);
            if (!string.IsNullOrWhiteSpace(fragLog))
            {
                throw new Exception($"Error compiling shader of type, failed with error {fragLog}");
            }

			pathProgram = gl.CreateProgram();

			gl.AttachShader(pathProgram, vert);
			gl.AttachShader(pathProgram, frag);

			gl.LinkProgram(pathProgram);
			gl.GetProgram(pathProgram, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                throw new Exception($"Program failed to link with error: {gl.GetProgramInfoLog(pathProgram)}");
            }

			gl.DetachShader(pathProgram, vert);
            gl.DetachShader(pathProgram, frag);
            gl.DeleteShader(vert);
            gl.DeleteShader(frag);
		}
		#endregion
		#region fillProgram
		{
			uint vert = gl.CreateShader(ShaderType.VertexShader);
			uint frag = gl.CreateShader(ShaderType.FragmentShader);

			gl.ShaderSource(vert, GetShader("shader_fill.vert.glsl"));
			gl.ShaderSource(frag, GetShader("shader_fill.frag.glsl"));

			gl.CompileShader(vert);
			gl.CompileShader(frag);

			string vertLog = gl.GetShaderInfoLog(vert);
            if (!string.IsNullOrWhiteSpace(vertLog))
            {
                throw new Exception($"Error compiling shader of type, failed with error {vertLog}");
            }
			string fragLog = gl.GetShaderInfoLog(frag);
            if (!string.IsNullOrWhiteSpace(fragLog))
            {
                throw new Exception($"Error compiling shader of type, failed with error {fragLog}");
            }

			fillProgram = gl.CreateProgram();

			gl.AttachShader(fillProgram, vert);
			gl.AttachShader(fillProgram, frag);

			gl.LinkProgram(fillProgram);
			gl.GetProgram(fillProgram, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                throw new Exception($"Program failed to link with error: {gl.GetProgramInfoLog(fillProgram)}");
            }

			gl.DetachShader(fillProgram, vert);
            gl.DetachShader(fillProgram, frag);
            gl.DeleteShader(vert);
            gl.DeleteShader(frag);
		}
		#endregion
	}

    private static string GetShader(string name)
	{
		Assembly assembly = typeof(SpirvCross).Assembly;
		Stream stream = assembly.GetManifestResourceStream("UltralightNet.OpenGL.shaders." + name);
		StreamReader resourceStreamReader = new(stream, Encoding.UTF8, false, 16, true);
		return resourceStreamReader.ReadToEnd();
	}


	public ULGpuDriver GetGPUDriver() => new(){
		BeginSynchronize = null,
		EndSynchronize = null,
		NextTextureId = () => {
			uint id = ++nextTextureId;
			textures.Add(id, new());
			return id;
		},
		NextGeometryId = () => {
			uint id = ++nextGeometryId;
			geometries.Add(id, new());
			return id;
		},
		NextRenderBufferId = () => {
			uint id = ++nextRenderBufferId;
			renderBuffers.Add(id, new());
			return id;
		},
	};

	private class TextureEntry
	{
		public uint textureId;
	}
	private class GeometryEntry
	{
		public uint vertices;
		public uint indicies;
	}
	private class RenderBufferEntry
	{
		public uint framebuffer;
		public TextureEntry textureEntry;
	}
}