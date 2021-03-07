using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.ImageSharp;
using Veldrid.SPIRV;

namespace VeldridSandbox
{
	public partial class Program
	{
		private GraphicsDevice graphicsDevice;
		private ResourceFactory factory;

		private CommandList commandList;

		private Pipeline mainPipeline;
		private Pipeline ultralightPipeline;
		private Pipeline ultralightPathPipeline;

		private Framebuffer ultralightOutputBuffer;
		private Framebuffer ultralightPathOutputBuffer;

		private Texture ultralightOutputTexture;
		private Texture ultralightPathOutputTexture;

		private ResourceSet mainResourceSet;
		private ResourceSet uniformResourceSet;
		private ResourceSet ultralightResourceSet;
		private ResourceSet ultralightPathResourceSet;

		Sampler TextureSampler;

		Shader mainv;
		Shader mainf;
		Shader vertexShader;
		Shader fragmentShader;
		Shader pathVertexShader;
		Shader pathFragmentShader;

		DeviceBuffer rtVertexBuffer;
		DeviceBuffer quadIndexBuffer;

		DeviceBuffer ultralightVertexBuffer;
		DeviceBuffer ultralightPathVertexBuffer;

		DeviceBuffer ultralightVertexTest;
		ResourceSet flushedTextureViewResourceSet;
		DeviceBuffer ultralightVertexTestIndex;

		Uniforms uniforms = new();
		DeviceBuffer uniformBuffer;

		Texture testTexture;
		TextureView tv;

		private static readonly float[] _quadVerts = {
			/*1f, 1f, 0f,
			1f, -1f, 0f,
			-1f, -1f, 0f,
			-1f, 1f, 1f*/
			-.75f, .75f,
			.75f, .75f,
			-.75f, -.75f,
			.75f, -.75f
		};
		private static readonly /*uint*/short[] _quadIndices = {
			//0, 1, 3,
			//1, 2, 3
			0,1,2,3
		};

		public struct VertexPositionTexture
		{
			public const uint SizeInBytes = 16;

			public float PosX;
			public float PosY;

			public float TexU;
			public float TexV;

			public VertexPositionTexture(Vector2 pos, Vector2 uv)
			{
				PosX = pos.X;
				PosY = pos.Y;
				TexU = uv.X;
				TexV = uv.Y;
			}
		}

		private void CreateShaders()
		{
			CrossCompileOptions crossCompileOptions = new(false, BACKEND == GraphicsBackend.OpenGL || BACKEND == GraphicsBackend.OpenGLES);
			#region main
			ShaderDescription mainvShaderDescription = new(ShaderStages.Vertex, GetShaderBytes("embedded.basic.vert.glsl"), "main");
			ShaderDescription mainfShaderDescription = new(ShaderStages.Fragment, GetShaderBytes("embedded.basic.frag.glsl"), "main");
			Shader[] mainShaders = factory.CreateFromSpirv(mainvShaderDescription, mainfShaderDescription);
			mainv = mainShaders[0];
			mainf = mainShaders[1];
			#endregion
			#region fill
			ShaderDescription vertexShaderShaderDescription = new(ShaderStages.Vertex, GetShaderBytes("embedded.shader_v2f_c4f_t2f_t2f_d28f.vert.glsl"), "main");
			ShaderDescription fragmentShaderShaderDescription = new(ShaderStages.Fragment, GetShaderBytes("embedded.shader_fill.frag.glsl"), "main");
			Shader[] shaders = factory.CreateFromSpirv(vertexShaderShaderDescription, fragmentShaderShaderDescription, crossCompileOptions);
			vertexShader = shaders[0];
			fragmentShader = shaders[1];
			#endregion
			#region fillPath
			ShaderDescription pathVertexShaderShaderDescription = new(ShaderStages.Vertex, GetShaderBytes("embedded.shader_v2f_c4f_t2f.vert.glsl"), "main");
			ShaderDescription pathFragmentShaderShaderDescription = new(ShaderStages.Fragment, GetShaderBytes("embedded.shader_fill_path.frag.glsl"), "main");
			Shader[] pathShaders = factory.CreateFromSpirv(pathVertexShaderShaderDescription, pathFragmentShaderShaderDescription, crossCompileOptions);
			pathVertexShader = pathShaders[0];
			pathFragmentShader = pathShaders[1];
			#endregion
		}

