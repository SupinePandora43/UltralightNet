using Supine.UltralightSharp.Enums;
using Supine.UltralightSharp.Safe;
using System;
using System.Runtime.CompilerServices;
using Veldrid;

namespace VeldridSandbox
{
	public partial class Program
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe void Render()
		{
			commandList.Begin();

			RenderUltralight();

			commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
			//commandList.SetFullViewports();
			commandList.ClearColorTarget(0, RgbaFloat.Cyan);

			if (mainResourceSet != null)
			{
				commandList.SetPipeline(mainPipeline);
				commandList.SetGraphicsResourceSet(0, mainResourceSet);
				commandList.SetVertexBuffer(0, vertexBuffer);
				commandList.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);

				commandList.DrawIndexed(
					indexCount: 4,
					instanceCount: 1,
					indexStart: 0,
					vertexOffset: 0,
					instanceStart: 0);
			}
			else
			{
				RenderTarget rt = view.GetRenderTarget();
				var rbIndex = (int)rt.RenderBufferId - 1;
				RenderBufferEntry rbEntry = RenderBufferEntries[rbIndex];
				TextureView tvRT = rbEntry.TextureEntry.TextureView;

				mainResourceSet = factory.CreateResourceSet(new ResourceSetDescription(mainResourceLayout, tvRT));
			}
			commandList.End();
			graphicsDevice.SubmitCommands(commandList);
			graphicsDevice.SwapBuffers();
		}
	}
}
