using System;
using System.Collections.Generic;
using Veldrid;

namespace UltralightNet.Veldrid
{
	public class VeldridGPUDriver
	{
		private readonly GraphicsDevice graphicsDevice;
		private readonly ResourceLayout textureResourceLayout;

		public bool GenerateMipMaps = true;
		public uint MipLevels = 10;
		public TextureSampleCount SampleCount = TextureSampleCount.Count32;
		public bool WaitForIdle = true;

		private readonly SlottingList<TextureEntry> TextureEntries = new(32, 8);
		private readonly SlottingList<GeometryEntry> GeometryEntries = new(32, 8);
		private readonly SlottingList<RenderBufferEntry> RenderBufferEntries = new(8, 2);

		private Queue<ULCommand> commands = new();

		public VeldridGPUDriver(GraphicsDevice graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;

			textureResourceLayout = graphicsDevice.ResourceFactory.CreateResourceLayout(
				new ResourceLayoutDescription(
					new ResourceLayoutElementDescription("texture", ResourceKind.TextureReadOnly, ShaderStages.Fragment)
				)
			);
		}
		public ULGPUDriver GetGPUDriver() => new()
		{
			BeginSynchronize = nothing,
			EndSynchronize = nothing,

			NextTextureId = NextTextureId,
			NextGeometryId = NextGeometryId,
			NextRenderBufferId = NextRenderBufferId,

			CreateTexture = CreateTexture,
			UpdateTexture = UpdateTexture,
			DestroyTexture = DestroyTexture,

			CreateGeometry = CreateGeometry,
			UpdateGeometry = UpdateGeometry,
			DestroyGeometry = DestroyGeometry,

			CreateRenderBuffer = CreateRenderBuffer,
			DestroyRenderBuffer = DestroyRenderBuffer,

			UpdateCommandList = UpdateCommandList
		};
		private void nothing() { }

