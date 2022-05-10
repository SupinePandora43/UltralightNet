namespace UltralightNet.OpenGL;

using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Silk.NET.OpenGL;
using UltralightNet;
using UltralightNet.GPUCommon;
using System.Runtime.InteropServices;

public static unsafe partial class OpenGLGPUDriver
{
	private static GL? gl;

	private static uint pathProgram;
	private static uint fillProgram;
	private static uint UBO;

	// TODO: use Lists
	public static readonly List<TextureEntry> textures = new();
	private static readonly List<GeometryEntry> geometries = new();
	public static readonly List<RenderBufferEntry> renderBuffers = new();

	private static readonly Stack<int> freeTextures = new();
	private static readonly Stack<int> freeGeometry = new();
	private static readonly Stack<int> freeRenderBuffers = new();

	public static void Check([CallerLineNumber] int line = default)
	{
#if DEBUG
		var error = gl.GetError();
		if (error is not 0)
			Console.WriteLine($"{line}: {error}");
#endif
	}

	private static bool DSA = true;
	private static uint samples = 0;

	private static bool initialized = false;

	public static void Initialize(GL glapi, uint samples = 0)
	{
		OpenGLGPUDriver.gl = glapi;

		// DSA
		OpenGLGPUDriver.DSA = gl.GetInteger(GLEnum.MajorVersion) >= 4 && gl.GetInteger(GLEnum.MinorVersion) >= 5;
		// MSAAx4
		OpenGLGPUDriver.samples = samples is 0 ? (4 <= gl.GetInteger(GLEnum.MaxSamples) ? 4u : 1u) : samples;

		textures.Add(new());
		geometries.Add(new());
		renderBuffers.Add(new());

		// Save state
		uint glProgram;
		gl.GetInteger(GetPName.CurrentProgram, (int*)&glProgram);

		#region pathProgram
		{
			uint vert = gl.CreateShader(ShaderType.VertexShader);
			uint frag = gl.CreateShader(ShaderType.FragmentShader);

			gl.ShaderSource(vert, GetShader("shader_fill_path.vert"));
			gl.ShaderSource(frag, GetShader("shader_fill_path.frag"));

			gl.CompileShader(vert);
			gl.CompileShader(frag);

			string vertLog = gl.GetShaderInfoLog(vert);
			if (!string.IsNullOrWhiteSpace(vertLog))
			{
				//throw new Exception($"Error compiling shader of type, failed with error {vertLog}");
			}
			string fragLog = gl.GetShaderInfoLog(frag);
			if (!string.IsNullOrWhiteSpace(fragLog))
			{
				//throw new Exception($"Error compiling shader of type, failed with error {fragLog}");
			}

			pathProgram = gl.CreateProgram();

			gl.AttachShader(pathProgram, vert);
			gl.AttachShader(pathProgram, frag);

			gl.BindAttribLocation(pathProgram, 0, "in_Position");
			gl.BindAttribLocation(pathProgram, 1, "in_Color");
			gl.BindAttribLocation(pathProgram, 2, "in_TexCoord");

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

			gl.UniformBlockBinding(pathProgram, gl.GetUniformBlockIndex(pathProgram, "Uniforms"), 0);
		}
		#endregion
		#region fillProgram
		{
			uint vert = gl.CreateShader(ShaderType.VertexShader);
			uint frag = gl.CreateShader(ShaderType.FragmentShader);

			gl.ShaderSource(vert, GetShader("shader_fill.vert"));
			gl.ShaderSource(frag, GetShader("shader_fill.frag"));

			gl.CompileShader(vert);
			gl.CompileShader(frag);

			string vertLog = gl.GetShaderInfoLog(vert);
			if (!string.IsNullOrWhiteSpace(vertLog))
			{
				//throw new Exception($"Error compiling shader of type, failed with error {vertLog}");
			}
			string fragLog = gl.GetShaderInfoLog(frag);
			if (!string.IsNullOrWhiteSpace(fragLog))
			{
				//throw new Exception($"Error compiling shader of type, failed with error {fragLog}");
			}

			fillProgram = gl.CreateProgram();

			gl.AttachShader(fillProgram, vert);
			gl.AttachShader(fillProgram, frag);

			gl.BindAttribLocation(fillProgram, 0, "in_Position");
			gl.BindAttribLocation(fillProgram, 1, "in_Color");
			gl.BindAttribLocation(fillProgram, 2, "in_TexCoord");
			gl.BindAttribLocation(fillProgram, 3, "in_ObjCoord");
			gl.BindAttribLocation(fillProgram, 4, "in_Data0");
			gl.BindAttribLocation(fillProgram, 5, "in_Data1");
			gl.BindAttribLocation(fillProgram, 6, "in_Data2");
			gl.BindAttribLocation(fillProgram, 7, "in_Data3");
			gl.BindAttribLocation(fillProgram, 8, "in_Data4");
			gl.BindAttribLocation(fillProgram, 9, "in_Data5");
			gl.BindAttribLocation(fillProgram, 10, "in_Data6");

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

			gl.UseProgram(fillProgram);
			gl.Uniform1(gl.GetUniformLocation(fillProgram, "Texture1"), 0);
			gl.Uniform1(gl.GetUniformLocation(fillProgram, "Texture2"), 1);
			gl.UniformBlockBinding(fillProgram, gl.GetUniformBlockIndex(fillProgram, "Uniforms"), 0);
		}
		#endregion

		Check();

		if (DSA)
		{
			UBO = gl.CreateBuffer();
			gl.NamedBufferData(UBO, 768, null, VertexBufferObjectUsage.DynamicDraw);
		}
		else
		{
			UBO = gl.GenBuffer();
			gl.BindBuffer(GLEnum.UniformBuffer, UBO);
			gl.BufferData(GLEnum.UniformBuffer, 768, null, GLEnum.DynamicDraw);
			gl.BindBuffer(GLEnum.UniformBuffer, 0);
		}
		gl.BindBufferRange(GLEnum.UniformBuffer, 0, UBO, 0, 768);

		// Restore state
		gl.UseProgram(glProgram);

		initialized = true;
	}

