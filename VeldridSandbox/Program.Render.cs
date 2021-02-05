using Supine.UltralightSharp.Enums;
using Supine.UltralightSharp.Safe;
using System;
using Veldrid;

namespace VeldridSandbox
{
	public partial class Program
	{
		private unsafe void Render()
		{
			commandList.Begin();
			commandList.SetPipeline(mainPipeline);
			commandList.SetGraphicsResourceSet(0, ultralightResourceSet);
			foreach (var command in queuedCommands)
			{
				var index = (int)command.GpuState.RenderBufferId - 1;
				var rb = RenderBufferEntries[index];
				switch (command.CommandType)
				{
					case CommandType.ClearRenderBuffer:

						commandList.SetFramebuffer(rb.FrameBuffer);
						commandList.ClearColorTarget(0, RgbaFloat.Blue);
						break;
					case CommandType.DrawGeometry:
						commandList.SetFramebuffer(rb.FrameBuffer);
						switch (command.GpuState.ShaderType)
						{
							case ShaderType.Fill:
								break;
							case ShaderType.FillPath:
								break;
							default: throw new ArgumentOutOfRangeException(nameof(ShaderType));
						}

						break;
				}
			}
			queuedCommands.Clear();
			
			commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);


			commandList.SetFullViewports();
			//commandList.ClearColorTarget(0, RgbaFloat.DarkRed);

			commandList.End();
			graphicsDevice.SubmitCommands(commandList);
			graphicsDevice.SwapBuffers();
			
			// draw it

			RenderTarget rt = view.GetRenderTarget();
			var rbIndex = (int)rt.RenderBufferId - 1;
			RenderBufferEntry rbEntry = RenderBufferEntries[rbIndex];
			Texture tex = rbEntry.TextureEntry.Texure;
			MappedResource mappedTex = graphicsDevice.Map(tex, MapMode.Read);
			
		}
	}
}
