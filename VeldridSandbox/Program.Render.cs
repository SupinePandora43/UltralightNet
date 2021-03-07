using Supine.UltralightSharp.Safe;
using System;
using System.Collections.Generic;
using Veldrid;

namespace VeldridSandbox
{
	public partial class Program
	{
		readonly Dictionary<uint, Tuple<ResourceSet, Texture>> resourceSets = new();

		private unsafe void Render()
		{
			commandList.Begin();

			RenderUltralight();

			commandList.SetPipeline(mainPipeline);
			commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
			commandList.SetFullViewports();
			commandList.ClearColorTarget(0, RgbaFloat.Clear);

			#region RenderTarget
			RenderTarget rt = view.GetRenderTarget();


			if ((!resourceSets.TryGetValue(rt.RenderBufferId, out Tuple<ResourceSet, Texture> resourceSet)) || resourceSet.Item2 == null || resourceSet.Item2.IsDisposed)
			{
				foreach (var disposeTuple in resourceSets)
				{
					disposeTuple.Value.Item1.Dispose();
					disposeTuple.Value.Item2?.Dispose();
				}
				resourceSets.Clear();

				var rbIndex = (int)rt.RenderBufferId - 1;
				RenderBufferEntry rbEntry = RenderBufferEntries[rbIndex];
				resourceSet = new(factory.CreateResourceSet(new ResourceSetDescription(basicQuadResourceLayout, TextureSampler, rbEntry.TextureEntry.Texure)), rbEntry.TextureEntry.Texure);
				resourceSets.Add(rt.RenderBufferId, resourceSet);
			}

			commandList.SetGraphicsResourceSet(0, resourceSet.Item1);
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