		#region NextId
		private uint NextTextureId()
		{
			return (uint)TextureEntries.Add(new());
		}
		public uint NextGeometryId()
		{
			return (uint)GeometryEntries.Add(new());
		}
		public uint NextRenderBufferId()
		{
			return (uint)RenderBufferEntries.Add(new());
		}
		#endregion NextId
		#region Texture
		private void CreateTexture(uint texture_id, ULBitmap bitmap)
		{
			bool isRT = bitmap.IsEmpty;
			TextureEntry entry = TextureEntries[(int)texture_id];
			TextureDescription textureDescription = new()
			{
				Type = TextureType.Texture2D,
				Usage = TextureUsage.Sampled,
				Width = bitmap.Width,
				Height = bitmap.Height,
				MipLevels = isRT ? 1 : MipLevels,
				SampleCount = isRT ? TextureSampleCount.Count1 : SampleCount,
				ArrayLayers = 1,
				Depth = 1
			};

			if (isRT)
			{
				textureDescription.Format = PixelFormat.R8_G8_B8_A8_UNorm_SRgb;
				textureDescription.Usage |= TextureUsage.RenderTarget;
			}
			else
			{
				ULBitmapFormat format = bitmap.Format;
				if (format is ULBitmapFormat.A8_UNORM)
				{
					textureDescription.Format = PixelFormat.R8_UNorm;
				}
				else if (format is ULBitmapFormat.BGRA8_UNORM_SRGB)
				{
					textureDescription.Format = PixelFormat.B8_G8_R8_A8_UNorm_SRgb;
				}
				else throw new NotSupportedException("format");
				if (GenerateMipMaps) textureDescription.Usage |= TextureUsage.GenerateMipmaps;
			}

			entry.texture = graphicsDevice.ResourceFactory.CreateTexture(textureDescription);

			if (!isRT)
			{
				graphicsDevice.UpdateTexture(entry.texture, bitmap.LockPixels(), (uint)bitmap.Size, 0, 0, 0, textureDescription.Width, textureDescription.Height, 1, 0, 0);
				bitmap.UnlockPixels();

				if (GenerateMipMaps)
				{
					var cl = graphicsDevice.ResourceFactory.CreateCommandList();
					cl.Begin();
					cl.GenerateMipmaps(entry.texture);
					cl.End();
					graphicsDevice.SubmitCommands(cl);
					cl.Dispose();
				}
			}

			entry.resourceSet = graphicsDevice.ResourceFactory.CreateResourceSet(
				new ResourceSetDescription(
					textureResourceLayout,
					entry.texture
				)
			);

			if (WaitForIdle) graphicsDevice.WaitForIdle();
		}
		private void UpdateTexture(uint texture_id, ULBitmap bitmap)
		{
			TextureEntry entry = TextureEntries[(int)texture_id];

			graphicsDevice.UpdateTexture(entry.texture, bitmap.LockPixels(), (uint)bitmap.Size, 0, 0, 0, bitmap.Width, bitmap.Height, 1, 0, 0);
			bitmap.UnlockPixels();

			if (GenerateMipMaps)
			{
				var cl = graphicsDevice.ResourceFactory.CreateCommandList();
				cl.Begin();
				cl.GenerateMipmaps(entry.texture);
				cl.End();
				graphicsDevice.SubmitCommands(cl);
				if (WaitForIdle) graphicsDevice.WaitForIdle();
				cl.Dispose();
			}
		}
		private void DestroyTexture(uint texture_id)
		{
			TextureEntry entry = TextureEntries.RemoveAt((int)texture_id);
			entry.texture.Dispose();
			entry.resourceSet.Dispose();
		}
		#endregion Texture
		#region Geometry
		private void CreateGeometry(uint geometry_id, ULVertexBuffer vertices, ULIndexBuffer indices)
		{
			GeometryEntry entry = GeometryEntries[(int)geometry_id];

			BufferDescription vertexDescription = new(vertices.size, BufferUsage.VertexBuffer);
			entry.vertices = graphicsDevice.ResourceFactory.CreateBuffer(ref vertexDescription);
			BufferDescription indexDescription = new(indices.size, BufferUsage.IndexBuffer);
			entry.indicies = graphicsDevice.ResourceFactory.CreateBuffer(ref indexDescription);

			graphicsDevice.UpdateBuffer(entry.vertices, 0, vertices.data, vertices.size);
			graphicsDevice.UpdateBuffer(entry.indicies, 0, indices.data, indices.size);

			if (WaitForIdle) graphicsDevice.WaitForIdle();
		}
		private void UpdateGeometry(uint geometry_id, ULVertexBuffer vertices, ULIndexBuffer indices)
		{
			GeometryEntry entry = GeometryEntries[(int)geometry_id];

			graphicsDevice.UpdateBuffer(entry.vertices, 0, vertices.data, vertices.size);
			graphicsDevice.UpdateBuffer(entry.indicies, 0, indices.data, indices.size);

			if (WaitForIdle) graphicsDevice.WaitForIdle();
		}
		private void DestroyGeometry(uint geometry_id)
		{
			GeometryEntry entry = GeometryEntries.RemoveAt((int)geometry_id);

			entry.vertices.Dispose();
			entry.indicies.Dispose();
		}
		#endregion
		#region RenderBuffer
		private void CreateRenderBuffer(uint render_buffer_id, ULRenderBuffer buffer)
		{
			RenderBufferEntry entry = RenderBufferEntries[(int)render_buffer_id];
			TextureEntry textureEntry = TextureEntries[(int)buffer.texture_id];

			entry.textureEntry = textureEntry;

			FramebufferDescription fd = new()
			{
				ColorTargets = new[] {
					new FramebufferAttachmentDescription(textureEntry.texture, 0)
				}
			};

			entry.framebuffer = graphicsDevice.ResourceFactory.CreateFramebuffer(ref fd);

			if (WaitForIdle) graphicsDevice.WaitForIdle();
		}
		private void DestroyRenderBuffer(uint render_buffer_id)
		{
			RenderBufferEntry entry = RenderBufferEntries[(int)render_buffer_id];
			entry.textureEntry = null;
			entry.framebuffer.Dispose();
		}
		#endregion RenderBuffer

		private void UpdateCommandList(ULCommandList list)
		{
			foreach (ULCommand command in list.ToSpan())
				commands.Enqueue(command);
		}

		private class TextureEntry
		{
			public Texture texture;
			public ResourceSet resourceSet;
		}
		private class GeometryEntry
		{
			public DeviceBuffer vertices;
			public DeviceBuffer indicies;
		}
		private class RenderBufferEntry
		{
			public Framebuffer framebuffer;
			public TextureEntry textureEntry;
		}
	}
}
