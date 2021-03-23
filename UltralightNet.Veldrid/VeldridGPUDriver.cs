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

		private readonly List<TextureEntry> textureEntries;
		private uint textureid = 0;
		private readonly List<TextureEntry> geometryEntries;
		private uint geometryid = 0;
		private readonly List<TextureEntry> renderBufferEntries;
		private uint renderbufferid = 0;

		public VeldridGPUDriver(GraphicsDevice graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;

			textureEntries = new();
		}
		public ULGPUDriver GetGPUDriver() => new()
		{
			NextTextureId = NextTextureId,
			NextGeometryId = NextGeometryId,
			NextRenderBufferId = NextRenderBufferId,
			CreateTexture = CreateTexture,
			UpdateTexture = UpdateTexture
		};

		#region NextId
		private uint NextTextureId()
		{
			textureEntries.Insert((int)++textureid, new());
			return textureid;
		}
		public uint NextGeometryId()
		{
			geometryEntries.Insert((int)++geometryid, new());
			return geometryid;
		}
		public uint NextRenderBufferId()
		{
			renderBufferEntries.Insert((int)++renderbufferid, new());
			return renderbufferid;
		}
		#endregion NextId

		private void CreateTexture(uint texture_id, ULBitmap bitmap)
		{
			TextureEntry entry = textureEntries[(int)textureid];
			TextureDescription textureDescription = new()
			{
				Width = bitmap.Width,
				Height = bitmap.Height,
				MipLevels = MipLevels,
				SampleCount = SampleCount,
				ArrayLayers = 1
			};

			if (bitmap.IsEmpty)
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
			}
			if (GenerateMipMaps) textureDescription.Usage |= TextureUsage.GenerateMipmaps;

			entry.texture = graphicsDevice.ResourceFactory.CreateTexture(textureDescription);
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
		private void UpdateTexture(uint texture_id, ULBitmap bitmap)
		{
			TextureEntry entry = textureEntries[(int)textureid];

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
	}
}
