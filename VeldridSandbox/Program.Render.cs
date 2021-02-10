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
			//commandList.SetPipeline(mainPipeline);
			//commandList.SetGraphicsResourceSet(0, ultralightResourceSet);
			foreach (var command in queuedCommands)
			{
				ref readonly var state = ref command.GpuState;
				var index = (int)state.RenderBufferId - 1;
				var rb = RenderBufferEntries[index];

				#region State
				uniforms.State.X = (float)stopwatch.ElapsedMilliseconds;
				uniforms.State.Y = state.ViewportWidth;
				uniforms.State.Z = state.ViewportHeight;
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

				switch (command.CommandType)
				{
					case CommandType.ClearRenderBuffer:
						commandList.SetFramebuffer(rb.FrameBuffer);
						commandList.ClearColorTarget(0, RgbaFloat.Blue);
						break;
					case CommandType.DrawGeometry:
						commandList.SetFramebuffer(rb.FrameBuffer);
						var entry = GeometryEntries[(int)command.GeometryId - 1];

						switch (command.GpuState.ShaderType)
						{
							case ShaderType.Fill:
								commandList.SetPipeline(ultralightPipeline);
								break;
							case ShaderType.FillPath:
								commandList.SetPipeline(ultralightPathPipeline);
								break;
							default: throw new ArgumentOutOfRangeException(nameof(ShaderType));
						}

						commandList.SetVertexBuffer(0, entry.VertexBuffer);
						commandList.SetIndexBuffer(entry.IndiciesBuffer, IndexFormat.UInt16);

						commandList.DrawIndexed(
							command.IndicesCount,
							1,
							0,
							0,
							command.IndicesOffset
						);

						break;
				}
			}
			queuedCommands.Clear();

			//commandList.End();
			//graphicsDevice.SubmitCommands(commandList);
			//graphicsDevice.SwapBuffers();

			// draw it

			RenderTarget rt = view.GetRenderTarget();
			var rbIndex = (int)rt.RenderBufferId - 1;
			RenderBufferEntry rbEntry = RenderBufferEntries[rbIndex];
			Texture tex = rbEntry.TextureEntry.Texure;

			//commandList.Begin();

			commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
			commandList.SetFullViewports();
			commandList.ClearColorTarget(0, RgbaFloat.Cyan);
			commandList.SetPipeline(mainPipeline);
			commandList.SetGraphicsResourceSet(0, mainResourceSet);
			commandList.SetVertexBuffer(0, vertexBuffer);
			commandList.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);

			commandList.DrawIndexed(
				indexCount: 6,
				instanceCount: 1,
				indexStart: 0,
				vertexOffset: 0,
				instanceStart: 0);

			commandList.End();
			graphicsDevice.SubmitCommands(commandList);
			graphicsDevice.SwapBuffers();
		}
	}
}