		private void CreateBuffers()
		{
			BufferDescription quadBufferDescription = new(VertexPositionTexture.SizeInBytes * 4, BufferUsage.VertexBuffer);
			#region RenderTarget
			rtVertexBuffer = factory.CreateBuffer(quadBufferDescription);
			graphicsDevice.UpdateBuffer(rtVertexBuffer, 0, new VertexPositionTexture[] {
				new(new(-1, 1), new(0,0)),
				new(new(1, 1),  new(1,0)),
				new(new(-1, -1),new(0,1)),
				new(new(1, -1), new(1,1)),
			});
			quadIndexBuffer = factory.CreateBuffer(new(/*sizeof(uint) * 6*/ 4 * sizeof(short), BufferUsage.IndexBuffer));
			graphicsDevice.UpdateBuffer(quadIndexBuffer, 0, _quadIndices);
			#endregion
			ultralightVertexBuffer = factory.CreateBuffer(quadBufferDescription);
			graphicsDevice.UpdateBuffer(ultralightVertexBuffer, 0, new VertexPositionTexture[] {
				new(new(-.7f, -.4f), new(0,0)),
				new(new(-.3f, -.4f), new(1,0)),
				new(new(-.7f, -.8f), new(0,1)),
				new(new(-.3f, -.8f), new(1,1)),
			});
			ultralightPathVertexBuffer = factory.CreateBuffer(quadBufferDescription);
			graphicsDevice.UpdateBuffer(ultralightPathVertexBuffer, 0, new VertexPositionTexture[] {
				new(new(.3f, -.4f), new(0,0)),
				new(new(.7f, -.4f), new(1,0)),
				new(new(.3f, -.8f), new(0,1)),
				new(new(.7f, -.8f), new(1,1)),
			});
			uniformBuffer = factory.CreateBuffer(new(768, BufferUsage.UniformBuffer));

			ultralightVertexTest = factory.CreateBuffer(new(608, BufferUsage.VertexBuffer));

			VertexUltralightData _get_data(float x, float y, float u, float v)
			{
				return new VertexUltralightData()
				{
					in_Position = new(x / 512, y / 512),
					in_Color = new(255, 255, 255, 255),
					in_TexCoord = new(u, v),
					in_Data0 = new(0.5f, 0f, 0f, 0f)
				};
			}
			graphicsDevice.UpdateBuffer(ultralightVertexTest, 0, new[] {
				_get_data(-512, 512,0,0), _get_data(512,512,1,0),
				_get_data(-512,-512,0,1), _get_data(512,-512, 1,1),
			});
			ultralightVertexTestIndex = factory.CreateBuffer(new(
				6 * sizeof(ushort), BufferUsage.IndexBuffer
			));
			graphicsDevice.UpdateBuffer(ultralightVertexTestIndex, 0, new ushort[] { 0, 1, 3, 0, 2, 3 });
		}
		ResourceLayout basicQuadResourceLayout = null;
		ResourceLayout ultralightResourceLayout = null;
		private void CreatePipeline()
		{
			TextureSampler = graphicsDevice.Aniso4xSampler;

			CreateShaders();
			CreateBuffers();

			#region QUAD

			basicQuadResourceLayout = factory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("_sampler",
						ResourceKind.Sampler,
						ShaderStages.Fragment
					),
					new ResourceLayoutElementDescription("_texture",
						ResourceKind.TextureReadOnly,
						ShaderStages.Fragment
					)
				)
			);


