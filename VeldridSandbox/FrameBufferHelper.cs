/**
 * ORIGINAL CREATOR:
 * Discord: Eightvo#9485
 * he send it to Veldrid server
 * and it worked :3
 */

using Veldrid;

namespace VeldridSandbox
{
	public class FrameBufferHelper
	{
		public static Framebuffer CreateFramebuffer(GraphicsDevice device, uint resolutionX, uint resolutionY, PixelFormat pixelFormat)
		{
			var drawTrgt = device.ResourceFactory.CreateTexture(
				new TextureDescription(resolutionX,
									   resolutionY,
									   1, 1, 1,
									   pixelFormat,
									   TextureUsage.RenderTarget | TextureUsage.Sampled | TextureUsage.Storage,
									   TextureType.Texture2D)
			);

			var depthTrgt = device.ResourceFactory.CreateTexture(
				new TextureDescription(resolutionY,
									   resolutionY,
									   1, 1, 1,
									   PixelFormat.R32_Float,
									   TextureUsage.DepthStencil,
									   TextureType.Texture2D)
			);

			FramebufferAttachmentDescription[] cltTrgs = new FramebufferAttachmentDescription[1]
			{
				new FramebufferAttachmentDescription()
				{
					ArrayLayer=0,
					MipLevel=0,
					Target=drawTrgt
				}
			};

			FramebufferAttachmentDescription depTrg = new FramebufferAttachmentDescription()
			{
				ArrayLayer = 0,
				MipLevel = 0,
				Target = depthTrgt
			};

			var frameBuffDesc = new FramebufferDescription()
			{
				ColorTargets = cltTrgs,
				//DepthTarget = depTrg
			};
			var offscreenBuffer = device.ResourceFactory.CreateFramebuffer(frameBuffDesc);
			return offscreenBuffer;
		}
	}
}
