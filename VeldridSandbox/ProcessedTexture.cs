/* SOURCE: https://github.com/mellinoe/veldrid-samples/blob/0ac284ddc8f62d8d75c8eae88cc1824c320b1caf/src/AssetPrimitives/ProcessedTexture.cs*/

using System;
using Veldrid;

namespace VeldridSandbox
{
	public class ProcessedTexture
	{
		public PixelFormat Format { get; set; }
		public TextureType Type { get; set; }
		public uint Width { get; set; }
		public uint Height { get; set; }
		public uint Depth { get; set; }
		public uint MipLevels { get; set; }
		public uint ArrayLayers { get; set; }
		public byte[] TextureData { get; set; }

		public ProcessedTexture(
			PixelFormat format,
			TextureType type,
			uint width,
			uint height,
			uint depth,
			uint mipLevels,
			uint arrayLayers,
			byte[] textureData)
		{
			Format = format;
			Type = type;
			Width = width;
			Height = height;
			Depth = depth;
			MipLevels = mipLevels;
			ArrayLayers = arrayLayers;
			TextureData = textureData;
		}

		public unsafe Texture CreateDeviceTexture(GraphicsDevice gd, ResourceFactory rf, TextureUsage usage)
		{
			Texture texture = rf.CreateTexture(new TextureDescription(
				Width, Height, Depth, MipLevels, ArrayLayers, Format, usage, Type));

			Texture staging = rf.CreateTexture(new TextureDescription(
				Width, Height, Depth, MipLevels, ArrayLayers, Format, TextureUsage.Staging, Type));

			ulong offset = 0;
			fixed (byte* texDataPtr = &TextureData[0])
			{
				for (uint level = 0; level < MipLevels; level++)
				{
					uint mipWidth = GetDimension(Width, level);
					uint mipHeight = GetDimension(Height, level);
					uint mipDepth = GetDimension(Depth, level);
					uint subresourceSize = mipWidth * mipHeight * mipDepth * GetFormatSize(Format);

					for (uint layer = 0; layer < ArrayLayers; layer++)
					{
						gd.UpdateTexture(
							staging, (IntPtr)(texDataPtr + offset), subresourceSize,
							0, 0, 0, mipWidth, mipHeight, mipDepth,
							level, layer);
						offset += subresourceSize;
					}
				}
			}

			CommandList cl = rf.CreateCommandList();
			cl.Begin();
			cl.CopyTexture(staging, texture);
			cl.End();
			gd.SubmitCommands(cl);

			return texture;
		}

		private uint GetFormatSize(PixelFormat format)
		{
			switch (format)
			{
				case PixelFormat.R8_G8_B8_A8_UNorm: return 4;
				case PixelFormat.BC3_UNorm: return 1;
				default: throw new NotImplementedException();
			}
		}

		public static uint GetDimension(uint largestLevelDimension, uint mipLevel)
		{
			uint ret = largestLevelDimension;
			for (uint i = 0; i < mipLevel; i++)
			{
				ret /= 2;
			}

			return Math.Max(1, ret);
		}
	}
}
