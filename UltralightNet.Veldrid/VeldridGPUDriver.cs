using System;
using System.Collections.Generic;
using Veldrid;

namespace UltralightNet.Veldrid
{
	public class VeldridGPUDriver
	{
		private readonly GraphicsDevice graphicsDevice;

		public bool GenerateMipMaps = true;
		public uint MipLevels = 10;
		public TextureSampleCount SampleCount = TextureSampleCount.Count32;
		public bool WaitForIdle = true;

		private readonly SlottingList<TextureEntry> TextureEntries = new(32, 8);
		private readonly SlottingList<GeometryEntry> GeometryEntries = new(32, 8);
		private readonly SlottingList<RenderBufferEntry> RenderBufferEntries = new(8, 2);

		public VeldridGPUDriver(GraphicsDevice graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;
		}
		public ULGPUDriver GetGPUDriver() => new()
		{
			NextTextureId = NextTextureId,
			NextGeometryId = NextGeometryId,
			NextRenderBufferId = NextRenderBufferId,
			CreateTexture = CreateTexture,
			UpdateTexture = UpdateTexture,
			BeginSynchronize = nothing,
			EndSynchronize = nothing
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

		private void CreateTexture(uint texture_id, ULBitmap bitmap)
		{
			bool isRT = bitmap.IsEmpty;
			TextureEntry entry = TextureEntries[(int)texture_id];
			TextureDescription textureDescription = new()
			{
				Type = TextureType.Texture2D,
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
				textureDescription.Usage = TextureUsage.RenderTarget;
			}
			else
			{
				textureDescription.Usage = TextureUsage.Sampled;
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
				graphicsDevice.UpdateTexture(entry.texture, bitmap.LockPixels(), bitmap.Size, 0, 0, 0, textureDescription.Width, textureDescription.Height, 1, 0, 0);
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
		}
		private void UpdateTexture(uint texture_id, ULBitmap bitmap)
		{
			TextureEntry entry = TextureEntries[(int)texture_id];

			graphicsDevice.UpdateTexture(entry.texture, bitmap.LockPixels(), bitmap.Size, 0, 0, 0, bitmap.Width, bitmap.Height, 1, 0, 0);
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

		}
	}
}
