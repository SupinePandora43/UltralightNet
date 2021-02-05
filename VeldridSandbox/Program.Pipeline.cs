using System;
using Veldrid;

namespace VeldridSandbox
{
	public partial class Program
	{
		private GraphicsDevice graphicsDevice;
		private ResourceFactory factory;

		private CommandList commandList;

		private Pipeline mainPipeline;
		private Sampler mainSampler;
		private Pipeline ultralightPipeline;
		private Pipeline ultralightPathPipeline;

		private Framebuffer ultralightOutputBuffer;
		private Texture ultralightOutputTexture;
		private ResourceSet mainResourceSet;
		private ResourceSet ultralightResourceSet;
		private ResourceSet ultralightPathResourceSet;

		Shader mainv;
		Shader mainf;
		Shader vertexShader;
		Shader fragmentShader;
		Shader pathVertexShader;
		Shader pathFragmentShader;

		DeviceBuffer vertexBuffer;
		DeviceBuffer indexBuffer;

		private static readonly float[] _quadVerts = {
			1f, 1f, 0f,
			1f, -1f, 0f,
			-1f, -1f, 0f,
			-1f, 1f, 1f
		};
		private static readonly uint[] _quadIndices = {
			0, 1, 3,
			1, 2, 3
		};

		private void CreateShaders()
		{
			mainv = factory.CreateShader(new ShaderDescription(ShaderStages.Vertex, GetShaderBytes("embedded.basic.vert.glsl"), "main"));
			mainf = factory.CreateShader(new ShaderDescription(ShaderStages.Fragment, GetShaderBytes("embedded.basic.frag.glsl"), "main"));
			vertexShader = GetShader("embedded.shader_v2f_c4f_t2f_t2f_d28f.vert.glsl", ShaderStages.Vertex);
			fragmentShader = GetShader("embedded.shader_fill.frag.glsl", ShaderStages.Fragment);
			pathVertexShader = GetShader("embedded.shader_v2f_c4f_t2f.vert.glsl", ShaderStages.Vertex);
			pathFragmentShader = GetShader("embedded.shader_fill_path.frag.glsl", ShaderStages.Fragment);
		}

		private void CreateBuffers()
		{
			vertexBuffer = factory.CreateBuffer(new(48, BufferUsage.VertexBuffer));
			graphicsDevice.UpdateBuffer(vertexBuffer, 0, _quadVerts);

			indexBuffer = factory.CreateBuffer(new(sizeof(uint) * 6, BufferUsage.IndexBuffer));
			graphicsDevice.UpdateBuffer(indexBuffer, 0, _quadIndices);
		}

		private void CreatePipeline()
		{
			CreateShaders();
			CreateBuffers();

			ResourceLayout mainResourceLayout = factory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("iTex", ResourceKind.Sampler, ShaderStages.Fragment)));

			mainResourceSet = factory.CreateResourceSet(new ResourceSetDescription(mainResourceLayout, graphicsDevice.Aniso4xSampler));

			ResourceLayout ultralightResourceLayout = factory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("Texture1", ResourceKind.Sampler, ShaderStages.Fragment),
					new ResourceLayoutElementDescription("Texture2", ResourceKind.Sampler, ShaderStages.Fragment),
					new ResourceLayoutElementDescription("Texture3", ResourceKind.Sampler, ShaderStages.Fragment)));

			ultralightResourceSet = factory.CreateResourceSet(new ResourceSetDescription(ultralightResourceLayout));

			ResourceLayout ultralightUniformsResourceLayout = factory.CreateResourceLayout(
					new ResourceLayoutDescription(
						new ResourceLayoutElementDescription(
							"Uniforms",
							ResourceKind.UniformBuffer,
							ShaderStages.Vertex
						),
						new ResourceLayoutElementDescription(
							"Uniforms",
							ResourceKind.UniformBuffer,
							ShaderStages.Fragment
						)
					)
				);
			GraphicsPipelineDescription mainPipelineDescription = new(
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
								"vPos",
								VertexElementSemantic.Position, // TextureCoordinate
								VertexElementFormat.Float3
							),
							new VertexElementDescription(
								"fUv",
								VertexElementSemantic.TextureCoordinate,
								VertexElementFormat.Float2
							)
						),
						new VertexLayoutDescription(
							new VertexElementDescription(
								"fUv",
								VertexElementSemantic.TextureCoordinate,
								VertexElementFormat.Float2
							)
						)
					}, new[] {
						mainv,
						mainf,
						//vertexShader,
						//fragmentShader,
						//pathVertexShader,
						//pathFragmentShader
					}
				),
				new ResourceLayout[] {
					mainResourceLayout,
					ultralightResourceLayout,
					ultralightUniformsResourceLayout
				},
				graphicsDevice.SwapchainFramebuffer.OutputDescription
			);
			mainPipeline = factory.CreateGraphicsPipeline(mainPipelineDescription);

			ultralightOutputTexture = factory.CreateTexture(new(
				512,
				512,
				1,
				1,
				1,
				PixelFormat.R8_G8_B8_A8_SInt,
				TextureUsage.RenderTarget,
				TextureType.Texture2D));
			ultralightOutputBuffer = factory.CreateFramebuffer(new FramebufferDescription()
			{
				ColorTargets = new[] { new FramebufferAttachmentDescription(ultralightOutputTexture, 0) }
			});

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
								"Position",
								VertexElementSemantic.TextureCoordinate,
								VertexElementFormat.Float2
							)
						)
					}, new[] {
						vertexShader,
						fragmentShader
						//pathVertexShader,
						//pathFragmentShader
					}
				),
				new ResourceLayout[] {
					mainResourceLayout,
					ultralightResourceLayout,
					ultralightUniformsResourceLayout
				},
				ultralightOutputBuffer.OutputDescription
			);

			ultralightPipeline = factory.CreateGraphicsPipeline(ultralightPipelineDescription);

			//pipelineDescription.Outputs = UltralightBuffer.OutputDescription;
			//UltralightPipeline = factory.CreateGraphicsPipeline(pipelineDescription);

			commandList = factory.CreateCommandList();
		}
	}
}
