using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Numerics;
using Veldrid;
using Veldrid.ImageSharp;

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
		private ResourceSet ultralightResourceSet;
		private ResourceSet ultralightPathResourceSet;

		Sampler TextureSampler;

		Shader mainv;
		Shader mainf;
		Shader vertexShader;
		Shader fragmentShader;
		Shader pathVertexShader;
		Shader pathFragmentShader;

		DeviceBuffer vertexBuffer;
		DeviceBuffer indexBuffer;

		Uniforms uniforms = new();
		DeviceBuffer uniformBuffer;

		Texture testTexture;

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
			mainv = GetShader("embedded.basic.vert.glsl", ShaderStages.Vertex);
			mainf = GetShader("embedded.basic.frag.glsl", ShaderStages.Fragment);
			vertexShader = GetShader("embedded.shader_v2f_c4f_t2f_t2f_d28f.vert.glsl", ShaderStages.Vertex);
			fragmentShader = GetShader("embedded.shader_fill.frag.glsl", ShaderStages.Fragment);
			pathVertexShader = GetShader("embedded.shader_v2f_c4f_t2f.vert.glsl", ShaderStages.Vertex);
			pathFragmentShader = GetShader("embedded.shader_fill_path.frag.glsl", ShaderStages.Fragment);
		}

		private void CreateBuffers()
		{
			vertexBuffer = factory.CreateBuffer(new(VertexPositionTexture.SizeInBytes * 4, BufferUsage.VertexBuffer));
			graphicsDevice.UpdateBuffer(vertexBuffer, 0, new VertexPositionTexture[] {
				new(new(-.75f, .75f), new(1,0)),
				new(new(.75f, .75f), new(0,0)),
				new(new(-.75f, -.75f), new(1,1)),
				new(new(.75f, -.75f), new(0,1)),
			});

			/*vertexBuffer = factory.CreateBuffer(new(32, BufferUsage.VertexBuffer));
			graphicsDevice.UpdateBuffer(vertexBuffer, 0, _quadVerts);
			*/
			indexBuffer = factory.CreateBuffer(new(/*sizeof(uint) * 6*/ 4 * sizeof(short), BufferUsage.IndexBuffer));
			graphicsDevice.UpdateBuffer(indexBuffer, 0, _quadIndices);

			uniformBuffer = factory.CreateBuffer(new(768, BufferUsage.UniformBuffer));
		}

		private void CreatePipeline()
		{
			TextureSampler = graphicsDevice.Aniso4xSampler;

			CreateShaders();
			CreateBuffers();

			#region QUAD

			ResourceLayout mainResourceLayout = factory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("SurfaceTexture",
						ResourceKind.TextureReadOnly,
						ShaderStages.Fragment
					),
					new ResourceLayoutElementDescription("SurfaceSampler",
						ResourceKind.Sampler,
						ShaderStages.Fragment
					)
				)
			);


			ResourceLayout ultralightResourceLayout = factory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("Texture1", ResourceKind.Sampler, ShaderStages.Fragment),
					new ResourceLayoutElementDescription("Texture2", ResourceKind.Sampler, ShaderStages.Fragment)//,
																												 //new ResourceLayoutElementDescription("Texture3", ResourceKind.Sampler, ShaderStages.Fragment)
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
			GraphicsPipelineDescription mainPipelineDescription = new(
				BlendStateDescription.SingleAlphaBlend,
				new DepthStencilStateDescription(
					depthTestEnabled: true,
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
						)/*, new(
							
						)*/
					}, new[] {
						mainv,
						mainf
					}
				),
				new ResourceLayout[] {
					mainResourceLayout
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
				PixelFormat.R8_G8_B8_A8_UInt,
				TextureUsage.Sampled | TextureUsage.Storage | TextureUsage.RenderTarget,
				TextureType.Texture2D));

			Texture stagingTexture = factory.CreateTexture(new(
				512,
				512,
				1,
				1,
				1,
				PixelFormat.R8_G8_B8_A8_UNorm_SRgb,
				TextureUsage.Staging,
				TextureType.Texture2D));

			testTexture = factory.CreateTexture(new(
				128,
				128,
				1,
				8,
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

			//Stream omgStream = assembly.GetManifestResourceStream("VeldridSandbox.embedded.omg.png");

			WebRequest request = HttpWebRequest.CreateHttp("https://emoji.gg/assets/emoji/2446_cursed_flushed.png");
			
			ImageSharpTexture imageSharpTexture = new(request.GetResponse().GetResponseStream());

			unsafe
			{
				/*fixed (byte* pixels = omgRaw.ToArray())
				{
					graphicsDevice.UpdateTexture(stagingTexture, (IntPtr)pixels, 4 * 128 * 128, 0, 0, 0, 128, 128, 1, 0, 0);
				}*/
			}

			CommandList cl = factory.CreateCommandList();
			cl.Begin();

			Texture omg = imageSharpTexture.CreateDeviceTexture(graphicsDevice, factory);

			testTexture = omg;

			/*cl.CopyTexture(omg, testTexture);
			cl.End();
			graphicsDevice.SubmitCommands(cl);
			cl.Dispose();
			cl = null;
			*/
			mainResourceSet = factory.CreateResourceSet(
				new ResourceSetDescription(
					mainResourceLayout,
					testTexture,
					TextureSampler
				)
			);

			#region Ultralight

			GraphicsPipelineDescription ultralightPipelineDescription = new(
				BlendStateDescription.SingleOverrideBlend,
				new DepthStencilStateDescription(
					depthTestEnabled: true,
					depthWriteEnabled: true,
					comparisonKind: ComparisonKind.LessEqual),
				new RasterizerStateDescription(
					cullMode: FaceCullMode.None,
					fillMode: PolygonFillMode.Solid,
					frontFace: FrontFace.Clockwise,
					depthClipEnabled: true,
					scissorTestEnabled: false),
				PrimitiveTopology.TriangleStrip,
				new ShaderSetDescription(
					new VertexLayoutDescription[] {
						new VertexLayoutDescription(
							new VertexElementDescription(
								"in_Position",
								VertexElementSemantic.TextureCoordinate,
								VertexElementFormat.Float2
							),
							new VertexElementDescription(
								"in_Color",
								VertexElementSemantic.Color,
								VertexElementFormat.Float4
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
								VertexElementSemantic.Color,
								VertexElementFormat.Float4
							),
							new VertexElementDescription(
								"in_Data1",
								VertexElementSemantic.Color,
								VertexElementFormat.Float4
							),
							new VertexElementDescription(
								"in_Data2",
								VertexElementSemantic.Color,
								VertexElementFormat.Float4
							),
							new VertexElementDescription(
								"in_Data3",
								VertexElementSemantic.Color,
								VertexElementFormat.Float4
							),
							new VertexElementDescription(
								"in_Data4",
								VertexElementSemantic.Color,
								VertexElementFormat.Float4
							),
							new VertexElementDescription(
								"in_Data5",
								VertexElementSemantic.Color,
								VertexElementFormat.Float4
							),
							new VertexElementDescription(
								"in_Data6",
								VertexElementSemantic.Color,
								VertexElementFormat.Float4
							)
						),
						new VertexLayoutDescription(
							new VertexElementDescription(
								"ex_Color",
								VertexElementSemantic.Color,
								VertexElementFormat.Float4
							),
							new VertexElementDescription(
								"ex_TexCoord",
								VertexElementSemantic.TextureCoordinate,
								VertexElementFormat.Float2
							),
							new VertexElementDescription(
								"ex_ObjectCoord",
								VertexElementSemantic.TextureCoordinate,
								VertexElementFormat.Float2
							),
							new VertexElementDescription(
								"ex_ScreenCoord",
								VertexElementSemantic.TextureCoordinate,
								VertexElementFormat.Float2
							),
							new VertexElementDescription(
								"ex_Data0",
								VertexElementSemantic.Color,
								VertexElementFormat.Float4
							),
							new VertexElementDescription(
								"ex_Data1",
								VertexElementSemantic.Color,
								VertexElementFormat.Float4
							),
							new VertexElementDescription(
								"ex_Data2",
								VertexElementSemantic.Color,
								VertexElementFormat.Float4
							),
							new VertexElementDescription(
								"ex_Data3",
								VertexElementSemantic.Color,
								VertexElementFormat.Float4
							),
							new VertexElementDescription(
								"ex_Data4",
								VertexElementSemantic.Color,
								VertexElementFormat.Float4
							),
							new VertexElementDescription(
								"ex_Data5",
								VertexElementSemantic.Color,
								VertexElementFormat.Float4
							),
							new VertexElementDescription(
								"ex_Data6",
								VertexElementSemantic.Color,
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
			ultralightPathOutputTexture = factory.CreateTexture(
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
			);

			ultralightPathOutputBuffer = factory.CreateFramebuffer(
				new FramebufferDescription()
				{
					ColorTargets = new[] {
						new FramebufferAttachmentDescription(ultralightPathOutputTexture, 0)
					}
				}
			);

			GraphicsPipelineDescription ultralightPathPipelineDescription = new(
				BlendStateDescription.SingleOverrideBlend,
				new DepthStencilStateDescription(
					depthTestEnabled: true,
					depthWriteEnabled: true,
					comparisonKind: ComparisonKind.LessEqual),
				new RasterizerStateDescription(
					cullMode: FaceCullMode.None,
					fillMode: PolygonFillMode.Solid,
					frontFace: FrontFace.Clockwise,
					depthClipEnabled: true,
					scissorTestEnabled: false),
				PrimitiveTopology.TriangleStrip,
				new ShaderSetDescription(
					new VertexLayoutDescription[] {
						new VertexLayoutDescription(
							new VertexElementDescription(
								"in_Position",
								VertexElementSemantic.TextureCoordinate,
								VertexElementFormat.Float2
							),
							new VertexElementDescription(
								"in_Color",
								VertexElementSemantic.Color,
								VertexElementFormat.Float4
							),
							new VertexElementDescription(
								"in_TexCoord",
								VertexElementSemantic.TextureCoordinate,
								VertexElementFormat.Float2
							)
						),
						new VertexLayoutDescription(
							new VertexElementDescription(
								"ex_Color",
								VertexElementSemantic.Color,
								VertexElementFormat.Float4
							),
							new VertexElementDescription(
								"ex_ObjectCoord",
								VertexElementSemantic.TextureCoordinate,
								VertexElementFormat.Float2
							),
							new VertexElementDescription(
								"ex_ScreenCoord",
								VertexElementSemantic.TextureCoordinate,
								VertexElementFormat.Float2
							)
						)
					}, new[] {
						pathVertexShader,
						pathFragmentShader
					}
				),
				new ResourceLayout[] {
					uniformsResourceLayout
				},
				ultralightPathOutputBuffer.OutputDescription
			);



			ultralightPathPipeline = factory.CreateGraphicsPipeline(ultralightPathPipelineDescription);

			#endregion

			commandList = factory.CreateCommandList();
		}
	}
}
