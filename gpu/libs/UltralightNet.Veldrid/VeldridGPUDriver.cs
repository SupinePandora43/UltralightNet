using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using UltralightNet.GPUCommon;
using UltralightNet.Platform;
using Veldrid;
using Veldrid.SPIRV;

namespace UltralightNet.GPU.Veldrid;

public unsafe sealed class VeldridGPUDriver : IGPUDriver, IDisposable
{
	readonly GraphicsDevice graphicsDevice;
	readonly bool IsVulkan;

	readonly ResourceLayout uniformsResourceLayout;
	readonly ResourceLayout textureResourceLayout;

	readonly Sampler sampler;
	DeviceBuffer? uniformBuffer;
	ResourceSet? uniformSet;

	// TODO: MSAA
	// readonly TextureSampleCount SampleCount = TextureSampleCount.Count1;

	readonly Pipeline fillPipeline;
	readonly Pipeline fillWithoutBlendPipeline;
	readonly Pipeline fillPathPipeline;

	readonly Dictionary<uint, TextureEntry> TextureEntries = new();
	readonly Dictionary<uint, GeometryEntry> GeometryEntries = new();
	readonly Dictionary<uint, RenderBufferEntry> RenderBufferEntries = new();

	readonly bool IsDirectX = false;

	Uniforms[]? FakeMappedUniformBuffer;

