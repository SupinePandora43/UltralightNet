using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using UltralightNet.GPUCommon;
using Veldrid;
using Veldrid.SPIRV;

namespace UltralightNet.Veldrid
{
	public unsafe class VeldridGPUDriver
	{
		private readonly GraphicsDevice graphicsDevice;
		private readonly ResourceLayout textureResourceLayout;

		public bool GenerateMipMaps = false;
		public uint MipLevels = 1;
		public TextureSampleCount SampleCount = TextureSampleCount.Count1;
		public bool Debug = false;

		/// <summary>
		/// public only for <see cref="GetRenderTarget(View)"/> inlining
		/// </summary>
		public readonly Dictionary<uint, TextureEntry> TextureEntries = new();
		private readonly Dictionary<uint, GeometryEntry> GeometryEntries = new();
		private readonly Dictionary<uint, RenderBufferEntry> RenderBufferEntries = new();

		private readonly bool IsDirectX = false;
		private readonly bool IsOpenGL = false;

		public CommandList CommandList;
		/// <summary>
		/// used for mipmaps, etc
		/// </summary>
		private readonly CommandList _commandList;

		public float time = 1f;

		public VeldridGPUDriver(GraphicsDevice graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;

			textureResourceLayout = graphicsDevice.ResourceFactory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("texture", ResourceKind.TextureReadOnly, ShaderStages.Fragment)
				)
			);

			_commandList = graphicsDevice.ResourceFactory.CreateCommandList();

			//IsDirectX = graphicsDevice.BackendType is GraphicsBackend.Direct3D11;

			IsOpenGL = (graphicsDevice.BackendType is GraphicsBackend.OpenGL) || (graphicsDevice.BackendType is GraphicsBackend.OpenGLES);

			InitializeBuffers();
			InitShaders();
			InitFramebuffers();
			InitPipelines();

			emptyTexture = graphicsDevice.ResourceFactory.CreateTexture(
				new(
					2,
					2,
					1,
					1,
					1,
					PixelFormat.R8_UNorm,
					TextureUsage.Sampled,
					TextureType.Texture2D
				)
			);
			emptyResourceSet = graphicsDevice.ResourceFactory.CreateResourceSet(
				new ResourceSetDescription(
					textureResourceLayout,
					emptyTexture
				)
			);

