namespace UltralightNet.OpenGL;

using System;
using System.IO;
using Silk.NET.OpenGL;

public unsafe class UltralightNetOpenGLGpuDriver {
	private GL gl;

	private uint pathProgram;
	private uint fillProgram;

	private readonly Dictionary<uint, TextureEntry> textures = new();
	private readonly Dictionary<uint, GeometryEntry> geometries = new();
	private readonly Dictionary<uint, RenderBufferEntry> renderBuffers = new();

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
			for (uint i = 0; ; i++)
			{
				if (!textures.ContainsKey(i)){
					textures.Add(i, new());
					return i;
				}
			}
		},
		NextGeometryId = () => {
			for (uint i = 0; ; i++)
			{
				if (!geometries.ContainsKey(i)){
					geometries.Add(i, new());
					return i;
				}
			}
		},
		NextRenderBufferId = () => {
			for (uint i = 0; ; i++)
			{
				if (!renderBuffers.ContainsKey(i)){
					renderBuffers.Add(i, new());
					return i;
				}
			}
		},
		CreateTexture = (entryId, bitmap) => {
			uint textureId = gl.GenTexture();
			textures[entryId].textureId = textureId;

			gl.ActiveTexture(TextureUnit.Texture0);
			gl.BindTexture(TextureTarget.Texture2D, textureId);

			var linear = (int) GLEnum.Linear;
			gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref linear);
			gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref linear);
		
			var clampToEdge = (int) GLEnum.ClampToEdge;
			gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ref clampToEdge);
			gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ref clampToEdge);
	
			if(bitmap.IsEmpty){
				// FIXME: rgba
				gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.Rgba8, bitmap.Width, bitmap.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, default);
			} else {
				gl.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
				gl.PixelStore(PixelStoreParameter.UnpackRowLength, (int) (bitmap.RowBytes / bitmap.Bpp));
			
				void* pixels = (void*) bitmap.LockPixels();
				
				if(bitmap.Format is ULBitmapFormat.BGRA8_UNORM_SRGB){
					// FIXME: rgba
					gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.Srgb8Alpha8, bitmap.Width, bitmap.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
				}else{
					gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.R8, texWidth, texHeight, 0, PixelFormat.Red, PixelType.UnsignedByte, pixels);
				}

				bitmap.UnlockPixels();
			}

			//FIXME: mipmap
			gl.GenerateMipmap(TextureTarget.Texture2D);
		},
		UpdateTexture = (entryId, bitmap) => {
			uint textureId = textures[entryId].textureId;

			gl.ActiveTexture(TextureUnit.Texture0);
			gl.BindTexture(TextureTarget.Texture2D, textureId);

			gl.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
			gl.PixelStore(PixelStoreParameter.UnpackRowLength, (int) (bitmap.RowBytes / bitmap.Bpp));
		
			void* pixels = (void*) bitmap.LockPixels();
			
			if(bitmap.Format is ULBitmapFormat.BGRA8_UNORM_SRGB){
				// FIXME: rgba
				gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.Srgb8Alpha8, bitmap.Width, bitmap.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
			}else{
				gl.TexImage2D(TextureTarget.Texture2D, 0, (int) InternalFormat.R8, texWidth, texHeight, 0, PixelFormat.Red, PixelType.UnsignedByte, pixels);
			}

			bitmap.UnlockPixels();

			//FIXME: mipmap
			gl.GenerateMipmap(TextureTarget.Texture2D);
		}
	};

	// TODO: delete if useless
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