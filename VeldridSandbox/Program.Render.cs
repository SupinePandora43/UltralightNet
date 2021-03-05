using Supine.UltralightSharp.Enums;
using Supine.UltralightSharp.Safe;
using System;
using System.Runtime.CompilerServices;
using Veldrid;

namespace VeldridSandbox
{
	public partial class Program
	{
		TextureView rttv;
		private unsafe void Render()
		{
			commandList.Begin();

			//RenderUltralight();

			commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
			//commandList.SetFullViewports();
			commandList.ClearColorTarget(0, RgbaFloat.Orange);

			#region RenderTarget
			RenderTarget rt = view.GetRenderTarget();
			var rbIndex = (int)rt.RenderBufferId - 1;
			RenderBufferEntry rbEntry = RenderBufferEntries[rbIndex];
			TextureView tvRT = rbEntry.TextureEntry.TextureView;

			if (rttv != tvRT)
			{
				rttv = tvRT;
				if (rttv != null) rttv.Dispose();
				if (mainResourceSet != null) mainResourceSet.Dispose();
				mainResourceSet = factory.CreateResourceSet(new ResourceSetDescription(basicQuadResourceLayout, rttv));
			}
			commandList.SetPipeline(mainPipeline);
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
			#region Ultralight output
			commandList.SetGraphicsResourceSet(0, ultralightResourceSet);
			commandList.SetVertexBuffer(0, ultralightVertexBuffer);
			commandList.SetIndexBuffer(quadIndexBuffer, IndexFormat.UInt16);

			commandList.DrawIndexed(
				indexCount: 4,
				instanceCount: 1,
				indexStart: 0,
				vertexOffset: 0,
				instanceStart: 0);
			#endregion
			#region Ultralight Path output
			commandList.SetGraphicsResourceSet(0, ultralightPathResourceSet);
			commandList.SetVertexBuffer(0, ultralightPathVertexBuffer);
			commandList.SetIndexBuffer(quadIndexBuffer, IndexFormat.UInt16);

			commandList.DrawIndexed(
				indexCount: 4,
				instanceCount: 1,
				indexStart: 0,
				vertexOffset: 0,
				instanceStart: 0);
			#endregion

			#region UL Shader Test

			commandList.SetPipeline(ultralightPipeline);

			commandList.SetGraphicsResourceSet(0, uniformResourceSet);
			commandList.SetGraphicsResourceSet(1, flushedTextureViewResourceSet);

			commandList.SetIndexBuffer(ultralightVertexTestIndex, IndexFormat.UInt16);
			commandList.SetVertexBuffer(0, ultralightVertexTest);

			commandList.SetFramebuffer(ultralightOutputBuffer);

			/*commandList.DrawIndexed(
				indexCount: 4,
				instanceCount: 1,
				indexStart: 0,
				vertexOffset: 0,
				instanceStart: 0);*/
			commandList.DrawIndexed(6);
			#endregion

			commandList.End();
			graphicsDevice.SubmitCommands(commandList);
			graphicsDevice.SwapBuffers();
		}
	}
}