			TextureEntries.Add(0, new() { texture = emptyTexture, resourceSet = emptyResourceSet });
		}
		public ULGPUDriver GetGPUDriver() => new()
		{
			BeginSynchronize = null,
			EndSynchronize = null,

			NextTextureId = NextTextureId,
			NextGeometryId = NextGeometryId,
			NextRenderBufferId = NextRenderBufferId,

			_CreateTexture = CreateTexture,
			_UpdateTexture = UpdateTexture,
			DestroyTexture = DestroyTexture,

			CreateGeometry = CreateGeometry,
			UpdateGeometry = UpdateGeometry,
			DestroyGeometry = DestroyGeometry,

			CreateRenderBuffer = CreateRenderBuffer,
			DestroyRenderBuffer = DestroyRenderBuffer,

			UpdateCommandList = UpdateCommandList
		};

		#region NextId
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[SkipLocalsInit]
		private static uint GetKey<TValue>(Dictionary<uint, TValue> dictionary)
		{
			for (uint i = 1; ; i++)
			{
				if (!dictionary.ContainsKey(i))
					return i;
			}
		}
		[SkipLocalsInit]
		private uint NextTextureId()
		{
			uint id = GetKey(TextureEntries);
			TextureEntries.Add(id, new());
#if DEBUG
			Console.WriteLine($"NextTextureId() = {id}");
#endif
			return id;
		}
		[SkipLocalsInit]
		private uint NextGeometryId()
		{
			uint id = GetKey(GeometryEntries);
			GeometryEntries.Add(id, new());
#if DEBUG
			Console.WriteLine($"NextGeometryId() = {id}");
#endif
			return id;
		}
		[SkipLocalsInit]
		private uint NextRenderBufferId()
		{
			uint id = GetKey(RenderBufferEntries);
			RenderBufferEntries.Add(id, new());
#if DEBUG
			Console.WriteLine($"NextRenderBufferId() = {id}");
#endif
			return id;
		}
		#endregion NextId
		#region Texture
		/// <summary>
		/// Thx https://github.com/TechnologicalPizza
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void Set2DTextureData(ReadOnlySpan<byte> pixels, uint width, uint height, int stride, uint bpp, Texture dst, uint dstX, uint dstY, uint dstZ, uint dstMipLevel, uint dstArrayLayer)
		{
			_commandList.Begin();

			TextureDescription stagingDesc = new(width, height, 1, 1, 1, dst.Format, TextureUsage.Staging, TextureType.Texture2D);
			using Texture staging = graphicsDevice.ResourceFactory.CreateTexture(ref stagingDesc);

			MappedResource mappedStaging = graphicsDevice.Map(staging, MapMode.Write);
			try
			{
				Span<byte> stagingSpan = new((void*)mappedStaging.Data, (int)mappedStaging.SizeInBytes);

				int rowPitch = (int)(width * bpp);
				int mappedRowPitch = (int)mappedStaging.RowPitch;

				for (int y = 0; y < height; y++)
				{
					ReadOnlySpan<byte> sourceRow = pixels.Slice(y * stride, rowPitch);
					Span<byte> stagingRow = stagingSpan.Slice(y * mappedRowPitch, mappedRowPitch);
					sourceRow.CopyTo(stagingRow);
				}
			}
#if DEBUG
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
#endif
			finally
			{
				graphicsDevice.Unmap(staging);
			}

			_commandList.CopyTexture(
				staging, 0, 0, 0, 0, 0,
				dst, dstX, dstY, dstZ, dstMipLevel, dstArrayLayer, width, height, 1, 1);
			_commandList.End();
			graphicsDevice.SubmitCommands(_commandList);
		}
		[SkipLocalsInit]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void UploadTexture(Texture texture, IntPtr bitmap, uint width, uint height, uint bpp)
		{
			IntPtr pixels = Methods.ulBitmapLockPixels(bitmap);

			uint rowBytes = Methods.ulBitmapGetRowBytes(bitmap);
			uint offset = rowBytes - (width * bpp);

			if (offset is 0)
			{
				graphicsDevice.UpdateTexture(texture, pixels, width * height * bpp, 0, 0, 0, width, height, 1, 0, 0);
			}
			else
			{
				unsafe
				{
					Set2DTextureData(new ReadOnlySpan<byte>((void*)pixels, (int)(rowBytes * height)), width, height, (int)rowBytes, bpp, texture, 0, 0, 0, 0, 0);
				}
			}

			Methods.ulBitmapUnlockPixels(bitmap);
		}
		[SkipLocalsInit]
		private unsafe void CreateTexture(uint texture_id, void* bitmapPTRV)
		{
#if DEBUG
			Console.WriteLine($"CreateTexture({texture_id})");
#endif
			IntPtr bitmapPTR = (IntPtr)bitmapPTRV;
			bool isRT = Methods.ulBitmapIsEmpty(bitmapPTR);
			TextureEntry entry = TextureEntries[texture_id];

			uint width = Methods.ulBitmapGetWidth(bitmapPTR);
			uint height = Methods.ulBitmapGetHeight(bitmapPTR);
			uint bpp = Methods.ulBitmapGetBpp(bitmapPTR);

			TextureDescription textureDescription = new()
			{
				Type = TextureType.Texture2D,
				Usage = TextureUsage.Sampled,
				Width = width,
				Height = height,
				MipLevels = isRT ? 1 : MipLevels,
				SampleCount = isRT ? SampleCount : TextureSampleCount.Count1,
				ArrayLayers = 1,
				Depth = 1
			};

			textureDescription.Format = bpp is 1 ? PixelFormat.R8_UNorm : PixelFormat.B8_G8_R8_A8_UNorm;

			if (isRT)
			{
				textureDescription.Usage |= TextureUsage.RenderTarget;
			}
			if (GenerateMipMaps) textureDescription.Usage |= TextureUsage.GenerateMipmaps;

			entry.texture = graphicsDevice.ResourceFactory.CreateTexture(textureDescription);

			if (!isRT)
			{
				UploadTexture(entry.texture, bitmapPTR, width, height, bpp);
			}

			entry.resourceSet = graphicsDevice.ResourceFactory.CreateResourceSet(
				new ResourceSetDescription(
					textureResourceLayout,
					entry.texture
				)
			);
		}
		[SkipLocalsInit]
		private void UpdateTexture(uint texture_id, void* bitmapPTRV)
		{
			IntPtr bitmapPTR = (IntPtr)bitmapPTRV;
			TextureEntry entry = TextureEntries[texture_id];

			uint height = Methods.ulBitmapGetHeight(bitmapPTR);
			uint width = Methods.ulBitmapGetWidth(bitmapPTR);
			uint bpp = Methods.ulBitmapGetBpp(bitmapPTR);

			UploadTexture(entry.texture, bitmapPTR, width, height, bpp);

			if (GenerateMipMaps)
			{
				var cl = graphicsDevice.ResourceFactory.CreateCommandList();
				cl.Begin();
				cl.GenerateMipmaps(entry.texture);
				cl.End();
				graphicsDevice.SubmitCommands(cl);
				cl.Dispose();
			}
		}
		[SkipLocalsInit]
		private void DestroyTexture(uint texture_id)
		{
#if DEBUG
			Console.WriteLine($"DestroyTexture({texture_id})");
#endif
			TextureEntries.Remove(texture_id, out TextureEntry entry);
			entry.resourceSet.Dispose();
			entry.resourceSet = null;
			entry.texture.Dispose();
			entry.texture = null;
		}
		#endregion Texture
		#region Geometry
		[SkipLocalsInit]
		private unsafe void CreateGeometry(uint geometry_id, ULVertexBuffer vertices, ULIndexBuffer indices)
		{
#if DEBUG
			Console.WriteLine($"CreateGeometry({geometry_id})");
#endif
			GeometryEntry entry = GeometryEntries[geometry_id];

			BufferDescription vertexDescription = new(vertices.size, BufferUsage.VertexBuffer);
			entry.vertices = graphicsDevice.ResourceFactory.CreateBuffer(ref vertexDescription);
			BufferDescription indexDescription = new(indices.size, BufferUsage.IndexBuffer);
			entry.indicies = graphicsDevice.ResourceFactory.CreateBuffer(ref indexDescription);

			graphicsDevice.UpdateBuffer(entry.vertices, 0, (IntPtr)vertices.data, vertices.size);
			graphicsDevice.UpdateBuffer(entry.indicies, 0, (IntPtr)indices.data, indices.size);
		}
		[SkipLocalsInit]
		private unsafe void UpdateGeometry(uint geometry_id, ULVertexBuffer vertices, ULIndexBuffer indices)
		{
			//Console.WriteLine($"UpdateGeometry({geometry_id})");
			GeometryEntry entry = GeometryEntries[geometry_id];

			graphicsDevice.UpdateBuffer(entry.vertices, 0, (IntPtr)vertices.data, vertices.size);
			graphicsDevice.UpdateBuffer(entry.indicies, 0, (IntPtr)indices.data, indices.size);
		}
		[SkipLocalsInit]
		private void DestroyGeometry(uint geometry_id)
		{
#if DEBUG
			Console.WriteLine($"DestroyGeometry({geometry_id})");
#endif
			GeometryEntries.Remove(geometry_id, out GeometryEntry entry);

			entry.vertices.Dispose();
			entry.indicies.Dispose();
		}
		#endregion
		#region RenderBuffer
		[SkipLocalsInit]
		private void CreateRenderBuffer(uint render_buffer_id, ULRenderBuffer buffer)
		{
#if DEBUG
			Console.WriteLine($"CreateRenderBuffer({render_buffer_id})");
#endif
			RenderBufferEntry entry = RenderBufferEntries[render_buffer_id];
			TextureEntry textureEntry = TextureEntries[buffer.texture_id];

			entry.textureEntry = textureEntry;

			FramebufferDescription fd = new()
			{
				ColorTargets = new[] {
					new FramebufferAttachmentDescription(textureEntry.texture, 0)
				}
			};

			entry.framebuffer = graphicsDevice.ResourceFactory.CreateFramebuffer(ref fd);
		}
		[SkipLocalsInit]
		private void DestroyRenderBuffer(uint render_buffer_id)
		{
#if DEBUG
			Console.WriteLine($"DestroyRenderBuffer({render_buffer_id})");
#endif
			RenderBufferEntries.Remove(render_buffer_id, out RenderBufferEntry entry);
			entry.textureEntry = null;
			entry.framebuffer.Dispose();
			entry.framebuffer = null;
		}
		#endregion RenderBuffer

		[SkipLocalsInit]
		private void UpdateCommandList(ULCommandList list)
		{
			foreach (ULCommand command in list.ToSpan())
			{
				RenderBufferEntry renderBufferEntry = RenderBufferEntries[command.gpu_state.render_buffer_id];

				CommandList.SetFramebuffer(renderBufferEntry.framebuffer);

				if (command.command_type is ULCommandType.ClearRenderBuffer)
				{
					CommandList.SetFullScissorRect(0);
					CommandList.ClearColorTarget(0, RgbaFloat.Clear);
				}
				else
				{
					ULGPUState state = command.gpu_state;

					if (state.shader_type is ULShaderType.Fill)
					{
						if (state.enable_scissor)
						{
							if (state.enable_blend)
								CommandList.SetPipeline(ul_scissor_blend);
							else
								CommandList.SetPipeline(ul_scissor);
							CommandList.SetScissorRect(0, (uint)state.scissor_rect.left, (uint)state.scissor_rect.top, (uint)(state.scissor_rect.right - state.scissor_rect.left), (uint)(state.scissor_rect.bottom - state.scissor_rect.top));
						}
						else
						{
							if (state.enable_blend)
								CommandList.SetPipeline(ul_blend);
							else
								CommandList.SetPipeline(ul);
						}
						CommandList.SetGraphicsResourceSet(1, TextureEntries[state.texture_1_id].resourceSet);
						CommandList.SetGraphicsResourceSet(2, TextureEntries[state.texture_2_id].resourceSet);
					}
					else
					{
						if (state.enable_scissor)
						{
							if (state.enable_blend)
								CommandList.SetPipeline(ulPath_scissor_blend);
							else
								CommandList.SetPipeline(ulPath_scissor);
							CommandList.SetScissorRect(0, (uint)state.scissor_rect.left, (uint)state.scissor_rect.top, (uint)(state.scissor_rect.right - state.scissor_rect.left), (uint)(state.scissor_rect.bottom - state.scissor_rect.top));
						}
						else
						{
							if (state.enable_blend)
								CommandList.SetPipeline(ulPath_blend);
							else
								CommandList.SetPipeline(ulPath);
						}
					}

					#region Uniforms
					Uniforms uniforms = default;
					uniforms.State.X = state.viewport_width;
					uniforms.State.Y = state.viewport_height;
					uniforms.Transform = state.transform.ApplyProjection(state.viewport_width, state.viewport_height, true);
					new ReadOnlySpan<Vector4>(&state.scalar_0, 2).CopyTo(new Span<Vector4>(&uniforms.Scalar4_0.W, 2));
					new ReadOnlySpan<Vector4>(&state.vector_0.W, 8).CopyTo(new Span<Vector4>(&uniforms.Vector_0.W, 8));
					new ReadOnlySpan<Matrix4x4>(&state.clip_0.M11, 8).CopyTo(new Span<Matrix4x4>(&uniforms.Clip_0.M11, 8));
					uniforms.ClipSize = (uint)state.clip_size;
					#endregion Uniforms
					CommandList.UpdateBuffer(uniformBuffer, 0, ref uniforms);


					//commandList.SetFramebuffer(renderBufferEntry.framebuffer);

					CommandList.SetGraphicsResourceSet(0, uniformResourceSet);

					CommandList.SetViewport(0, new Viewport(0f, 0f, state.viewport_width, state.viewport_height, 0f, 1f));

					GeometryEntry geometryEntry = GeometryEntries[command.geometry_id];

					CommandList.SetVertexBuffer(0, geometryEntry.vertices);
					CommandList.SetIndexBuffer(geometryEntry.indicies, IndexFormat.UInt32);

					CommandList.DrawIndexed(
						command.indices_count,
						1,
						command.indices_offset,
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

		public class TextureEntry
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

		// todo: https://github.com/ultralight-ux/AppCore/blob/6324e85f31f815b1519b495f559f1f72717b2651/src/linux/gl/GPUDriverGL.cpp#L407

		private Texture pipelineOutputTexture;
		private Framebuffer pipelineOutputFramebuffer;

		private readonly Texture emptyTexture;
		private readonly ResourceSet emptyResourceSet;

		private Pipeline ul_scissor_blend;
		private Pipeline ul_blend;
		private Pipeline ul_scissor;
		private Pipeline ul;

		private Pipeline ulPath_scissor_blend;
		private Pipeline ulPath_blend;
		private Pipeline ulPath_scissor;
		private Pipeline ulPath;

		private DeviceBuffer uniformBuffer;

		private void InitializeBuffers()
		{
			uniformBuffer = graphicsDevice.ResourceFactory.CreateBuffer(new(768, BufferUsage.UniformBuffer));
		}

		private Shader[] ultralightShaders;
		private Shader[] ultralightPathShaders;

		private ResourceSet uniformResourceSet;

		private void InitShaders()
		{
			var a = typeof(VeldridGPUDriver).Assembly;
			var fillVert = a.GetManifestResourceStream("UltralightNet.Veldrid.shader_fill.vert.spv");
			byte[] fillVertBytes = new byte[fillVert.Length];
			fillVert.Read(fillVertBytes, 0, (int)fillVert.Length);
			var fillFrag = a.GetManifestResourceStream("UltralightNet.Veldrid.shader_fill.frag.spv");
			byte[] fillFragBytes = new byte[fillFrag.Length];
			fillFrag.Read(fillFragBytes, 0, (int)fillFrag.Length);

			ultralightShaders = graphicsDevice.ResourceFactory.CreateFromSpirv(new(ShaderStages.Vertex, fillVertBytes, "main"), new ShaderDescription(ShaderStages.Fragment, fillFragBytes, "main"));

			var pathVert = a.GetManifestResourceStream("UltralightNet.Veldrid.shader_fill_path.vert.spv");
			byte[] pathVertBytes = new byte[pathVert.Length];
			pathVert.Read(pathVertBytes, 0, (int)pathVert.Length);
			var pathFrag = a.GetManifestResourceStream("UltralightNet.Veldrid.shader_fill_path.frag.spv");
			byte[] pathFragBytes = new byte[pathFrag.Length];
			pathFrag.Read(pathFragBytes, 0, (int)pathFrag.Length);

			ultralightPathShaders = graphicsDevice.ResourceFactory.CreateFromSpirv(new(ShaderStages.Vertex, pathVertBytes, "main"), new ShaderDescription(ShaderStages.Fragment, pathFragBytes, "main"));
		}

		private VertexElementSemantic HLSL_to_any(VertexElementSemantic hlsl_semantic) => IsDirectX ? hlsl_semantic : VertexElementSemantic.TextureCoordinate;

		private ShaderSetDescription FillShaderSetDescription() =>
			new(
				new VertexLayoutDescription[] {
					new VertexLayoutDescription(
						140,
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
						),
						new VertexElementDescription(
							"in_ObjCoord",
							VertexElementSemantic.TextureCoordinate,
							VertexElementFormat.Float2
						),
						new VertexElementDescription(
							"in_Data0",
							HLSL_to_any(VertexElementSemantic.Color),
							VertexElementFormat.Float4
						),
						new VertexElementDescription(
							"in_Data1",
							HLSL_to_any(VertexElementSemantic.Color),
							VertexElementFormat.Float4
						),
						new VertexElementDescription(
							"in_Data2",
							HLSL_to_any(VertexElementSemantic.Color),
							VertexElementFormat.Float4
						),
						new VertexElementDescription(
							"in_Data3",
							HLSL_to_any(VertexElementSemantic.Color),
							VertexElementFormat.Float4
						),
						new VertexElementDescription(
							"in_Data4",
							HLSL_to_any(VertexElementSemantic.Color),
							VertexElementFormat.Float4
						),
						new VertexElementDescription(
							"in_Data5",
							HLSL_to_any(VertexElementSemantic.Color),
							VertexElementFormat.Float4
						),
						new VertexElementDescription(
							"in_Data6",
							HLSL_to_any(VertexElementSemantic.Color),
							VertexElementFormat.Float4
						)
					)
				}, ultralightShaders
			);
		private ShaderSetDescription FillPathShaderSetDescription() => new(
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
			);

		private void InitFramebuffers()
		{
			pipelineOutputTexture = graphicsDevice.ResourceFactory.CreateTexture(new(
				512,
				512,
				1,
				1,
				1,
				PixelFormat.B8_G8_R8_A8_UNorm,
				TextureUsage.RenderTarget,
				TextureType.Texture2D,
				SampleCount));

			pipelineOutputFramebuffer = graphicsDevice.ResourceFactory.CreateFramebuffer(
				new FramebufferDescription()
				{
					ColorTargets = new[] {
						new FramebufferAttachmentDescription(pipelineOutputTexture, 0)
					}
				}
			);
		}
		private static void EnableScissors(ref GraphicsPipelineDescription pipa)
		{
			pipa.RasterizerState.ScissorTestEnabled = true;
		}
		private static void DisableScissors(ref GraphicsPipelineDescription pipa)
		{
			pipa.RasterizerState.ScissorTestEnabled = false;
		}
		private static void DisableBlend(ref GraphicsPipelineDescription pipa)
		{
			pipa.BlendState.AttachmentStates = new[]
			{
				new BlendAttachmentDescription()
				{
					SourceColorFactor = BlendFactor.One,
					SourceAlphaFactor = BlendFactor.One,
					DestinationColorFactor = BlendFactor.InverseSourceAlpha,
					DestinationAlphaFactor = BlendFactor.InverseSourceAlpha,

					BlendEnabled = false
				}
			};
		}
		private void InitPipelines()
		{
			ShaderSetDescription fillShaderSetDescription = FillShaderSetDescription();

			ResourceLayout uniformsResourceLayout = graphicsDevice.ResourceFactory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription(
						"Uniforms",
						ResourceKind.UniformBuffer,
						ShaderStages.Vertex | ShaderStages.Fragment
					)
				)
			);
			uniformResourceSet = graphicsDevice.ResourceFactory.CreateResourceSet(new(uniformsResourceLayout, uniformBuffer));

			GraphicsPipelineDescription _ultralightPipelineDescription = new()
			{
				BlendState = new()
				{
					AttachmentStates = new[]
					{
						// glBlendFunc(GL_ONE, GL_ONE_MINUS_SRC_ALPHA);
						new BlendAttachmentDescription()
						{
							/*SourceColorFactor = BlendFactor.One,
							SourceAlphaFactor = BlendFactor.InverseDestinationAlpha,
							DestinationColorFactor = BlendFactor.InverseSourceAlpha,
							DestinationAlphaFactor = BlendFactor.One,*/
							SourceColorFactor = BlendFactor.One,
							SourceAlphaFactor = BlendFactor.One,
							DestinationColorFactor = BlendFactor.InverseSourceAlpha,
							DestinationAlphaFactor = BlendFactor.InverseSourceAlpha,
							BlendEnabled = true,
							ColorFunction = BlendFunction.Add,
							AlphaFunction = BlendFunction.Add
						}
					},
					AlphaToCoverageEnabled = false
				},
				DepthStencilState = new DepthStencilStateDescription()
				{
					DepthTestEnabled = false, // glDisable(GL_DEPTH_TEST)
					DepthWriteEnabled = false,
					StencilTestEnabled = false,
					DepthComparison = ComparisonKind.Never // glDepthFunc(GL_NEVER)
				},
				RasterizerState = new()
				{
					CullMode = FaceCullMode.None,
					FrontFace = FrontFace.Clockwise,
					FillMode = PolygonFillMode.Solid,
					ScissorTestEnabled = false,
					DepthClipEnabled = false // true
				},
				PrimitiveTopology = PrimitiveTopology.TriangleList,
				ResourceBindingModel = ResourceBindingModel.Default,
				ShaderSet = fillShaderSetDescription,
				ResourceLayouts = new[]
				{
					uniformsResourceLayout,
					textureResourceLayout,
					textureResourceLayout,
					// textureResourceLayout // unused
				},
				Outputs = pipelineOutputFramebuffer.OutputDescription
			};

			GraphicsPipelineDescription ultralight_pd__SCISSOR_TRUE__ENALBE_BLEND = _ultralightPipelineDescription;

			EnableScissors(ref ultralight_pd__SCISSOR_TRUE__ENALBE_BLEND);

			GraphicsPipelineDescription ultralight_pd__SCISSOR_FALSE__ENALBE_BLEND = _ultralightPipelineDescription;

			DisableScissors(ref ultralight_pd__SCISSOR_FALSE__ENALBE_BLEND);

			GraphicsPipelineDescription ultralight_pd__SCISSOR_TRUE__DISALBE_BLEND = _ultralightPipelineDescription;

			EnableScissors(ref ultralight_pd__SCISSOR_TRUE__DISALBE_BLEND);
			DisableBlend(ref ultralight_pd__SCISSOR_TRUE__DISALBE_BLEND);

			GraphicsPipelineDescription ultralight_pd__SCISSOR_FALSE__DISALBE_BLEND = _ultralightPipelineDescription;

			DisableScissors(ref ultralight_pd__SCISSOR_FALSE__DISALBE_BLEND);
			DisableBlend(ref ultralight_pd__SCISSOR_FALSE__DISALBE_BLEND);


			ul_scissor_blend = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralight_pd__SCISSOR_TRUE__ENALBE_BLEND);
			ul_blend = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralight_pd__SCISSOR_FALSE__ENALBE_BLEND);
			ul_scissor = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralight_pd__SCISSOR_TRUE__DISALBE_BLEND);
			ul = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralight_pd__SCISSOR_FALSE__DISALBE_BLEND);


			GraphicsPipelineDescription _ultralightPathPipelineDescription = _ultralightPipelineDescription;
			_ultralightPathPipelineDescription.ShaderSet = FillPathShaderSetDescription();
			_ultralightPathPipelineDescription.ResourceLayouts = new[] { uniformsResourceLayout };

			GraphicsPipelineDescription ultralightPath_pd__SCISSOR_TRUE__ENALBE_BLEND = _ultralightPathPipelineDescription;

			EnableScissors(ref ultralightPath_pd__SCISSOR_TRUE__ENALBE_BLEND);

			GraphicsPipelineDescription ultralightPath_pd__SCISSOR_FALSE__ENALBE_BLEND = _ultralightPathPipelineDescription;

			DisableScissors(ref ultralightPath_pd__SCISSOR_FALSE__ENALBE_BLEND);

			GraphicsPipelineDescription ultralightPath_pd__SCISSOR_TRUE__DISABLE_BLEND = _ultralightPathPipelineDescription;

			EnableScissors(ref ultralightPath_pd__SCISSOR_TRUE__DISABLE_BLEND);
			DisableBlend(ref ultralightPath_pd__SCISSOR_TRUE__DISABLE_BLEND);

			GraphicsPipelineDescription ultralightPath_pd__SCISSOR_FALSE__DISABLE_BLEND = _ultralightPathPipelineDescription;

			DisableScissors(ref ultralightPath_pd__SCISSOR_FALSE__DISABLE_BLEND);
			DisableBlend(ref ultralightPath_pd__SCISSOR_FALSE__DISABLE_BLEND);

			ulPath_scissor_blend = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralightPath_pd__SCISSOR_TRUE__ENALBE_BLEND);
			ulPath_blend = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralightPath_pd__SCISSOR_FALSE__ENALBE_BLEND);
			ulPath_scissor = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralightPath_pd__SCISSOR_TRUE__DISABLE_BLEND);
			ulPath = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralightPath_pd__SCISSOR_FALSE__DISABLE_BLEND);
		}
	}
}
