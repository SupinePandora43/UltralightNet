using System;
using System.Collections.Generic;
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

		private Queue<ULCommand> commands = new();

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

			IsDirectX = graphicsDevice.BackendType is GraphicsBackend.Direct3D11;

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
					PixelFormat.R8_G8_B8_A8_UNorm_SRgb,
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

		private uint GetKey<TValue>(Dictionary<uint, TValue> dictionary)
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

			if (isRT)
			{
				textureDescription.Format = PixelFormat.R8_G8_B8_A8_UNorm_SRgb;
				textureDescription.Usage |= TextureUsage.RenderTarget;
			}
			else
			{
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
				if (GenerateMipMaps) textureDescription.Usage |= TextureUsage.GenerateMipmaps;
			}

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
			TextureEntries.Remove(texture_id, out TextureEntry entry);
			entry.texture.Dispose();
			entry.resourceSet.Dispose();
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
			entry.textureEntry = null;
			entry.framebuffer.Dispose();
		}
		#endregion RenderBuffer

		private void UpdateCommandList(ULCommandList list)
		{
			foreach (ULCommand command in list.ToSpan())
				commands.Enqueue(command);
		}

		public void Render()
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

					if (state.shader_type is ULShaderType.FillPath) continue; // for now

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

					#region Uniforms
					// State
					commandList.UpdateBuffer(uniformBuffer, 0, new float[] { 1, state.viewport_width, state.viewport_height, 1 });
					// Transform
					commandList.UpdateBuffer(uniformBuffer, 4 * 4, state.transform.ulApplyProjection(512, 512, true));
					commandList.UpdateBuffer(uniformBuffer, (4 * 4) + (4 * 16), state.pixelData);
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
			/*ShaderDescription ulVertDesc = new()
			{
				Stage = ShaderStages.Vertex,
				EntryPoint = "main",
				Debug = Debug
			};
			ShaderDescription ulFragDesc = new()
			{
				Stage = ShaderStages.Fragment,
				EntryPoint = "main",
				Debug = Debug
			};

			if (graphicsDevice.BackendType is GraphicsBackend.Direct3D11)
			{
				ulVertDesc.ShaderBytes = Shaders.HLSL.ulVert;
				ulFragDesc.ShaderBytes = Shaders.HLSL.ulFill;
			}

			ultralightShaders = new[] {
				graphicsDevice.ResourceFactory.CreateShader(ref ulVertDesc),
				graphicsDevice.ResourceFactory.CreateShader(ref ulFragDesc)
			};

			ShaderDescription ulPathVertDesc = new()
			{
				Stage = ShaderStages.Vertex,
				EntryPoint = "main",
				Debug = Debug
			};
			ShaderDescription ulPathFragDesc = new()
			{
				Stage = ShaderStages.Fragment,
				EntryPoint = "main",
				Debug = Debug
			};

			if (graphicsDevice.BackendType is GraphicsBackend.Direct3D11)
			{
				ulPathVertDesc.ShaderBytes = Shaders.HLSL.ulPathVert;
				ulPathFragDesc.ShaderBytes = Shaders.HLSL.ulPathFill;
			}

			ultralightPathShaders = new[] {
				graphicsDevice.ResourceFactory.CreateShader(ref ulPathVertDesc),
				graphicsDevice.ResourceFactory.CreateShader(ref ulPathFragDesc)
			};*/

			ultralightShaders = SpirvCross.ul(graphicsDevice);
		}

		private ShaderSetDescription FillShaderSetDescription() =>
			new(
				new VertexLayoutDescription[] {
					new VertexLayoutDescription(
						140,
						new VertexElementDescription(
							"in_Position",
							VertexElementSemantic.TextureCoordinate,
							VertexElementFormat.Float2
						),
						new VertexElementDescription(
							"in_Color",
							VertexElementSemantic.TextureCoordinate,
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
							VertexElementSemantic.TextureCoordinate,
							VertexElementFormat.Float4
						),
						new VertexElementDescription(
							"in_Data1",
							VertexElementSemantic.TextureCoordinate,
							VertexElementFormat.Float4
						),
						new VertexElementDescription(
							"in_Data2",
							VertexElementSemantic.TextureCoordinate,
							VertexElementFormat.Float4
						),
						new VertexElementDescription(
							"in_Data3",
							VertexElementSemantic.TextureCoordinate,
							VertexElementFormat.Float4
						),
						new VertexElementDescription(
							"in_Data4",
							VertexElementSemantic.TextureCoordinate,
							VertexElementFormat.Float4
						),
						new VertexElementDescription(
							"in_Data5",
							VertexElementSemantic.TextureCoordinate,
							VertexElementFormat.Float4
						),
						new VertexElementDescription(
							"in_Data6",
							VertexElementSemantic.TextureCoordinate,
							VertexElementFormat.Float4
						)
					)
				}, ultralightShaders
			);
		private void InitFramebuffers()
		{
			pipelineOutputTexture = graphicsDevice.ResourceFactory.CreateTexture(new(
				512,
				512,
				1,
				1,
				1,
				PixelFormat.R8_G8_B8_A8_UNorm_SRgb,
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

			// todo: glBlendFunc(GL_ONE, GL_ONE_MINUS_SRC_ALPHA);
			GraphicsPipelineDescription _ultralightPipelineDescription = new()
			{
				BlendState = new()
				{
					AttachmentStates = new[]
					{
						// glBlendFunc(GL_ONE, GL_ONE_MINUS_SRC_ALPHA);
						new BlendAttachmentDescription()
						{
							SourceColorFactor = BlendFactor.One,
							SourceAlphaFactor = BlendFactor.One,
							DestinationColorFactor = BlendFactor.InverseSourceAlpha,
							DestinationAlphaFactor = BlendFactor.InverseSourceAlpha
						}
					}
				},
				DepthStencilState = new DepthStencilStateDescription()
				{
					DepthTestEnabled = false, // glDisable(GL_DEPTH_TEST)
					StencilTestEnabled = false,
					DepthComparison = ComparisonKind.Never // glDepthFunc(GL_NEVER)
				},
				RasterizerState = new()
				{
					CullMode = FaceCullMode.None,
					//FrontFace = FrontFace.Clockwise,
					//FillMode = PolygonFillMode.Solid,
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
			ultralight_pd__SCISSOR_TRUE__DISALBE_BLEND.BlendState = BlendStateDescription.SingleDisabled;

			GraphicsPipelineDescription ultralight_pd__SCISSOR_FALSE__DISALBE_BLEND = _ultralightPipelineDescription;

			ultralight_pd__SCISSOR_FALSE__DISALBE_BLEND.RasterizerState.ScissorTestEnabled = false;
			ultralight_pd__SCISSOR_FALSE__DISALBE_BLEND.BlendState = BlendStateDescription.SingleDisabled;


			ul_scissor_blend = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralight_pd__SCISSOR_TRUE__ENALBE_BLEND);
			ul_blend = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralight_pd__SCISSOR_FALSE__ENALBE_BLEND);
			ul_scissor = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralight_pd__SCISSOR_TRUE__DISALBE_BLEND);
			ul = graphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref ultralight_pd__SCISSOR_FALSE__DISALBE_BLEND);
		}
	}
}