	private static string GetShader(string name)
	{
		Assembly assembly = typeof(OpenGLGPUDriver).Assembly;
		Stream stream = assembly.GetManifestResourceStream("UltralightNet.OpenGL." + name);
		StreamReader resourceStreamReader = new(stream, Encoding.UTF8, false, 16, true);
		return resourceStreamReader.ReadToEnd();
	}

	public static ULGPUDriver GetGPUDriver(){
		if(!initialized) throw new Exception("Initialize first!");
		return new()
		{
			__BeginSynchronize = null,
			__EndSynchronize = null,
			__NextTextureId = &NextTextureId,
			__NextGeometryId = &NextGeometryId,
			__NextRenderBufferId = &NextRenderBufferId,
			__CreateTexture = &CreateTexture,
			__UpdateTexture = &UpdateTexture,
			__DestroyTexture = &DestroyTexture,
			__CreateRenderBuffer = &CreateRenderBuffer,
			__DestroyRenderBuffer = &DestroyRenderBuffer,
			__CreateGeometry = &CreateGeometry,
			__UpdateGeometry = &UpdateGeometry,
			__DestroyGeometry = &DestroyGeometry,
			__UpdateCommandList = &UpdateCommandList
		};
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] {typeof(CallConvCdecl)})]
	private static uint NextTextureId(){
		if(freeTextures.TryPop(out int freeId)){
			textures[freeId] = new();
			return (uint)freeId;
		}else{
			textures.Add(new());
			return (uint)textures.Count -1;
		}
	}
	[UnmanagedCallersOnly(CallConvs = new Type[] {typeof(CallConvCdecl)})]
	private static uint NextGeometryId(){
		if(freeGeometry.TryPop(out int freeId)){
			geometries[freeId] = new();
			return (uint)freeId;
		}else{
			geometries.Add(new());
			return (uint)geometries.Count -1;
		}
	}
	[UnmanagedCallersOnly(CallConvs = new Type[] {typeof(CallConvCdecl)})]
	private static uint NextRenderBufferId(){
		if(freeRenderBuffers.TryPop(out int freeId)){
			renderBuffers[freeId] = new();
			return (uint)freeId;
		}else{
			renderBuffers.Add(new());
			return (uint)renderBuffers.Count -1;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] {typeof(CallConvCdecl)})]
	private static void CreateTexture(uint entryId, void* bitmapPtr){
		ULBitmap bitmap = new((IntPtr)bitmapPtr);

		var isRT = bitmap.IsEmpty;

		uint textureId;
		uint multisampledTextureId = 0;

		uint width = bitmap.Width;
		uint height = bitmap.Height;

		uint rowBytes = bitmap.RowBytes;
		uint bpp = bitmap.Bpp;

		if (DSA)
		{
			if (isRT)
			{
				gl.CreateTextures(TextureTarget.Texture2D, 1, &textureId);
				gl.TextureStorage2D(textureId, 1, SizedInternalFormat.Rgba8, width, height);

				if (samples is not 1)
				{
					gl.CreateTextures(TextureTarget.Texture2DMultisample, 1, &multisampledTextureId);
					gl.TextureStorage2DMultisample(multisampledTextureId, samples, SizedInternalFormat.Rgba8, width, height, true);
				}
			}
			else
			{
				gl.CreateTextures(TextureTarget.Texture2D, 1, &textureId);

				if(rowBytes != width * bpp){
					gl.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
					gl.PixelStore(PixelStoreParameter.UnpackRowLength, (int)(rowBytes / bpp));
				}

				void* pixels = (void*)bitmap.LockPixels();

				if (bitmap.Format is ULBitmapFormat.BGRA8_UNORM_SRGB)
				{
					gl.TextureStorage2D(textureId, 1, SizedInternalFormat.Rgba8, width, height);
					gl.TextureSubImage2D(textureId, 0, 0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);
				}
				else
				{
					gl.TextureStorage2D(textureId, 1, SizedInternalFormat.R8, width, height);
					gl.TextureSubImage2D(textureId, 0, 0, 0, width, height, PixelFormat.Red, PixelType.UnsignedByte, pixels);
				}

				bitmap.UnlockPixels();

				gl.TextureParameterI(textureId, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
				gl.TextureParameterI(textureId, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

				gl.TextureParameterI(textureId, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
				gl.TextureParameterI(textureId, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
				gl.TextureParameterI(textureId, TextureParameterName.TextureWrapR, (int)GLEnum.Repeat);

				//gl.GenerateTextureMipmap(textureId);
			}
		}
		else
		{
			textureId = gl.GenTexture();

			gl.ActiveTexture(TextureUnit.Texture0);
			gl.BindTexture(TextureTarget.Texture2D, textureId);

			gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
			gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
			gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
			gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);

			if (isRT)
			{
				gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba8, width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, null);
				if (samples is not 1)
				{
					gl.GenTextures(1, &multisampledTextureId);
					gl.BindTexture(TextureTarget.Texture2DMultisample, multisampledTextureId);
					gl.TexImage2DMultisample(TextureTarget.Texture2DMultisample, samples, GLEnum.Rgba8, width, height, true);
				}
			}
			else
			{
				if(rowBytes != width * bpp){
					gl.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
					gl.PixelStore(PixelStoreParameter.UnpackRowLength, (int)(rowBytes / bpp));
				}

				void* pixels = (void*)bitmap.LockPixels();

				if (bitmap.Format is ULBitmapFormat.BGRA8_UNORM_SRGB)
				{
					gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba8, width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);
				}
				else
				{
					gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.R8, width, height, 0, PixelFormat.Red, PixelType.UnsignedByte, pixels);
				}

				bitmap.UnlockPixels();
			}
			//gl.GenerateMipmap(TextureTarget.Texture2D);
		}

		textures[(int)entryId].textureId = textureId;
		textures[(int)entryId].multisampledTextureId = multisampledTextureId;
		textures[(int)entryId].width = width;
		textures[(int)entryId].height = height;
	}
	[UnmanagedCallersOnly(CallConvs = new Type[] {typeof(CallConvCdecl)})]
	private static void UpdateTexture(uint entryId, void* bitmapPtr){
		ULBitmap bitmap = new((IntPtr)bitmapPtr);

		Check();
		uint textureId = textures[(int)entryId].textureId;

		uint width = bitmap.Width;
		uint height = bitmap.Height;

		uint rowBytes = bitmap.RowBytes;
		uint bpp = bitmap.Bpp;

		if (DSA)
		{
			if(rowBytes != width * bpp){
				gl.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
				gl.PixelStore(PixelStoreParameter.UnpackRowLength, (int)(rowBytes / bpp));
			}

			void* pixels = (void*)bitmap.LockPixels();

			if (bitmap.Format is ULBitmapFormat.BGRA8_UNORM_SRGB)
			{
				gl.TextureSubImage2D(textureId, 0, 0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);
			}
			else
			{
				gl.TextureSubImage2D(textureId, 0, 0, 0, width, height, PixelFormat.Red, PixelType.UnsignedByte, pixels);
			}

			bitmap.UnlockPixels();

			//gl.GenerateTextureMipmap(textureId);
		}
		else
		{
			gl.ActiveTexture(TextureUnit.Texture0);
			gl.BindTexture(TextureTarget.Texture2D, textureId);

			if(rowBytes != width * bpp){
				gl.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
				gl.PixelStore(PixelStoreParameter.UnpackRowLength, (int)(rowBytes / bpp));
			}

			void* pixels = (void*)bitmap.LockPixels();

			if (bitmap.Format is ULBitmapFormat.BGRA8_UNORM_SRGB)
			{
				gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgb8, width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);
			}
			else
			{
				gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.R8, width, height, 0, PixelFormat.Red, PixelType.UnsignedByte, pixels);
			}

			bitmap.UnlockPixels();

			//gl.GenerateMipmap(TextureTarget.Texture2D);
		}

		Check();
	}
	[UnmanagedCallersOnly(CallConvs = new Type[] {typeof(CallConvCdecl)})]
	private static void DestroyTexture(uint id){
		var entry = textures[(int)id];

		gl.DeleteTexture(entry.textureId);
		if (entry.multisampledTextureId is not 0) gl.DeleteTexture(entry.multisampledTextureId);

		freeTextures.Push((int)id);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] {typeof(CallConvCdecl)})]
	private static void CreateRenderBuffer(uint id, ULRenderBuffer renderBuffer){
		var entry = renderBuffers[(int)id];

		entry.textureEntry = textures[(int)renderBuffer.TextureId];

		uint framebuffer;
		uint multisampledFramebuffer = 0;
		uint textureGLId = entry.textureEntry.textureId;
		uint multisampledTextureId = entry.textureEntry.multisampledTextureId;

		if (DSA)
		{
			framebuffer = gl.CreateFramebuffer();
			gl.NamedFramebufferTexture(framebuffer, FramebufferAttachment.ColorAttachment0, textureGLId, 0);
			gl.NamedFramebufferDrawBuffer(framebuffer, ColorBuffer.ColorAttachment0);

			if (multisampledTextureId is not 0)
			{
				multisampledFramebuffer = gl.CreateFramebuffer();
				gl.NamedFramebufferTexture(multisampledFramebuffer, FramebufferAttachment.ColorAttachment0, multisampledTextureId, 0);
				gl.NamedFramebufferDrawBuffer(multisampledFramebuffer, ColorBuffer.ColorAttachment0);
			}

#if DEBUG
			var status = gl.CheckNamedFramebufferStatus(framebuffer, FramebufferTarget.Framebuffer);
			if (status is not GLEnum.FramebufferComplete)
			{
				throw new Exception(status.ToString());
			}
#endif
		}
		else
		{
			framebuffer = gl.GenFramebuffer();
			gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
			gl.BindTexture(TextureTarget.Texture2D, textureGLId);
			gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, textureGLId, 0);
			gl.DrawBuffer(DrawBufferMode.ColorAttachment0);

			if (multisampledTextureId is not 0)
			{
				multisampledFramebuffer = gl.GenFramebuffer();
				gl.BindFramebuffer(FramebufferTarget.Framebuffer, multisampledFramebuffer);
				gl.BindTexture(TextureTarget.Texture2DMultisample, multisampledTextureId);
				gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2DMultisample, multisampledTextureId, 0);
				gl.DrawBuffer(DrawBufferMode.ColorAttachment0);
			}

