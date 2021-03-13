using Supine.UltralightSharp.Enums;
using Supine.UltralightSharp.Safe;
using System;
using System.Collections.Generic;
using Veldrid;

namespace VeldridSandbox
{
	partial class Program
	{
		private void RenderUltralight()
		{
			graphicsDevice.WaitForIdle();

			graphicsDevice.WaitForIdle();
			
			foreach (var command in queuedCommands)
			{
				ref readonly var state = ref command.GpuState;
				var index = (int)state.RenderBufferId - 1;
				var rb = RenderBufferEntries[index];

				switch (command.CommandType)
				{
					case Supine.UltralightSharp.Enums.CommandType.ClearRenderBuffer:
						commandList.SetFramebuffer(rb.FrameBuffer);
						commandList.ClearColorTarget(0, RgbaFloat.Clear);
						//Console.WriteLine("cleared buffer");
						break;
					case Supine.UltralightSharp.Enums.CommandType.DrawGeometry:
						#region State
						uniforms.State.X = stopwatch.Elapsed.Seconds;
						uniforms.State.Y = width;
						uniforms.State.Z = height;
						uniforms.State.W = 1;
						#endregion
						#region Transform
						var txf =
							Ultralight.ApplyProjection(
								state.Transform,
								state.ViewportWidth,
								state.ViewportHeight,
								false
							);
						uniforms.Transform = txf;
						#endregion
						#region Scalar4
						uniforms.Scalar4_0.X = state.UniformScalars[0];
						uniforms.Scalar4_0.Y = state.UniformScalars[1];
						uniforms.Scalar4_0.Z = state.UniformScalars[2];
						uniforms.Scalar4_0.W = state.UniformScalars[3];
						uniforms.Scalar4_1.X = state.UniformScalars[4];
						uniforms.Scalar4_1.Y = state.UniformScalars[5];
						uniforms.Scalar4_1.Z = state.UniformScalars[6];
						uniforms.Scalar4_1.W = state.UniformScalars[7];
						#endregion
						#region Vector
						uniforms.Vector_0 = state.UniformVectors[0];
						uniforms.Vector_1 = state.UniformVectors[1];
						uniforms.Vector_2 = state.UniformVectors[2];
						uniforms.Vector_3 = state.UniformVectors[3];
						uniforms.Vector_4 = state.UniformVectors[4];
						uniforms.Vector_5 = state.UniformVectors[5];
						uniforms.Vector_6 = state.UniformVectors[6];
						uniforms.Vector_7 = state.UniformVectors[7];
						#endregion
						#region fClipSize
						uniforms.fClipSize = state.ClipSize;
						#endregion
						#region Clip
						uniforms.Clip_0 = state.Clip[0];
						uniforms.Clip_1 = state.Clip[1];
						uniforms.Clip_2 = state.Clip[2];
						uniforms.Clip_3 = state.Clip[3];
						uniforms.Clip_4 = state.Clip[4];
						uniforms.Clip_5 = state.Clip[5];
						uniforms.Clip_6 = state.Clip[6];
						uniforms.Clip_7 = state.Clip[7];
						#endregion
						graphicsDevice.UpdateBuffer(uniformBuffer, 0, uniforms);

						bool fill = true;
						switch (command.GpuState.ShaderType)
						{
							case Supine.UltralightSharp.Enums.ShaderType.Fill:
								commandList.SetPipeline(ultralightPipeline);
								//commandList.SetFramebuffer(ultralightOutputBuffer);
								break;
							case Supine.UltralightSharp.Enums.ShaderType.FillPath:
								commandList.SetPipeline(ultralightPathPipeline);
								//commandList.SetFramebuffer(ultralightPathOutputBuffer);
								fill = false;
								break;
							default: throw new ArgumentOutOfRangeException(nameof(ShaderType));
						}

						//if (command.GpuState.ShaderType == Supine.UltralightSharp.Enums.ShaderType.FillPath) continue;
						commandList.SetFramebuffer(rb.FrameBuffer);
						//commandList.SetFullViewports();
						commandList.SetViewport(0, new Viewport(0, 0, state.ViewportWidth, state.ViewportHeight, 0, 1));
						var entry = GeometryEntries[(int)command.GeometryId - 1];

						commandList.SetGraphicsResourceSet(0, uniformResourceSet);

						var texIndex1 = (int)state.Texture1Id - 1;
						var texIndex2 = (int)state.Texture2Id - 1;

						if (state.ShaderType == ShaderType.Fill)
						{
							if (texIndex1 > 0) commandList.SetGraphicsResourceSet(1, TextureEntries[texIndex1].textureResource);
							else commandList.SetGraphicsResourceSet(1, flushedTextureViewResourceSet);
							if (texIndex2 > 0) commandList.SetGraphicsResourceSet(2, TextureEntries[texIndex2].textureResource);
							else commandList.SetGraphicsResourceSet(2, flushedTextureViewResourceSet);
						}
						commandList.SetVertexBuffer(0, entry.VertexBuffer);
						if (state.EnableScissor)
						{
							ref readonly var r = ref state.ScissorRect;
							commandList.SetScissorRect(0, (uint)r.Left, (uint)r.Top, (uint)(r.Right - r.Left), (uint)(r.Bottom - r.Top));
						}

						commandList.SetIndexBuffer(entry.IndiciesBuffer, IndexFormat.UInt32);
						unsafe
						{
							commandList.DrawIndexed(
								command.IndicesCount,
								1,
								command.IndicesOffset,
								0,
								0
							);
						}

						break;
				}
			}
			queuedCommands.Clear();

			//commandList.End();
			//graphicsDevice.SubmitCommands(commandList);
			//graphicsDevice.SwapBuffers();
		}
	}
}
