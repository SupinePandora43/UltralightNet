using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using UltralightNet.Veldrid.Shaders;
using Veldrid;

namespace UltralightNet.Veldrid
{
	public class VeldridGPUDriver
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
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		private static uint GetKey<TValue>(Dictionary<uint, TValue> dictionary)
		{
			for (uint i = 1; ; i++)
			{
				if (!dictionary.ContainsKey(i))
					return i;
			}
		}
		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		private uint NextTextureId()
		{
			uint id = GetKey(TextureEntries);
			TextureEntries.Add(id, new());
#if DEBUG
			Console.WriteLine($"NextTextureId() = {id}");
#endif
			return id;
		}
		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		private uint NextGeometryId()
		{
			uint id = GetKey(GeometryEntries);
			GeometryEntries.Add(id, new());
#if DEBUG
			Console.WriteLine($"NextGeometryId() = {id}");
#endif
			return id;
		}
		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
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
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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
		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
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
		private void CreateTexture(uint texture_id, IntPtr bitmapPTR)
		{
#if DEBUG
			Console.WriteLine($"CreateTexture({texture_id})");
#endif
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
		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		private void UpdateTexture(uint texture_id, IntPtr bitmapPTR)
		{
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
		private void CreateGeometry(uint geometry_id, ULVertexBuffer vertices, ULIndexBuffer indices)
		{
#if DEBUG
			Console.WriteLine($"CreateGeometry({geometry_id})");
#endif
			GeometryEntry entry = GeometryEntries[geometry_id];

			BufferDescription vertexDescription = new(vertices.size, BufferUsage.VertexBuffer);
			entry.vertices = graphicsDevice.ResourceFactory.CreateBuffer(ref vertexDescription);
			BufferDescription indexDescription = new(indices.size, BufferUsage.IndexBuffer);
			entry.indicies = graphicsDevice.ResourceFactory.CreateBuffer(ref indexDescription);

			graphicsDevice.UpdateBuffer(entry.vertices, 0, vertices.data, vertices.size);
			graphicsDevice.UpdateBuffer(entry.indicies, 0, indices.data, indices.size);
		}
		private void UpdateGeometry(uint geometry_id, ULVertexBuffer vertices, ULIndexBuffer indices)
		{
			//Console.WriteLine($"UpdateGeometry({geometry_id})");
			GeometryEntry entry = GeometryEntries[geometry_id];

			graphicsDevice.UpdateBuffer(entry.vertices, 0, vertices.data, vertices.size);
			graphicsDevice.UpdateBuffer(entry.indicies, 0, indices.data, indices.size);
		}
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

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
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
					Uniforms uniforms = new()
					{
						State = new Vector4(time, state.viewport_width, state.viewport_height, 1f),
						Transform = state.transform.ApplyProjection(state.viewport_width, state.viewport_height, IsOpenGL),

						scalar_0 = state.scalar_0,
						scalar_1 = state.scalar_1,
						scalar_2 = state.scalar_2,
						scalar_3 = state.scalar_3,
						scalar_4 = state.scalar_4,
						scalar_5 = state.scalar_5,
						scalar_6 = state.scalar_6,
						scalar_7 = state.scalar_7,

						vector_0 = state.vector_0,
						vector_1 = state.vector_1,
						vector_2 = state.vector_2,
						vector_3 = state.vector_3,
						vector_4 = state.vector_4,
						vector_5 = state.vector_5,
						vector_6 = state.vector_6,
						vector_7 = state.vector_7,

						clip_size = state.clip_size,

						clip_0 = state.clip_0,
						clip_1 = state.clip_1,
						clip_2 = state.clip_2,
						clip_3 = state.clip_3,
						clip_4 = state.clip_4,
						clip_5 = state.clip_5,
						clip_6 = state.clip_6,
						clip_7 = state.clip_7
					};
					CommandList.UpdateBuffer(uniformBuffer, 0, ref uniforms);
					#endregion


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
		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public ResourceSet GetRenderTarget(View view) => TextureEntries[view.RenderTarget.texture_id].resourceSet;

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
			if (IsDirectX)
			{
				ShaderDescription ulVertDesc = new()
				{
					Stage = ShaderStages.Vertex,
					EntryPoint = "main",
					Debug = Debug,
					ShaderBytes = HLSL.ulVert
				};
				ShaderDescription ulFragDesc = new()
				{
					Stage = ShaderStages.Fragment,
					EntryPoint = "main",
					Debug = Debug,
					ShaderBytes = HLSL.ulFill
				};

				ultralightShaders = new[] {
					graphicsDevice.ResourceFactory.CreateShader(ref ulVertDesc),
					graphicsDevice.ResourceFactory.CreateShader(ref ulFragDesc)
				};

				ShaderDescription ulPathVertDesc = new()
				{
					Stage = ShaderStages.Vertex,
					EntryPoint = "main",
					Debug = Debug,
					ShaderBytes = HLSL.ulPathVert
				};
				ShaderDescription ulPathFragDesc = new()
				{
					Stage = ShaderStages.Fragment,
					EntryPoint = "main",
					Debug = Debug,
					ShaderBytes = HLSL.ulPathFill,
				};

				ultralightPathShaders = new[] {
					graphicsDevice.ResourceFactory.CreateShader(ref ulPathVertDesc),
					graphicsDevice.ResourceFactory.CreateShader(ref ulPathFragDesc)
				};
			}
			else
			{
				ultralightShaders = SpirvCross.ul(graphicsDevice);
				ultralightPathShaders = SpirvCross.ulPath(graphicsDevice);
			}
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
		private static void DisableBlend(ref GraphicsPipelineDescription pipa)
		{
			pipa.BlendState.AttachmentStates = new[]
			{
				new BlendAttachmentDescription()
				{
					SourceColorFactor = BlendFactor.One,
					SourceAlphaFactor = BlendFactor.One,
					DestinationColorFactor = BlendFactor.Zero,
					DestinationAlphaFactor = BlendFactor.Zero,

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
				//BlendState = BlendStateDescription.SingleAlphaBlend,
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

			ultralight_pd__SCISSOR_TRUE__ENALBE_BLEND.RasterizerState.ScissorTestEnabled = true;

			GraphicsPipelineDescription ultralight_pd__SCISSOR_FALSE__ENALBE_BLEND = _ultralightPipelineDescription;

			ultralight_pd__SCISSOR_FALSE__ENALBE_BLEND.RasterizerState.ScissorTestEnabled = false;

			GraphicsPipelineDescription ultralight_pd__SCISSOR_TRUE__DISALBE_BLEND = _ultralightPipelineDescription;

			ultralight_pd__SCISSOR_TRUE__DISALBE_BLEND.RasterizerState.ScissorTestEnabled = true;
			DisableBlend(ref ultralight_pd__SCISSOR_TRUE__DISALBE_BLEND);

			GraphicsPipelineDescription ultralight_pd__SCISSOR_FALSE__DISALBE_BLEND = _ultralightPipelineDescription;

			ultralight_pd__SCISSOR_FALSE__DISALBE_BLEND.RasterizerState.ScissorTestEnabled = false;
			DisableBlend(ref ultralight_pd__SCISSOR_FALSE__DISALBE_BLEND);


			ul_scissor_blend = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralight_pd__SCISSOR_TRUE__ENALBE_BLEND);
			ul_blend = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralight_pd__SCISSOR_FALSE__ENALBE_BLEND);
			ul_scissor = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralight_pd__SCISSOR_TRUE__DISALBE_BLEND);
			ul = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralight_pd__SCISSOR_FALSE__DISALBE_BLEND);


			GraphicsPipelineDescription _ultralightPathPipelineDescription = _ultralightPipelineDescription;
			_ultralightPathPipelineDescription.ShaderSet = FillPathShaderSetDescription();
			_ultralightPathPipelineDescription.ResourceLayouts = new[] { uniformsResourceLayout };

			GraphicsPipelineDescription ultralightPath_pd__SCISSOR_TRUE__ENALBE_BLEND = _ultralightPathPipelineDescription;

			ultralightPath_pd__SCISSOR_TRUE__ENALBE_BLEND.RasterizerState.ScissorTestEnabled = true;

			GraphicsPipelineDescription ultralightPath_pd__SCISSOR_FALSE__ENALBE_BLEND = _ultralightPathPipelineDescription;

			ultralightPath_pd__SCISSOR_FALSE__ENALBE_BLEND.RasterizerState.ScissorTestEnabled = false;

			GraphicsPipelineDescription ultralightPath_pd__SCISSOR_TRUE__DISABLE_BLEND = _ultralightPathPipelineDescription;

			DisableBlend(ref ultralightPath_pd__SCISSOR_TRUE__DISABLE_BLEND);

			GraphicsPipelineDescription ultralightPath_pd__SCISSOR_FALSE__DISABLE_BLEND = _ultralightPathPipelineDescription;

			ultralightPath_pd__SCISSOR_FALSE__DISABLE_BLEND.RasterizerState.ScissorTestEnabled = true;
			DisableBlend(ref ultralightPath_pd__SCISSOR_FALSE__DISABLE_BLEND);

			ulPath_scissor_blend = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralightPath_pd__SCISSOR_TRUE__ENALBE_BLEND);
			ulPath_blend = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralightPath_pd__SCISSOR_FALSE__ENALBE_BLEND);
			ulPath_scissor = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralightPath_pd__SCISSOR_TRUE__DISABLE_BLEND);
			ulPath = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralightPath_pd__SCISSOR_FALSE__DISABLE_BLEND);
		}

		private struct Uniforms
		{
			public Vector4 State;
			public Matrix4x4 Transform;

			public float scalar_0;
			public float scalar_1;
			public float scalar_2;
			public float scalar_3;
			public float scalar_4;
			public float scalar_5;
			public float scalar_6;
			public float scalar_7;

			public Vector4 vector_0;
			public Vector4 vector_1;
			public Vector4 vector_2;
			public Vector4 vector_3;
			public Vector4 vector_4;
			public Vector4 vector_5;
			public Vector4 vector_6;
			public Vector4 vector_7;

			public uint clip_size;

			public Matrix4x4 clip_0;
			public Matrix4x4 clip_1;
			public Matrix4x4 clip_2;
			public Matrix4x4 clip_3;
			public Matrix4x4 clip_4;
			public Matrix4x4 clip_5;
			public Matrix4x4 clip_6;
			public Matrix4x4 clip_7;
		}
	}
}