			ultralightResourceLayout = factory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("Texture1", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
					new ResourceLayoutElementDescription("Texture2", ResourceKind.TextureReadOnly, ShaderStages.Fragment)
				// new ResourceLayoutElementDescription("Texture3", ResourceKind.TextureReadOnly, ShaderStages.Fragment)
				)
			);

			//ultralightResourceSet = factory.CreateResourceSet(new ResourceSetDescription(ultralightResourceLayout));

			ResourceLayout uniformsResourceLayout = factory.CreateResourceLayout(
					new ResourceLayoutDescription(
						new ResourceLayoutElementDescription(
							"Uniforms",
							ResourceKind.UniformBuffer,
							ShaderStages.Vertex | ShaderStages.Fragment
						)
					)
				);

			uniformResourceSet = factory.CreateResourceSet(new ResourceSetDescription(
				uniformsResourceLayout,
				uniformBuffer
			));

			GraphicsPipelineDescription mainPipelineDescription = new(
				BlendStateDescription.SingleAlphaBlend,
				new DepthStencilStateDescription(
					depthTestEnabled: false,
					depthWriteEnabled: true,
					comparisonKind: ComparisonKind.LessEqual),
				new RasterizerStateDescription(
					cullMode: FaceCullMode.Back,
					fillMode: PolygonFillMode.Solid,
					frontFace: FrontFace.Clockwise,
					depthClipEnabled: true,
					scissorTestEnabled: false),
				PrimitiveTopology.TriangleStrip,
				new ShaderSetDescription(
					new VertexLayoutDescription[] {
						new VertexLayoutDescription(
							new VertexElementDescription(
								"vPos",
								VertexElementSemantic.TextureCoordinate,
								VertexElementFormat.Float2
							),
							new VertexElementDescription(
								"fUv",
								VertexElementSemantic.TextureCoordinate,
								VertexElementFormat.Float2
							)
						)
					}, new[] {
						mainv,
						mainf
					}
				),
				new ResourceLayout[] {
					basicQuadResourceLayout
				},
				graphicsDevice.SwapchainFramebuffer.OutputDescription
			);
			mainPipeline = factory.CreateGraphicsPipeline(mainPipelineDescription);

			#endregion

			ultralightOutputTexture = factory.CreateTexture(new(
				512,
				512,
				1,
				1,
				1,
				PixelFormat.R8_G8_B8_A8_UNorm_SRgb,
				TextureUsage.RenderTarget,
				TextureType.Texture2D));

			ultralightPathOutputTexture = factory.CreateTexture(new(
				512,
				512,
				1,
				1,
				1,
				PixelFormat.R8_G8_B8_A8_UNorm_SRgb,
				TextureUsage.RenderTarget,
				TextureType.Texture2D));

			testTexture = factory.CreateTexture(new(
				256,
				256,
				1,
				9,
				1,
				PixelFormat.R8_G8_B8_A8_UNorm,
				TextureUsage.Sampled,
				TextureType.Texture2D));


			ultralightOutputBuffer = factory.CreateFramebuffer(
				new FramebufferDescription()
				{
					ColorTargets = new[] {
						new FramebufferAttachmentDescription(ultralightOutputTexture, 0)
					}
				}
			);
			ultralightPathOutputBuffer = factory.CreateFramebuffer(
				new FramebufferDescription()
				{
					ColorTargets = new[] {
						new FramebufferAttachmentDescription(ultralightPathOutputTexture, 0)
					}
				}
			);

			tv = factory.CreateTextureView(testTexture);

			flushedTextureViewResourceSet = factory.CreateResourceSet(
				new ResourceSetDescription(
					ultralightResourceLayout,
					tv,
					tv
				)
			);

			#region Async Flushed Image Loading
			Task.Run(async () =>
			{
				// fetch
				WebRequest request = WebRequest.CreateHttp("https://emoji.gg/assets/emoji/2446_cursed_flushed.png");
				// parse
				ImageSharpTexture imageSharpTexture = new((await request.GetResponseAsync()).GetResponseStream());
				// create texture
				Texture omg = imageSharpTexture.CreateDeviceTexture(graphicsDevice, factory);

				#region upload to gpu
				CommandList cl = factory.CreateCommandList();
				cl.Begin();
				cl.CopyTexture(omg, tv.Target);
				cl.End();
				graphicsDevice.SubmitCommands(cl);
				#endregion


				#region dispose
				cl.Dispose();
				cl = null;
				omg.Dispose();
				omg = null;
				imageSharpTexture = null;
				request = null;
				#endregion
			});
			#endregion

			// uncomment to see flushed (~0_0~)
			/*mainResourceSet = factory.CreateResourceSet(
				new ResourceSetDescription(
					mainResourceLayout,
					tv
				)
			);*/

			#region Ultralight

			GraphicsPipelineDescription ultralightPipelineDescription = new(
				BlendStateDescription.SingleAlphaBlend,
				new DepthStencilStateDescription(
					depthTestEnabled: false,
					depthWriteEnabled: true,
					comparisonKind: ComparisonKind.LessEqual),
				new RasterizerStateDescription(
					cullMode: FaceCullMode.None,
					fillMode: PolygonFillMode.Solid,
					frontFace: FrontFace.Clockwise,
					depthClipEnabled: true,
					scissorTestEnabled: false),
				PrimitiveTopology.TriangleList,
				new ShaderSetDescription(
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
					}, new[] {
						vertexShader,
						fragmentShader
					}
				),
				new ResourceLayout[] {
					uniformsResourceLayout,
					ultralightResourceLayout
				},
				ultralightOutputBuffer.OutputDescription
			);
			ultralightPipeline = factory.CreateGraphicsPipeline(ultralightPipelineDescription);
			#endregion

			#region Ultralight Path
			/*ultralightPathOutputTexture = factory.CreateTexture(
				new(
					512,
					512,
					1,
					1,
					1,
					PixelFormat.R8_G8_B8_A8_UInt,
					TextureUsage.Sampled | TextureUsage.Storage | TextureUsage.RenderTarget,
					TextureType.Texture2D
				)
			);*/

			/*ultralightPathOutputBuffer = factory.CreateFramebuffer(
				new FramebufferDescription()
				{
					ColorTargets = new[] {
						new FramebufferAttachmentDescription(ultralightPathOutputTexture, 0)
					}
				}
			);*/

			GraphicsPipelineDescription ultralightPathPipelineDescription = new(
				BlendStateDescription.SingleAlphaBlend,
				new DepthStencilStateDescription(
					depthTestEnabled: false,
					depthWriteEnabled: true,
					comparisonKind: ComparisonKind.LessEqual),
				new RasterizerStateDescription(
					cullMode: FaceCullMode.None,
					fillMode: PolygonFillMode.Solid,
					frontFace: FrontFace.Clockwise,
					depthClipEnabled: true,
					scissorTestEnabled: false),
				PrimitiveTopology.TriangleList,
				new ShaderSetDescription(
					new VertexLayoutDescription[] {
						new VertexLayoutDescription(
							20,
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
							)
						)
					}, new[] {
						pathVertexShader,
						pathFragmentShader
					}
				), uniformsResourceLayout,
				ultralightPathOutputBuffer.OutputDescription
			);


			ultralightPathPipeline = factory.CreateGraphicsPipeline(ultralightPathPipelineDescription);

			#endregion

			commandList = factory.CreateCommandList();
		}
	}
}
