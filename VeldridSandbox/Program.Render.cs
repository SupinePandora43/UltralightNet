using Supine.UltralightSharp.Safe;
using Veldrid;

namespace VeldridSandbox
{
	public partial class Program
	{
		TextureView rttv = null;
		private unsafe void Render()
		{
			commandList.Begin();

			RenderUltralight();

			commandList.SetPipeline(mainPipeline);
			commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
			commandList.SetFullViewports();
			commandList.ClearColorTarget(0, RgbaFloat.Orange);

			#region RenderTarget
			RenderTarget rt = view.GetRenderTarget();
			var rbIndex = (int)rt.RenderBufferId - 1;
			RenderBufferEntry rbEntry = RenderBufferEntries[rbIndex];
			TextureView tvRT = rbEntry.TextureEntry.TextureView;

			if (rttv != tvRT)
			{
				if (rttv != null) rttv.Dispose();
				rttv = tvRT;
				if (mainResourceSet != null) mainResourceSet.Dispose();
				mainResourceSet = factory.CreateResourceSet(new ResourceSetDescription(basicQuadResourceLayout, rttv));
			}
			commandList.SetGraphicsResourceSet(0, mainResourceSet);
			commandList.SetVertexBuffer(0, rtVertexBuffer);
			commandList.SetIndexBuffer(quadIndexBuffer, IndexFormat.UInt16);

			commandList.DrawIndexed(
				indexCount: 4,
				instanceCount: 1,
				indexStart: 0,
				vertexOffset: 0,
				instanceStart: 0);
			#endregion

			commandList.End();
			graphicsDevice.SubmitCommands(commandList);
			graphicsDevice.SwapBuffers();

			graphicsDevice.WaitForIdle();
		}
	}
}
