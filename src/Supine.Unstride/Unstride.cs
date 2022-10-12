using System;
using System.Runtime.CompilerServices;

namespace Supine.Unstride
{
	public static class Unstrider
	{
		public static unsafe void Unstride(byte* inputPixels, byte* outputPixels, uint inputRowLength, uint outputRowLength, uint times)
		{
			for (uint y = 0; y < times; y++)
			{
				new ReadOnlySpan<byte>(inputPixels + (y * (inputRowLength)), unchecked((int)outputRowLength)).CopyTo(new Span<byte>(outputPixels + (y * outputRowLength), unchecked((int)outputRowLength)));
			}
		}
		public static unsafe byte[] Unstride(byte* pixels, uint width, uint height, uint bpp, uint offset)
		{
			byte[] unstridedPixelByteArray = new byte[width * height * bpp];

			uint pixelOffset = 0;
			uint stridePixelOffset = 0;

			for (uint verticalLine = 0; verticalLine < height; verticalLine++)
			{
				for (uint horizontalLine = 0; horizontalLine < width; horizontalLine++)
				{
					for (uint b = 0; b < bpp; b++)
					{
						unstridedPixelByteArray[pixelOffset] = pixels[stridePixelOffset];

						pixelOffset++;
						stridePixelOffset++;
					}
				}
				stridePixelOffset += offset;
			}

			return unstridedPixelByteArray;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] Unstride(IntPtr pixels, uint width, uint height, uint bpp, uint offset)
		{
			unsafe
			{
				return Unstride((byte*)pixels, width, height, bpp, offset);
			}
		}
	}
}