	public VeldridGPUDriver(GraphicsDevice graphicsDevice)
	{
		this.graphicsDevice = graphicsDevice;

		IsVulkan = graphicsDevice.BackendType is GraphicsBackend.Vulkan;

		{ // ResourceLayout
			uniformsResourceLayout = graphicsDevice.ResourceFactory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription(
						"Uniforms",
						ResourceKind.UniformBuffer,
						ShaderStages.Vertex | ShaderStages.Fragment,
						IsVulkan ? ResourceLayoutElementOptions.DynamicBinding : ResourceLayoutElementOptions.None
					),
					new ResourceLayoutElementDescription("Sampler", ResourceKind.Sampler, ShaderStages.Fragment) // TODO: make sure that stage doesn't cause problems.
				)
			);
			textureResourceLayout = graphicsDevice.ResourceFactory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("texture", ResourceKind.TextureReadOnly, ShaderStages.Fragment)
				)
			);
		}
		{ // Sampler
			sampler = graphicsDevice.ResourceFactory.CreateSampler(new(
				SamplerAddressMode.Clamp,
				SamplerAddressMode.Clamp,
				SamplerAddressMode.Clamp,
				SamplerFilter.MinLinear_MagPoint_MipLinear,
				ComparisonKind.Never,
				1,
				0,
				1,
				0,
				SamplerBorderColor.TransparentBlack
			));
		}
		{ // Uniform Buffer
			if (!IsVulkan)
			{
				uniformBuffer = graphicsDevice.ResourceFactory.CreateBuffer(new(768, BufferUsage.UniformBuffer));
				uniformSet = graphicsDevice.ResourceFactory.CreateResourceSet(new(uniformsResourceLayout, uniformBuffer, sampler));
			}
		}
		{ // 0
			var emptyTexture = graphicsDevice.ResourceFactory.CreateTexture(
				new(2, 2,
					1, 1, 1,
					PixelFormat.R8_UNorm,
					TextureUsage.Sampled,
					TextureType.Texture2D
				)
			);
			var emptyResourceSet = graphicsDevice.ResourceFactory.CreateResourceSet(
				new ResourceSetDescription(
					textureResourceLayout,
					emptyTexture
				)
			);
			TextureEntries.Add(0, new() { texture = emptyTexture, resourceSet = emptyResourceSet });
		}
		{ // Pipelines
			static byte[] GetEmbeddedShaderBytes(string name)
			{
				var stream = typeof(VeldridGPUDriver).Assembly.GetManifestResourceStream(name) ?? throw new FileNotFoundException("Couldn't load embedded .spv file.");
				var bytes = new byte[stream.Length];
				stream.Read(bytes);
				return bytes;
			}

			var fillShaders = graphicsDevice.ResourceFactory.CreateFromSpirv(
				new ShaderDescription(ShaderStages.Vertex, GetEmbeddedShaderBytes("UltralightNet.Veldrid.shader_fill.vert.spv"), "main"),
				new ShaderDescription(ShaderStages.Fragment, GetEmbeddedShaderBytes("UltralightNet.Veldrid.shader_fill.frag.spv"), "main"));
			var fillPathShaders = graphicsDevice.ResourceFactory.CreateFromSpirv(
				new ShaderDescription(ShaderStages.Vertex, GetEmbeddedShaderBytes("UltralightNet.Veldrid.shader_fill_path.vert.spv"), "main"),
				new ShaderDescription(ShaderStages.Fragment, GetEmbeddedShaderBytes("UltralightNet.Veldrid.shader_fill_path.frag.spv"), "main"));

			var fillShaderSetDescription = new ShaderSetDescription(new VertexLayoutDescription[]{new(140,
				new VertexElementDescription("in_Position", HLSL_to_any(VertexElementSemantic.Position), VertexElementFormat.Float2),
				new("in_Color", HLSL_to_any(VertexElementSemantic.Color), VertexElementFormat.Byte4_Norm),
				new("in_TexCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
				new("in_ObjCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
				new("in_Data0", HLSL_to_any(VertexElementSemantic.Color), VertexElementFormat.Float4),
				new("in_Data1", HLSL_to_any(VertexElementSemantic.Color), VertexElementFormat.Float4),
				new("in_Data2", HLSL_to_any(VertexElementSemantic.Color), VertexElementFormat.Float4),
				new("in_Data3", HLSL_to_any(VertexElementSemantic.Color), VertexElementFormat.Float4),
				new("in_Data4", HLSL_to_any(VertexElementSemantic.Color), VertexElementFormat.Float4),
				new("in_Data5", HLSL_to_any(VertexElementSemantic.Color), VertexElementFormat.Float4),
				new("in_Data6", HLSL_to_any(VertexElementSemantic.Color), VertexElementFormat.Float4))}, fillShaders);
			var fillPathShaderSetDescription = fillShaderSetDescription with { VertexLayouts = new[] { fillShaderSetDescription.VertexLayouts[0] with { Stride = 20, Elements = fillShaderSetDescription.VertexLayouts[0].Elements[0..3] } }, Shaders = fillPathShaders };

			GraphicsPipelineDescription ultralightPipelineDescription = new(
				new(default, new[] { new BlendAttachmentDescription(true,
					BlendFactor.One,
					BlendFactor.InverseDestinationAlpha,
					BlendFunction.Add,
					BlendFactor.InverseDestinationAlpha,
					BlendFactor.One,
					BlendFunction.Add) })
				{
					AttachmentStates = new[] {
						// glBlendFunc(GL_ONE, GL_ONE_MINUS_SRC_ALPHA);
						new BlendAttachmentDescription()
						{
							/*SourceColorFactor = BlendFactor.One,
							SourceAlphaFactor = BlendFactor.InverseDestinationAlpha,
							DestinationColorFactor = BlendFactor.InverseSourceAlpha,
							DestinationAlphaFactor = BlendFactor.One,*/
							SourceColorFactor = BlendFactor.One,
							SourceAlphaFactor = BlendFactor.InverseDestinationAlpha,//BlendFactor.One,
							DestinationColorFactor = BlendFactor.InverseSourceAlpha,
							DestinationAlphaFactor = BlendFactor.One, // InverseSourceAlpha
							BlendEnabled = true,
							ColorFunction = BlendFunction.Add,
							AlphaFunction = BlendFunction.Add
						}
					}
				},
				DepthStencilStateDescription.Disabled,
				new(FaceCullMode.None, PolygonFillMode.Solid, FrontFace.CounterClockwise, false, true),
				PrimitiveTopology.TriangleList,
				fillShaderSetDescription,
				new ResourceLayout[]
				{
					uniformsResourceLayout,
					textureResourceLayout,
					textureResourceLayout
				},
				new OutputDescription(null, new OutputAttachmentDescription(PixelFormat.B8_G8_R8_A8_UNorm_SRgb)),
				ResourceBindingModel.Default);

			fillPipeline = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ultralightPipelineDescription);
			fillWithoutBlendPipeline = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ultralightPipelineDescription with { BlendState = ultralightPipelineDescription.BlendState with { AttachmentStates = new[] { ultralightPipelineDescription.BlendState.AttachmentStates[0] with { BlendEnabled = false } } } });

			fillPathPipeline = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ultralightPipelineDescription with { ShaderSet = fillPathShaderSetDescription, ResourceLayouts = new[] { uniformsResourceLayout } });
		}
	}
	public void Dispose()
	{

		fillPipeline.Dispose();
		fillWithoutBlendPipeline.Dispose();
		fillPathPipeline.Dispose();
		sampler.Dispose();
		// TODO: dispose all textures
		textureResourceLayout.Dispose();
		uniformsResourceLayout.Dispose();
	}

	public CommandList CommandList { get; set; } = null;

	#region NextId
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint GetKey<TValue>(Dictionary<uint, TValue> dictionary)
	{
		for (uint i = 1; ; i++)
		{
			if (!dictionary.ContainsKey(i))
				return i;
		}
	}
	uint IGPUDriver.NextTextureId()
	{
		uint id = GetKey(TextureEntries);
		TextureEntries.Add(id, new());
		return id;
	}
	uint IGPUDriver.NextGeometryId()
	{
		uint id = GetKey(GeometryEntries);
		GeometryEntries.Add(id, new());
		return id;
	}
	uint IGPUDriver.NextRenderBufferId()
	{
		uint id = GetKey(RenderBufferEntries);
		RenderBufferEntries.Add(id, new());
		return id;
	}
	#endregion NextId
	#region Texture
	[Obsolete]
	void UploadTexture(TextureEntry texture, ULBitmap bitmap, uint width, uint height, uint bpp, uint rowBytes)
	{
		byte* pixelsPTR = bitmap.LockPixels();
		graphicsDevice.UpdateTexture(texture.texture, (IntPtr)pixelsPTR, width * height * bpp, 0, 0, 0, width, height, 1, 0, 0);
		bitmap.UnlockPixels();
	}
	void IGPUDriver.CreateTexture(uint texture_id, ULBitmap bitmap)
	{
		bool isRT = bitmap.IsEmpty;
		TextureEntry entry = TextureEntries[texture_id];

		uint width = bitmap.Width;
		uint height = bitmap.Height;
		uint bpp = bitmap.Bpp;

		TextureDescription textureDescription = new(
			width, height,
			1, 1, 1,
			bpp is 4 ? PixelFormat.B8_G8_R8_A8_UNorm_SRgb : PixelFormat.R8_UNorm,
			TextureUsage.Sampled | (isRT ? TextureUsage.RenderTarget : 0), TextureType.Texture2D);

		if (isRT)
		{
			textureDescription.Usage |= TextureUsage.RenderTarget;
		}

		entry.texture = graphicsDevice.ResourceFactory.CreateTexture(textureDescription);

		if (!isRT)
		{
			uint rowBytes = bitmap.RowBytes;
			Debug.Assert(bpp * width == rowBytes);
			UploadTexture(entry, bitmap, width, height, bpp, rowBytes);
		}

		entry.resourceSet = graphicsDevice.ResourceFactory.CreateResourceSet(
			new ResourceSetDescription(
				textureResourceLayout,
				entry.texture
			)
		);
	}
	void IGPUDriver.UpdateTexture(uint texture_id, ULBitmap bitmap)
	{
		TextureEntry entry = TextureEntries[texture_id];

		uint height = bitmap.Height;
		uint width = bitmap.Width;
		uint bpp = bitmap.Bpp;

		UploadTexture(entry, bitmap, width, height, bpp, bitmap.RowBytes);
	}
	void IGPUDriver.DestroyTexture(uint texture_id)
	{
		TextureEntries.Remove(texture_id, out TextureEntry entry);
		entry.resourceSet.Dispose();
		entry.resourceSet = null;
		entry.texture.Dispose();
		entry.texture = null;
	}
	#endregion Texture
	#region Geometry
	void IGPUDriver.CreateGeometry(uint geometry_id, ULVertexBuffer vertices, ULIndexBuffer indices)
	{
		GeometryEntry entry = GeometryEntries[geometry_id];

		BufferDescription vertexDescription = new(vertices.size, BufferUsage.VertexBuffer);
		entry.vertices = graphicsDevice.ResourceFactory.CreateBuffer(ref vertexDescription);
		BufferDescription indexDescription = new(indices.size, BufferUsage.IndexBuffer);
		entry.indicies = graphicsDevice.ResourceFactory.CreateBuffer(ref indexDescription);

		graphicsDevice.UpdateBuffer(entry.vertices, 0, (IntPtr)vertices.data, vertices.size);
		graphicsDevice.UpdateBuffer(entry.indicies, 0, (IntPtr)indices.data, indices.size);
	}
	void IGPUDriver.UpdateGeometry(uint geometry_id, ULVertexBuffer vertices, ULIndexBuffer indices)
	{
		//Console.WriteLine($"UpdateGeometry({geometry_id})");
		GeometryEntry entry = GeometryEntries[geometry_id];

		graphicsDevice.UpdateBuffer(entry.vertices, 0, (IntPtr)vertices.data, vertices.size);
		graphicsDevice.UpdateBuffer(entry.indicies, 0, (IntPtr)indices.data, indices.size);
	}
	void IGPUDriver.DestroyGeometry(uint geometry_id)
	{
		GeometryEntries.Remove(geometry_id, out GeometryEntry entry);

		entry.vertices.Dispose();
		entry.indicies.Dispose();
	}
	#endregion
	#region RenderBuffer
	void IGPUDriver.CreateRenderBuffer(uint render_buffer_id, ULRenderBuffer buffer)
	{
		RenderBufferEntry entry = RenderBufferEntries[render_buffer_id];
		TextureEntry textureEntry = TextureEntries[buffer.TextureId];

		entry.textureEntry = textureEntry;

		FramebufferDescription fd = new()
		{
			ColorTargets = new[] {
					new FramebufferAttachmentDescription(textureEntry.texture, 0)
				}
		};

		entry.framebuffer = graphicsDevice.ResourceFactory.CreateFramebuffer(ref fd);
	}
	void IGPUDriver.DestroyRenderBuffer(uint render_buffer_id)
	{
		if (RenderBufferEntries.Remove(render_buffer_id, out var entry))
		{
			entry.textureEntry = null;
			entry.framebuffer.Dispose();
			entry.framebuffer = null;
		}
	}
	#endregion RenderBuffer

	void IGPUDriver.UpdateCommandList(ULCommandList list)
	{
		if (list.size is 0) return;

		uint commandId = 0;

		if (IsVulkan)
		{
			if (uniformBuffer is null || uniformSet is null || FakeMappedUniformBuffer?.Length < list.size)
			{
				uniformSet?.Dispose();
				uniformBuffer?.Dispose();

				uniformBuffer = graphicsDevice.ResourceFactory.CreateBuffer(new BufferDescription((uint)768 * list.size, BufferUsage.UniformBuffer | BufferUsage.Dynamic));

				uniformSet = graphicsDevice.ResourceFactory.CreateResourceSet(new(uniformsResourceLayout, uniformBuffer, sampler));

				FakeMappedUniformBuffer = GC.AllocateUninitializedArray<Uniforms>((int)list.size);
			}

			Span<Uniforms> uniformSpan = FakeMappedUniformBuffer; // implicit conversion :)

			foreach (ULCommand command in list.AsSpan())
			{
				if (command.CommandType is ULCommandType.DrawGeometry)
				{
					ULGPUState state = command.GPUState;
					Unsafe.SkipInit(out Uniforms uniforms);
					uniforms.State.X = state.ViewportWidth;
					uniforms.State.Y = state.ViewportHeight;
					uniforms.Transform = state.Transform.ApplyProjection(state.ViewportWidth, state.ViewportHeight, true);
					state.Scalar.CopyTo(new Span<float>(&uniforms.Scalar4_0.W, 8));
					state.Vector.CopyTo(new Span<Vector4>(&uniforms.Vector_0.W, 8));
					state.Clip.CopyTo(new Span<Matrix4x4>(&uniforms.Clip_0.M11, 8));
					uniforms.ClipSize = state.ClipSize;
					uniformSpan[(int)commandId++] = uniforms;
				}
			}

			CommandList.UpdateBuffer(uniformBuffer, 0, (ReadOnlySpan<Uniforms>)uniformSpan);

			commandId = 0;
		}

		foreach (ULCommand command in list.AsSpan())
		{
			RenderBufferEntry renderBufferEntry = RenderBufferEntries[command.GPUState.RenderBufferId];

			CommandList.SetFramebuffer(renderBufferEntry.framebuffer);

			if (command.CommandType is ULCommandType.ClearRenderBuffer)
			{
				CommandList.SetFullScissorRect(0);
				CommandList.ClearColorTarget(0, RgbaFloat.Clear);
			}
			else if (command.CommandType is ULCommandType.DrawGeometry)
			{
				ULGPUState state = command.GPUState;
				if (state.ShaderType is ULShaderType.Fill)
				{
					CommandList.SetPipeline(state.EnableBlend ? fillPipeline : fillWithoutBlendPipeline);

					CommandList.SetGraphicsResourceSet(1, TextureEntries[state.Texture1Id].resourceSet);
					CommandList.SetGraphicsResourceSet(2, TextureEntries[state.Texture2Id].resourceSet);
				}
				else
				{
					Debug.Assert(state.EnableBlend);
					CommandList.SetPipeline(fillPathPipeline);
				}

				CommandList.SetScissorRect(0, (uint)state.ScissorRect.Left, (uint)state.ScissorRect.Top, (uint)(state.ScissorRect.Right - state.ScissorRect.Left), (uint)(state.ScissorRect.Bottom - state.ScissorRect.Top));

				#region Uniforms
				if (IsVulkan)
				{
					uint offset = 768u * commandId++;
					CommandList.SetGraphicsResourceSet(0, uniformSet, 1, ref offset); // dynamic offset my beloved
				}
				else
				{
					Unsafe.SkipInit(out Uniforms uniforms);
					uniforms.State.X = state.ViewportWidth;
					uniforms.State.Y = state.ViewportHeight;
					uniforms.Transform = state.Transform.ApplyProjection(state.ViewportWidth, state.ViewportHeight, true);
					state.Scalar.CopyTo(new Span<float>(&uniforms.Scalar4_0.W, 8));
					state.Vector.CopyTo(new Span<Vector4>(&uniforms.Vector_0.W, 8));
					state.Clip.CopyTo(new Span<Matrix4x4>(&uniforms.Clip_0.M11, 8));
					uniforms.ClipSize = (uint)state.ClipSize;
					CommandList.UpdateBuffer(uniformBuffer, 0, ref uniforms);

					CommandList.SetGraphicsResourceSet(0, uniformSet);
				}
				#endregion Uniforms

				CommandList.SetViewport(0, new Viewport(0f, 0f, state.ViewportWidth, state.ViewportHeight, 0f, 1f));

				GeometryEntry geometryEntry = GeometryEntries[command.GeometryId];

				CommandList.SetVertexBuffer(0, geometryEntry.vertices);
				CommandList.SetIndexBuffer(geometryEntry.indicies, IndexFormat.UInt32);

				CommandList.DrawIndexed(
					command.IndicesCount,
					1,
					command.IndicesOffset,
					0,
					0
				);
			}
		}
	}

	/// <remarks>will throw exception when view doesn't have RenderTarget</remarks>
	/// <exception cref="KeyNotFoundException">When called on view without RenderTarget</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SkipLocalsInit]
	public ResourceSet GetRenderTarget(View view) => TextureEntries[view.RenderTarget.TextureId].resourceSet;

	private class TextureEntry
	{
		public Texture texture;
		public ResourceSet resourceSet;
	}
	private class GeometryEntry
	{
		public DeviceBuffer vertices;
		public DeviceBuffer indicies;
	}
	private class RenderBufferEntry
	{
		public Framebuffer framebuffer;
		public TextureEntry textureEntry;
	}

	private VertexElementSemantic HLSL_to_any(VertexElementSemantic hlsl_semantic) => IsDirectX ? hlsl_semantic : VertexElementSemantic.TextureCoordinate;

	/*private ShaderSetDescription FillPathShaderSetDescription() => new(
			new VertexLayoutDescription[] {
					new VertexLayoutDescription(
							20,
							new VertexElementDescription(
								"in_Position",
								HLSL_to_any(VertexElementSemantic.Position),
								VertexElementFormat.Float2
							),
							new VertexElementDescription(
								"in_Color",
								HLSL_to_any(VertexElementSemantic.Color),
								VertexElementFormat.Byte4_Norm
							),
							new VertexElementDescription(
								"in_TexCoord",
								VertexElementSemantic.TextureCoordinate,
								VertexElementFormat.Float2
							)
						)
			}, ultralightPathShaders
		);*/
}
