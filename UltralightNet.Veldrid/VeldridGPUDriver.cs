using System;
using System.Collections.Generic;
using System.Numerics;
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
		public bool WaitForIdle = false;
		public bool Debug = false;

		private readonly Dictionary<uint, TextureEntry> TextureEntries = new();
		private readonly Dictionary<uint, GeometryEntry> GeometryEntries = new();
		private readonly Dictionary<uint, RenderBufferEntry> RenderBufferEntries = new();

		private readonly bool IsDirectX = false;
		private readonly bool IsOpenGL = false;

		private readonly Queue<ULCommand> commands = new();

		private readonly CommandList commandList;

		public VeldridGPUDriver(GraphicsDevice graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;

			textureResourceLayout = graphicsDevice.ResourceFactory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("texture", ResourceKind.TextureReadOnly, ShaderStages.Fragment)
				)
			);

			commandList = graphicsDevice.ResourceFactory.CreateCommandList();

			//IsDirectX = graphicsDevice.BackendType is GraphicsBackend.Direct3D11;

			IsOpenGL = (graphicsDevice.BackendType is GraphicsBackend.OpenGL) || (graphicsDevice.BackendType is GraphicsBackend.OpenGLES);

			InitializeBuffers();
			InitShaders();
			InitFramebuffers();
			InitPipelines();

			emptyTexture = graphicsDevice.ResourceFactory.CreateTexture(
				new(
					1,
					1,
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
		}
		public ULGPUDriver GetGPUDriver() => new()
		{
			BeginSynchronize = nothing,
			EndSynchronize = nothing,

			NextTextureId = NextTextureId,
			NextGeometryId = NextGeometryId,
			NextRenderBufferId = NextRenderBufferId,

			CreateTexture = CreateTexture,
			UpdateTexture = UpdateTexture,
			DestroyTexture = DestroyTexture,

			CreateGeometry = CreateGeometry,
			UpdateGeometry = UpdateGeometry,
			DestroyGeometry = DestroyGeometry,

			CreateRenderBuffer = CreateRenderBuffer,
			DestroyRenderBuffer = DestroyRenderBuffer,

			UpdateCommandList = UpdateCommandList
		};
		private void nothing() { }

		#region NextId

		private static uint GetKey<TValue>(Dictionary<uint, TValue> dictionary)
		{
			for (uint i = 1; ; i++)
			{
				if (!dictionary.ContainsKey(i))
					return i;
			}
		}
		private uint NextTextureId()
		{
			uint id = GetKey(TextureEntries);
			TextureEntries.Add(id, new());
			Console.WriteLine($"NextTextureId() = {id}");
			return id;
		}
		public uint NextGeometryId()
		{
			uint id = GetKey(GeometryEntries);
			GeometryEntries.Add(id, new());
			Console.WriteLine($"NextGeometryId() = {id}");
			return id;
		}
		public uint NextRenderBufferId()
		{
			uint id = GetKey(RenderBufferEntries);
			RenderBufferEntries.Add(id, new());
			Console.WriteLine($"NextRenderBufferId() = {id}");
			return id;
		}
		#endregion NextId
		#region Texture
		private void CreateTexture(uint texture_id, ULBitmap bitmap)
		{
			Console.WriteLine($"CreateTexture({texture_id})");
			bool isRT = bitmap.IsEmpty;
			TextureEntry entry = TextureEntries[texture_id];
			TextureDescription textureDescription = new()
			{
				Type = TextureType.Texture2D,
				Usage = TextureUsage.Sampled,
				Width = bitmap.Width,
				Height = bitmap.Height,
				MipLevels = isRT ? 1 : MipLevels,
				SampleCount = isRT ? TextureSampleCount.Count1 : SampleCount,
				ArrayLayers = 1,
				Depth = 1
			};

			ULBitmapFormat format = bitmap.Format;
			if (format is ULBitmapFormat.A8_UNORM)
			{
				textureDescription.Format = PixelFormat.R8_UNorm;
			}
			else if (format is ULBitmapFormat.BGRA8_UNORM_SRGB)
			{
				textureDescription.Format = PixelFormat.B8_G8_R8_A8_UNorm_SRgb;
			}
			else throw new NotSupportedException("format");

			if (isRT)
			{
				textureDescription.Usage |= TextureUsage.RenderTarget;
			}
			else
			{
			}
			if (GenerateMipMaps) textureDescription.Usage |= TextureUsage.GenerateMipmaps;

			entry.texture = graphicsDevice.ResourceFactory.CreateTexture(textureDescription);

			if (!isRT)
			{
				graphicsDevice.UpdateTexture(entry.texture, bitmap.LockPixels(), (uint)bitmap.Size, 0, 0, 0, textureDescription.Width, textureDescription.Height, 1, 0, 0);
				bitmap.UnlockPixels();

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

			entry.resourceSet = graphicsDevice.ResourceFactory.CreateResourceSet(
				new ResourceSetDescription(
					textureResourceLayout,
					entry.texture
				)
			);

			if (WaitForIdle) graphicsDevice.WaitForIdle();
		}
		private void UpdateTexture(uint texture_id, ULBitmap bitmap)
		{
			Console.WriteLine($"UpdateTexture({texture_id})");
			TextureEntry entry = TextureEntries[texture_id];

			graphicsDevice.UpdateTexture(entry.texture, bitmap.LockPixels(), (uint)bitmap.Size, 0, 0, 0, bitmap.Width, bitmap.Height, 1, 0, 0);
			bitmap.UnlockPixels();

			if (GenerateMipMaps)
			{
				var cl = graphicsDevice.ResourceFactory.CreateCommandList();
				cl.Begin();
				cl.GenerateMipmaps(entry.texture);
				cl.End();
				graphicsDevice.SubmitCommands(cl);
				if (WaitForIdle) graphicsDevice.WaitForIdle();
				cl.Dispose();
			}
		}
		private void DestroyTexture(uint texture_id)
		{
			Console.WriteLine($"DestroyTexture({texture_id})");
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
			Console.WriteLine($"CreateGeometry({geometry_id})");
			GeometryEntry entry = GeometryEntries[geometry_id];

			BufferDescription vertexDescription = new(vertices.size, BufferUsage.VertexBuffer);
			entry.vertices = graphicsDevice.ResourceFactory.CreateBuffer(ref vertexDescription);
			BufferDescription indexDescription = new(indices.size, BufferUsage.IndexBuffer);
			entry.indicies = graphicsDevice.ResourceFactory.CreateBuffer(ref indexDescription);

			graphicsDevice.UpdateBuffer(entry.vertices, 0, vertices.data, vertices.size);
			graphicsDevice.UpdateBuffer(entry.indicies, 0, indices.data, indices.size);

			if (WaitForIdle) graphicsDevice.WaitForIdle();
		}
		private void UpdateGeometry(uint geometry_id, ULVertexBuffer vertices, ULIndexBuffer indices)
		{
			//Console.WriteLine($"UpdateGeometry({geometry_id})");
			GeometryEntry entry = GeometryEntries[geometry_id];

			graphicsDevice.UpdateBuffer(entry.vertices, 0, vertices.data, vertices.size);
			graphicsDevice.UpdateBuffer(entry.indicies, 0, indices.data, indices.size);

			if (WaitForIdle) graphicsDevice.WaitForIdle();
		}
		private void DestroyGeometry(uint geometry_id)
		{
			Console.WriteLine($"DestroyGeometry({geometry_id})");
			GeometryEntries.Remove(geometry_id, out GeometryEntry entry);

			entry.vertices.Dispose();
			entry.indicies.Dispose();
		}
		#endregion
		#region RenderBuffer
		private void CreateRenderBuffer(uint render_buffer_id, ULRenderBuffer buffer)
		{
			Console.WriteLine($"CreateRenderBuffer({render_buffer_id})");
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

			if (WaitForIdle) graphicsDevice.WaitForIdle();
		}
		private void DestroyRenderBuffer(uint render_buffer_id)
		{
			Console.WriteLine($"DestroyRenderBuffer({render_buffer_id})");
			RenderBufferEntries.Remove(render_buffer_id, out RenderBufferEntry entry);
			// todo: should i?
			// entry.textureEntry = null;
			entry.framebuffer.Dispose();
		}
		#endregion RenderBuffer

		private void UpdateCommandList(ULCommandList list)
		{
			foreach (ULCommand command in list.ToSpan())
				commands.Enqueue(command);
		}

		public void Render(float time = 1f)
		{
			if (commands.Count is 0) return;
			commandList.Begin();

			foreach (ULCommand command in commands)
			{
				RenderBufferEntry renderBufferEntry = RenderBufferEntries[command.gpu_state.render_buffer_id];

				commandList.SetFramebuffer(renderBufferEntry.framebuffer);

				if (command.command_type is ULCommandType.ClearRenderBuffer)
				{
					commandList.ClearColorTarget(0, RgbaFloat.Clear);
				}
				else
				{
					ULGPUState state = command.gpu_state;

					if (state.shader_type is ULShaderType.Fill)
					{
						if (state.enable_scissor)
						{
							if (state.enable_blend)
								commandList.SetPipeline(ul_scissor_blend);
							else
								commandList.SetPipeline(ul_scissor);
							commandList.SetScissorRect(0, (uint)state.scissor_rect.left, (uint)state.scissor_rect.top, (uint)(state.scissor_rect.right - state.scissor_rect.left), (uint)(state.scissor_rect.bottom - state.scissor_rect.top));
						}
						else
						{
							if (state.enable_blend)
								commandList.SetPipeline(ul_blend);
							else
								commandList.SetPipeline(ul);
						}
					}
					else
					{
						if (state.enable_scissor)
						{
							if (state.enable_blend)
								commandList.SetPipeline(ulPath_scissor_blend);
							else
								commandList.SetPipeline(ulPath_scissor);
							commandList.SetScissorRect(0, (uint)state.scissor_rect.left, (uint)state.scissor_rect.top, (uint)(state.scissor_rect.right - state.scissor_rect.left), (uint)(state.scissor_rect.bottom - state.scissor_rect.top));
						}
						else
						{
							if (state.enable_blend)
								commandList.SetPipeline(ulPath_blend);
							else
								commandList.SetPipeline(ulPath);
						}
					}

					#region Uniforms
					Uniforms uniforms = new()
					{
						State = new Vector4(time, state.viewport_width, state.viewport_height, 1f),
						Transform = state.transform.ulApplyProjection(state.viewport_width, state.viewport_height, IsOpenGL),

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
					commandList.UpdateBuffer(uniformBuffer, 0, uniforms);
					#endregion


					commandList.SetFramebuffer(renderBufferEntry.framebuffer);

					commandList.SetGraphicsResourceSet(0, uniformResourceSet);

					if (state.texture_1_id != 0)
						commandList.SetGraphicsResourceSet(1, TextureEntries[state.texture_1_id].resourceSet);
					else
						commandList.SetGraphicsResourceSet(1, emptyResourceSet);
					if (state.texture_2_id != 0)
						commandList.SetGraphicsResourceSet(2, TextureEntries[state.texture_2_id].resourceSet);
					else
						commandList.SetGraphicsResourceSet(2, emptyResourceSet);

					GeometryEntry geometryEntry = GeometryEntries[command.geometry_id];

					commandList.SetVertexBuffer(0, geometryEntry.vertices);
					commandList.SetIndexBuffer(geometryEntry.indicies, IndexFormat.UInt32);

					commandList.DrawIndexed(
						command.indices_count,
						1,
						command.indices_offset,
						0,
						0
					);
				}
			}

			commands.Clear();

			commandList.End();
			graphicsDevice.SubmitCommands(commandList);

			if (WaitForIdle) graphicsDevice.WaitForIdle();
		}

		/// <remarks>will throw exception when view doesn't have RenderTarget</remarks>
		/// <exception cref="KeyNotFoundException">When called on view without RenderTarget</exception>
		public ResourceSet GetRenderTarget(View view) => TextureEntries[view.RenderTarget.texture_id].resourceSet;

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

		// todo: https://github.com/ultralight-ux/AppCore/blob/6324e85f31f815b1519b495f559f1f72717b2651/src/linux/gl/GPUDriverGL.cpp#L407

		private Texture pipelineOutputTexture;
		private Framebuffer pipelineOutputFramebuffer;

		private Texture emptyTexture;
		private ResourceSet emptyResourceSet;

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
				1,
				1,
				1,
				1,
				1,
				PixelFormat.B8_G8_R8_A8_UNorm_SRgb,
				TextureUsage.RenderTarget,
				TextureType.Texture2D));

			pipelineOutputFramebuffer = graphicsDevice.ResourceFactory.CreateFramebuffer(
				new FramebufferDescription()
				{
					ColorTargets = new[] {
						new FramebufferAttachmentDescription(pipelineOutputTexture, 0)
					}
				}
			);
		}
		private void DisableBlend(ref GraphicsPipelineDescription pipa)
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
					DepthClipEnabled = true
				},
				//PrimitiveTopology = PrimitiveTopology.TriangleList,
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
