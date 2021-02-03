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
		private Pipeline ultralightPipeline;
		private Pipeline ultralightPathPipeline;

		private Framebuffer ultralightOutputBuffer;
		private Texture ultralightOutputTexture;
		private ResourceSet mainResourceSet;
		private ResourceSet ultralightResourceSet;

		Shader mainv;
		Shader mainf;
		Shader vertexShader;
		Shader fragmentShader;
		Shader pathVertexShader;
		Shader pathFragmentShader;

		private void CreateShaders()
		{
			mainv = factory.CreateShader(new ShaderDescription(ShaderStages.Vertex, GetShaderBytes("embedded.basic.vert.glsl"), "main"));
			mainf = factory.CreateShader(new ShaderDescription(ShaderStages.Fragment, GetShaderBytes("embedded.basic.frag.glsl"), "main"));
			vertexShader = GetShader("embedded.shader_v2f_c4f_t2f_t2f_d28f.vert.glsl", ShaderStages.Vertex);
			fragmentShader = GetShader("embedded.shader_fill.frag.glsl", ShaderStages.Fragment);
			pathVertexShader = GetShader("embedded.shader_v2f_c4f_t2f.vert.glsl", ShaderStages.Vertex);
			pathFragmentShader = GetShader("embedded.shader_fill_path.frag.glsl", ShaderStages.Fragment);
		}

		private void CreatePipeline()
		{
			CreateShaders();

			ResourceLayout mainResourceLayout = factory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("iTex", ResourceKind.Sampler, ShaderStages.Fragment)));


			ResourceLayout ultralightResourceLayout = factory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("Texture1", ResourceKind.Sampler, ShaderStages.Fragment),
					new ResourceLayoutElementDescription("Texture2", ResourceKind.Sampler, ShaderStages.Fragment),
					new ResourceLayoutElementDescription("Texture3", ResourceKind.Sampler, ShaderStages.Fragment)));

			//ultralightResourceSet = factory.CreateResourceSet(new ResourceSetDescription(ultralightResourceLayout));

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
								"Position",
								VertexElementSemantic.TextureCoordinate,
								VertexElementFormat.Float2
							)
						)
					}, new[] {
						//mainv,
						//mainf,
						vertexShader,
						fragmentShader,
						pathVertexShader,
						pathFragmentShader
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

			//pipelineDescription.Outputs = UltralightBuffer.OutputDescription;
			//UltralightPipeline = factory.CreateGraphicsPipeline(pipelineDescription);

			commandList = factory.CreateCommandList();
		}
	}
}