#if DEBUG
			var status = gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
			if (status is not GLEnum.FramebufferComplete)
			{
				throw new Exception(status.ToString());
			}
#endif
			gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		}
		entry.textureEntry.framebuffer = framebuffer;
		entry.textureEntry.multisampledFramebuffer = multisampledFramebuffer;
	}
	[UnmanagedCallersOnly(CallConvs = new Type[] {typeof(CallConvCdecl)})]
	private static void DestroyRenderBuffer(uint id){
		var entry = renderBuffers[(int)id];
		gl.DeleteFramebuffer(entry.textureEntry.framebuffer);
		if(entry.textureEntry.multisampledFramebuffer is not 0) gl.DeleteFramebuffer(entry.textureEntry.multisampledFramebuffer);
		freeRenderBuffers.Push((int)id);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] {typeof(CallConvCdecl)})]
	private static void CreateGeometry(uint id, ULVertexBuffer vb, ULIndexBuffer ib){
		var entry = geometries[(int)id];

		uint vao, vbo, ebo;

		if (DSA)
		{
			vao = gl.CreateVertexArray();
			vbo = gl.CreateBuffer();
			ebo = gl.CreateBuffer();

			gl.NamedBufferData(vbo, vb.size, vb.data, GLEnum.StaticDraw);
			gl.NamedBufferData(ebo, ib.size, ib.data, GLEnum.StaticDraw);

			if (vb.Format is ULVertexBufferFormat.VBF_2f_4ub_2f_2f_28f)
			{
				gl.EnableVertexArrayAttrib(vao, 0);
				gl.VertexArrayAttribBinding(vao, 0, 0);
				gl.VertexArrayAttribFormat(vao, 0, 2, GLEnum.Float, false, 0);
				gl.EnableVertexArrayAttrib(vao, 1);
				gl.VertexArrayAttribBinding(vao, 1, 0);
				gl.VertexArrayAttribFormat(vao, 1, 4, GLEnum.UnsignedByte, true, 8);
				gl.EnableVertexArrayAttrib(vao, 2);
				gl.VertexArrayAttribBinding(vao, 2, 0);
				gl.VertexArrayAttribFormat(vao, 2, 2, GLEnum.Float, false, 12);
				gl.EnableVertexArrayAttrib(vao, 3);
				gl.VertexArrayAttribBinding(vao, 3, 0);
				gl.VertexArrayAttribFormat(vao, 3, 2, GLEnum.Float, false, 20);
				gl.EnableVertexArrayAttrib(vao, 4);
				gl.VertexArrayAttribBinding(vao, 4, 0);
				gl.VertexArrayAttribFormat(vao, 4, 4, GLEnum.Float, false, 28);
				gl.EnableVertexArrayAttrib(vao, 5);
				gl.VertexArrayAttribBinding(vao, 5, 0);
				gl.VertexArrayAttribFormat(vao, 5, 4, GLEnum.Float, false, 44);
				gl.EnableVertexArrayAttrib(vao, 6);
				gl.VertexArrayAttribBinding(vao, 6, 0);
				gl.VertexArrayAttribFormat(vao, 6, 4, GLEnum.Float, false, 60);
				gl.EnableVertexArrayAttrib(vao, 7);
				gl.VertexArrayAttribBinding(vao, 7, 0);
				gl.VertexArrayAttribFormat(vao, 7, 4, GLEnum.Float, false, 76);
				gl.EnableVertexArrayAttrib(vao, 8);
				gl.VertexArrayAttribBinding(vao, 8, 0);
				gl.VertexArrayAttribFormat(vao, 8, 4, GLEnum.Float, false, 92);
				gl.EnableVertexArrayAttrib(vao, 9);
				gl.VertexArrayAttribBinding(vao, 9, 0);
				gl.VertexArrayAttribFormat(vao, 9, 4, GLEnum.Float, false, 108);
				gl.EnableVertexArrayAttrib(vao, 10);
				gl.VertexArrayAttribBinding(vao, 10, 0);
				gl.VertexArrayAttribFormat(vao, 10, 4, GLEnum.Float, false, 124);

				gl.VertexArrayVertexBuffer(vao, 0, vbo, 0, 140);
			}
			else
			{
				gl.EnableVertexArrayAttrib(vao, 0);
				gl.VertexArrayAttribBinding(vao, 0, 0);
				gl.VertexArrayAttribFormat(vao, 0, 2, GLEnum.Float, false, 0);
				gl.EnableVertexArrayAttrib(vao, 1);
				gl.VertexArrayAttribBinding(vao, 1, 0);
				gl.VertexArrayAttribFormat(vao, 1, 4, GLEnum.UnsignedByte, true, 8);
				gl.EnableVertexArrayAttrib(vao, 2);
				gl.VertexArrayAttribBinding(vao, 2, 0);
				gl.VertexArrayAttribFormat(vao, 2, 2, GLEnum.Float, false, 12);

				gl.VertexArrayVertexBuffer(vao, 0, vbo, 0, 20);
			}

			gl.VertexArrayElementBuffer(vao, ebo);
		}
		else
		{
			vao = gl.GenVertexArray();

			gl.BindVertexArray(vao);

			vbo = gl.GenBuffer();

			gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
			gl.BufferData(BufferTargetARB.ArrayBuffer, vb.size, vb.data, BufferUsageARB.DynamicDraw);

			if (vb.Format is ULVertexBufferFormat.VBF_2f_4ub_2f_2f_28f)
			{
				const uint stride = 140;

				gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, (void*)0);
				gl.VertexAttribPointer(1, 4, VertexAttribPointerType.UnsignedByte, true, stride, (void*)8);
				gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, (void*)12);
				gl.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, stride, (void*)20);
				gl.VertexAttribPointer(4, 4, VertexAttribPointerType.Float, false, stride, (void*)28);
				gl.VertexAttribPointer(5, 4, VertexAttribPointerType.Float, false, stride, (void*)44);
				gl.VertexAttribPointer(6, 4, VertexAttribPointerType.Float, false, stride, (void*)60);
				gl.VertexAttribPointer(7, 4, VertexAttribPointerType.Float, false, stride, (void*)76);
				gl.VertexAttribPointer(8, 4, VertexAttribPointerType.Float, false, stride, (void*)92);
				gl.VertexAttribPointer(9, 4, VertexAttribPointerType.Float, false, stride, (void*)108);
				gl.VertexAttribPointer(10, 4, VertexAttribPointerType.Float, false, stride, (void*)124);

				gl.EnableVertexAttribArray(0);
				gl.EnableVertexAttribArray(1);
				gl.EnableVertexAttribArray(2);
				gl.EnableVertexAttribArray(3);
				gl.EnableVertexAttribArray(4);
				gl.EnableVertexAttribArray(5);
				gl.EnableVertexAttribArray(6);
				gl.EnableVertexAttribArray(7);
				gl.EnableVertexAttribArray(8);
				gl.EnableVertexAttribArray(9);
				gl.EnableVertexAttribArray(10);
			}
			else
			{
				const uint stride = 20;

				gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, (void*)0);
				gl.VertexAttribPointer(1, 4, VertexAttribPointerType.UnsignedByte, true, stride, (void*)8);
				gl.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, (void*)12);

				gl.EnableVertexAttribArray(0);
				gl.EnableVertexAttribArray(1);
				gl.EnableVertexAttribArray(2);
			}

			ebo = gl.GenBuffer();

			gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
			gl.BufferData(BufferTargetARB.ElementArrayBuffer, ib.size, ib.data, BufferUsageARB.DynamicDraw);

			gl.BindVertexArray(0);
		}

		entry.vao = vao;
		entry.vbo = vbo;
		entry.ebo = ebo;
	}
	[UnmanagedCallersOnly(CallConvs = new Type[] {typeof(CallConvCdecl)})]
	private static void UpdateGeometry(uint id, ULVertexBuffer vb, ULIndexBuffer ib){
		var entry = geometries[(int)id];

		if (DSA)
		{
			gl.NamedBufferData(entry.vbo, vb.size, vb.data, GLEnum.StaticDraw);
			gl.NamedBufferData(entry.ebo, ib.size, ib.data, GLEnum.StaticDraw);
		}
		else
		{
			gl.BindVertexArray(entry.vao);
			gl.BindBuffer(BufferTargetARB.ArrayBuffer, entry.vbo);
			gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)vb.size, vb.data, BufferUsageARB.DynamicDraw);
			gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, entry.ebo);
			gl.BufferData(BufferTargetARB.ElementArrayBuffer, ib.size, ib.data, BufferUsageARB.DynamicDraw);
			gl.BindVertexArray(0);
		}
		Check();
	}
	[UnmanagedCallersOnly(CallConvs = new Type[] {typeof(CallConvCdecl)})]
	private static void DestroyGeometry(uint id){
		var entry = geometries[(int)id];

		gl.DeleteBuffer(entry.ebo);
		gl.DeleteBuffer(entry.vbo);
		gl.DeleteVertexArray(entry.vao);

		freeGeometry.Push((int)id);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] {typeof(CallConvCdecl)})]
	private static void UpdateCommandList(ULCommandList commandList){
		Check();
		uint glLastProgram = default;
		uint glProgram = default;
		uint glViewportWidth = unchecked((uint)-1);
		uint glViewportHeight = unchecked((uint)-1);

		if (commandList.size is 0) return; // Skip everything

		gl.GetInteger(GetPName.CurrentProgram, (int*)&glLastProgram);
		glProgram = glLastProgram;
		gl.Disable(EnableCap.ScissorTest);
		gl.Disable(EnableCap.DepthTest);
		gl.DepthFunc(DepthFunction.Never);
		gl.Enable(EnableCap.Blend);
		gl.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);

		if (!DSA)
			gl.BindBuffer(BufferTargetARB.UniformBuffer, UBO);


		uint currentFramebuffer = uint.MaxValue;
		ULIntRect? currentScissors = null;
		bool currentBlend = true;

		Uniforms uniforms = default;

		var commandSpan = commandList.ToSpan();
		foreach (var command in commandSpan)
		{
			var gpuState = command.GPUState;
			var renderBufferEntry = renderBuffers[(int)gpuState.RenderBufferId];
			var rtTextureEntry = renderBufferEntry.textureEntry;

			var framebufferToUse = rtTextureEntry.multisampledFramebuffer is not 0 ? rtTextureEntry.multisampledFramebuffer : rtTextureEntry.framebuffer;
			if (currentFramebuffer != framebufferToUse)
			{
				gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferToUse);
				currentFramebuffer = framebufferToUse;
				renderBufferEntry.dirty = true;
				if (rtTextureEntry.multisampledFramebuffer != rtTextureEntry.framebuffer && rtTextureEntry.multisampledFramebuffer is not 0) rtTextureEntry.needsConversion = true;
			}
			if (command.CommandType is ULCommandType.DrawGeometry)
			{
				if (glViewportWidth != gpuState.ViewportWidth || glViewportHeight != gpuState.ViewportHeight) // Set viewport
				{
					gl.Viewport(0, 0, gpuState.ViewportWidth, gpuState.ViewportHeight);
					glViewportWidth = gpuState.ViewportWidth;
					glViewportHeight = gpuState.ViewportHeight;
				}

				// Select program to use
				{
					uint program = gpuState.ShaderType is ULShaderType.Fill ? fillProgram : pathProgram;
					if (glProgram != program) gl.UseProgram(program);
					glProgram = program;
				}

				var geometryEntry = geometries[(int)command.GeometryId];

				#region Uniforms
				uniforms.State.X = gpuState.ViewportWidth;
				uniforms.State.Y = gpuState.ViewportHeight;
				uniforms.Transform = gpuState.Transform.ApplyProjection(gpuState.ViewportWidth, gpuState.ViewportHeight, true);
				gpuState.Scalar.CopyTo(new Span<float>(&uniforms.Scalar4_0.W, 8));
				gpuState.Vector.CopyTo(new Span<Vector4>(&uniforms.Vector_0, 8));
				gpuState.Clip.CopyTo(new Span<Matrix4x4>(&uniforms.Clip_0.M11, 8));
				uniforms.ClipSize = (uint)gpuState.ClipSize;

				if (DSA)
					gl.NamedBufferData(UBO, 768, &uniforms, GLEnum.DynamicDraw);
				else
					gl.BufferData(BufferTargetARB.UniformBuffer, 768, &uniforms, BufferUsageARB.DynamicDraw);
				#endregion Uniforms

				gl.BindVertexArray(geometryEntry.vao);

				bool rebindFramebuffer = false;

				if (gpuState.ShaderType is ULShaderType.Fill)
				{
					Debug.Assert(gpuState.Texture1Id != 0);
					if ((uint)textures.Count > gpuState.Texture1Id)
					{
						TextureEntry textureEntry = textures[(int)gpuState.Texture1Id];
						if (DSA)
						{
							#if false
							if (textureEntry.needsConversion)
								gl.BlitNamedFramebuffer(textureEntry.multisampledFramebuffer, textureEntry.framebuffer, 0, 0, (int)textureEntry.width, (int)textureEntry.height, 0, 0, (int)textureEntry.width, (int)textureEntry.height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);
							#else
							if (textureEntry.needsConversion)
							{
								gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, textureEntry.multisampledFramebuffer);
								gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, textureEntry.framebuffer);
								gl.BlitFramebuffer(0, 0, (int)textureEntry.width, (int)textureEntry.height, 0, 0, (int)textureEntry.width, (int)textureEntry.height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);
								rebindFramebuffer = true;
							}
							#endif
							gl.BindTextureUnit(0, textureEntry.textureId);
						}
						else
						{
							if (textureEntry.needsConversion)
							{
								gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, textureEntry.multisampledFramebuffer);
								gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, textureEntry.framebuffer);
								gl.BlitFramebuffer(0, 0, (int)textureEntry.width, (int)textureEntry.height, 0, 0, (int)textureEntry.width, (int)textureEntry.height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);
								rebindFramebuffer = true;
							}
							gl.ActiveTexture(GLEnum.Texture0);
							gl.BindTexture(GLEnum.Texture2D, textureEntry.textureId);
						}
					}
					if (textures.Count > gpuState.Texture2Id && gpuState.Texture2Id is not 0)
					{
						TextureEntry textureEntry = textures[(int)gpuState.Texture2Id];
						if (DSA)
						{
							#if false
							if (textureEntry.needsConversion)
								gl.BlitNamedFramebuffer(textureEntry.multisampledFramebuffer, textureEntry.framebuffer, 0, 0, (int)textureEntry.width, (int)textureEntry.height, 0, 0, (int)textureEntry.width, (int)textureEntry.height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);
							#else
							if (textureEntry.needsConversion)
							{
								gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, textureEntry.multisampledFramebuffer);
								gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, textureEntry.framebuffer);
								gl.BlitFramebuffer(0, 0, (int)textureEntry.width, (int)textureEntry.height, 0, 0, (int)textureEntry.width, (int)textureEntry.height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);
								rebindFramebuffer = true;
							}
							#endif
							gl.BindTextureUnit(1, textureEntry.textureId);
						}
						else
						{
							if (textureEntry.needsConversion)
							{
								gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, textureEntry.multisampledFramebuffer);
								gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, textureEntry.framebuffer);
								gl.BlitFramebuffer(0, 0, (int)textureEntry.width, (int)textureEntry.height, 0, 0, (int)textureEntry.width, (int)textureEntry.height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);
								rebindFramebuffer = true;
							}
							gl.ActiveTexture(GLEnum.Texture1);
							gl.BindTexture(GLEnum.Texture2D, textureEntry.textureId);
						}
					}
				}

				if(rebindFramebuffer)
					gl.BindFramebuffer(FramebufferTarget.Framebuffer, currentFramebuffer);

				if(currentScissors is not null != gpuState.EnableScissor){
					if(gpuState.EnableScissor){
						if(currentScissors is null) gl.Enable(EnableCap.ScissorTest);
						gl.Scissor(gpuState.ScissorRect.Left, gpuState.ScissorRect.Top, (uint)(gpuState.ScissorRect.Right - gpuState.ScissorRect.Left), (uint)(gpuState.ScissorRect.Bottom - gpuState.ScissorRect.Top));
						currentScissors = gpuState.ScissorRect;
					} else {
						gl.Disable(EnableCap.ScissorTest);
						currentScissors = null;
					}
				}

				if(currentBlend != gpuState.EnableBlend)
				{
					if(gpuState.EnableBlend)
						gl.Enable(EnableCap.Blend);
					else
						gl.Disable(EnableCap.Blend);
					currentBlend = gpuState.EnableBlend;
				}

				gl.DrawElements(PrimitiveType.Triangles, command.IndicesCount, DrawElementsType.UnsignedInt, (void*)(command.IndicesOffset * sizeof(uint)));
			}
			else if (command.CommandType is ULCommandType.ClearRenderBuffer)
			{
				gl.Disable(EnableCap.ScissorTest);
				currentScissors = null;
				gl.ClearColor(0, 0, 0, 0);
				gl.Clear((uint)GLEnum.ColorBufferBit);
			}
			else throw new Exception($"Invalid {nameof(ULCommandType)} value.");
		}

		gl.UseProgram(glLastProgram);

		gl.BindVertexArray(0);
		gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		Check();
	}

	// TODO: delete if useless
	public class TextureEntry
	{
		public uint textureId;
		public uint framebuffer;

		public uint multisampledTextureId;
		public uint multisampledFramebuffer;

		/// <summary>
		/// Copy multisampledFramebuffer data to framebuffer
		/// </summary>
		public bool needsConversion;

		// blitting arguments
		public uint width;
		public uint height;
	}
	private class GeometryEntry
	{
		public uint vao;
		public uint vbo;
		public uint ebo;
	}
	public class RenderBufferEntry
	{
		public TextureEntry textureEntry;
		public bool dirty;
	}
}
