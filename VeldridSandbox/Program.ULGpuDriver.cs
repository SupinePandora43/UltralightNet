using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Supine.UltralightSharp.Enums;
using Supine.UltralightSharp.Safe;
using System;
using System.Collections.Generic;
using System.IO;
using Veldrid;
using Bitmap = Supine.UltralightSharp.Safe.Bitmap;
using PixelFormat = Veldrid.PixelFormat;

namespace VeldridSandbox
{
	public partial class Program
	{
		private readonly Queue<Command> queuedCommands = new();

		public class GeometryEntry
		{
			public DeviceBuffer VertexBuffer { get; set; }
			public DeviceBuffer IndiciesBuffer { get; set; }
		}
		public class RenderBufferEntry
		{
			public Framebuffer FrameBuffer { get; set; }
			public TextureEntry TextureEntry { get; set; } = null!;
		}

		public class TextureEntry
		{
			public Texture Texure { get; set; }
			public TextureView TextureView { get; set; }
			public uint Width { get; set; }
			public uint Height { get; set; }
		}

		private static readonly SlottingList<GeometryEntry> GeometryEntries = new(32, 8);
		private static readonly SlottingList<TextureEntry> TextureEntries = new(32, 8);
		private static readonly SlottingList<RenderBufferEntry> RenderBufferEntries = new(8, 2);


		private void UpdateCommandList(Supine.UltralightSharp.Safe.CommandList list)
		{
			//Console.WriteLine("GpuDriver.UpdateCommandList(LIST)");
			foreach (var cmd in list)
				queuedCommands.Enqueue(cmd);
		}

		private void DestroyTexture(uint textureId)
		{
			Console.WriteLine($"GpuDriver.DestroyTexture({textureId})");
			var index = (int)textureId - 1;
			var entry = TextureEntries.RemoveAt(index);
			entry.TextureView.Dispose();
			entry.TextureView = null;
			entry.Texure.Dispose();
			entry.Texure = null;
		}

		private void UpdateTexture(uint textureId, Bitmap bitmap)
		{
			Console.WriteLine($"GpuDriver.UpdateTexture: {textureId}");
			var index = (int)textureId - 1;
			var entry = TextureEntries[index];

			var tex = entry.Texure;
			var texWidth = entry.Width;
			var texHeight = entry.Height;

			//var pixels = bitmap.LockPixels();

			var format = bitmap.GetFormat();

			IntPtr pixels = bitmap.LockPixels();

			switch (format)
			{
				case BitmapFormat.A8UNorm:
					{
						graphicsDevice.UpdateTexture(tex, pixels,
							bitmap.GetSize().ToUInt32(),
							0,
							0,
							0,
							texWidth,
							texHeight,
							1,
							0,
							0
						);
						unsafe
						{
							ReadOnlySpan<byte> span = new(pixels.ToPointer(), (int)(texWidth * texHeight));
							Image image = Image.LoadPixelData<L8>(span, (int)texWidth, (int)texHeight);
							FileStream fs = File.Open($"./texture_{textureId}_updated_{Guid.NewGuid()}.png", FileMode.OpenOrCreate);
							image.SaveAsPng(fs);
							fs.Close();
						}
						break;
					}
				case BitmapFormat.Bgra8UNormSrgb:
					{
						graphicsDevice.UpdateTexture(tex, pixels,
							bitmap.GetSize().ToUInt32(),
							0,
							0,
							0,
							texWidth,
							texHeight,
							1,
							0,
							0
						);
						unsafe
						{
							ReadOnlySpan<byte> span = new(pixels.ToPointer(), (int)(texWidth * texHeight * 4));
							Image image = Image.LoadPixelData<Bgra32>(span, (int)texWidth, (int)texHeight);
							FileStream fs = File.Open($"./texture_{textureId}_updated_{Guid.NewGuid()}.png", FileMode.OpenOrCreate);
							image.SaveAsPng(fs);
							fs.Close();
						}
						break;
					}
				default: throw new ArgumentOutOfRangeException(nameof(BitmapFormat));
			}

			bitmap.UnlockPixels();
			#region _gl.GenerateMipmap
			Veldrid.CommandList cl = factory.CreateCommandList();
			cl.Begin();
			cl.GenerateMipmaps(tex);
			cl.End();
			graphicsDevice.SubmitCommands(cl);
			cl.Dispose();
			cl = null;
			#endregion
		}

		private void CreateTexture(uint textureId, Bitmap bitmap)
		{
			var index = (int)textureId - 1;
			var entry = TextureEntries[index];
			var texWidth = bitmap.GetWidth();
			entry.Width = texWidth;
			var texHeight = bitmap.GetHeight();
			entry.Height = texHeight;
			Console.WriteLine($"GpuDriver.CreateTexture({textureId} {texWidth}x{texHeight})");
			var pixels = bitmap.LockPixels();

			if (bitmap.IsEmpty())
			{
				Console.WriteLine("Bitmap.IsEmtpy()==true");
				entry.Texure = factory.CreateTexture(
					TextureDescription.Texture2D(texWidth,
						texHeight,
						1,
						1,
						PixelFormat.R8_G8_B8_A8_UNorm_SRgb,
						TextureUsage.Sampled | TextureUsage.RenderTarget
					)
				);
				entry.TextureView = factory.CreateTextureView(entry.Texure);
			}
			else
			{
				Console.WriteLine("Bitmap.IsEmtpy()==false");
				switch (bitmap.GetFormat())
				{
					case BitmapFormat.A8UNorm:
						entry.Texure = factory.CreateTexture(
							TextureDescription.Texture2D(
								texWidth,
								texHeight,
								1,
								1,
								PixelFormat.R8_UNorm,
								TextureUsage.Sampled | TextureUsage.GenerateMipmaps
							)
						);
						unsafe
						{
							ReadOnlySpan<byte> span = new(pixels.ToPointer(), (int)(texWidth * texHeight));
							Image image = Image.LoadPixelData<L8>(span, (int)texWidth, (int)texHeight);
							FileStream fs = File.Open($"./texture_{textureId}_{Guid.NewGuid()}.png", FileMode.OpenOrCreate);
							image.SaveAsPng(fs);
							fs.Close();
						}
						break;
					case BitmapFormat.Bgra8UNormSrgb:
						entry.Texure = factory.CreateTexture(
							TextureDescription.Texture2D(
								texWidth,
								texHeight,
								1,
								1,
								PixelFormat.B8_G8_R8_A8_UNorm_SRgb,
								TextureUsage.Sampled | TextureUsage.GenerateMipmaps
							)
						);
						unsafe
						{
							ReadOnlySpan<byte> span = new(pixels.ToPointer(), (int)(texWidth * texHeight * 4));
							Image image = Image.LoadPixelData<Bgra32>(span, (int)texWidth, (int)texHeight);
							FileStream fs = File.Open($"./texture_{textureId}_{Guid.NewGuid()}.png", FileMode.OpenOrCreate);
							image.SaveAsPng(fs);
							fs.Close();
						}
						break;
				}
				entry.TextureView = factory.CreateTextureView(entry.Texure);
				graphicsDevice.UpdateTexture(entry.Texure,
					pixels,
					bitmap.GetSize().ToUInt32(),
					0,
					0,
					0,
					texWidth,
					texHeight,
					1,
					0,
					0);
				var cl = factory.CreateCommandList();
				cl.Begin();
				cl.GenerateMipmaps(entry.Texure);
				cl.End();
				graphicsDevice.SubmitCommands(cl);
				graphicsDevice.WaitForIdle();
				cl.Dispose();
				cl = null;
			}
			bitmap.UnlockPixels();
		}

		private uint NextTextureId()
		{
			var id = TextureEntries.Add(new TextureEntry()) + 1;
			Console.WriteLine($"GpuDriver.NextTextureId() = {id}");
			return (uint)id;
		}

		private void DestroyRenderBuffer(uint renderBufferId)
		{
			Console.WriteLine($"GpuDriver.DestroyRenderBuffer({renderBufferId})");
			var index = (int)renderBufferId - 1;
			var entry = RenderBufferEntries.RemoveAt(index);

			entry.FrameBuffer.Dispose();
			entry.FrameBuffer = null;
		}

		private void CreateRenderBuffer(uint renderBufferId, RenderBuffer buffer)
		{
			Console.WriteLine($"CreateRenderBuffer: {renderBufferId}");

			var index = (int)renderBufferId - 1;
			var entry = RenderBufferEntries[index];

			var texIndex = (int)buffer.TextureId - 1;
			var texEntry = TextureEntries[texIndex];
			var tex = texEntry.Texure;

			FramebufferDescription fd = new();
			fd.ColorTargets = new[] { new FramebufferAttachmentDescription(tex, 0) };

			entry.FrameBuffer = factory.CreateFramebuffer(fd);

			Veldrid.CommandList clearBufferCommandList = factory.CreateCommandList();
			clearBufferCommandList.Begin();
			clearBufferCommandList.SetFramebuffer(entry.FrameBuffer);
			clearBufferCommandList.ClearColorTarget(0, RgbaFloat.Clear);
			clearBufferCommandList.End();
			graphicsDevice.SubmitCommands(clearBufferCommandList);
			clearBufferCommandList.Dispose();
			clearBufferCommandList = null;

			//entry.FrameBuffer.ColorTargets.Append(new(tex, 0));
			entry.TextureEntry = texEntry;
		}

		private uint NextRenderBufferId()
		{
			var id = RenderBufferEntries.Add(new RenderBufferEntry()) + 1;
			Console.WriteLine($"GpuDriver.NextRenderBufferId() = {id}");
			return (uint)id;
		}

		private void DestroyGeometry(uint geometryId)
		{
			Console.WriteLine($"GpuDriver.DestroyGeometry({geometryId})");
			var index = (int)geometryId - 1;
			var entry = GeometryEntries.RemoveAt(index);

			entry.IndiciesBuffer.Dispose();
			entry.IndiciesBuffer = null;
			entry.VertexBuffer.Dispose();
			entry.VertexBuffer = null;
		}

		private unsafe void UpdateGeometry(uint geometryId, VertexBuffer safeVertices, IndexBuffer indices)
		{
			//Console.WriteLine($"GpuDriver.UpdateGeometry({geometryId})");
			var index = (int)geometryId - 1;
			var entry = GeometryEntries[index];

			graphicsDevice.UpdateBuffer(entry.VertexBuffer, 0, (IntPtr)safeVertices._Data, safeVertices.Size);
			graphicsDevice.UpdateBuffer(entry.IndiciesBuffer, 0, (IntPtr)indices._Data, indices.Size);

		}

		private unsafe void CreateGeometry(uint geometryId, VertexBuffer safeVertices, IndexBuffer indicies)
		{
			Console.WriteLine($"GpuDriver.CreateGeometry: {geometryId}");
			var index = (int)geometryId - 1;
			var entry = GeometryEntries[index];


			switch (safeVertices.Format)
			{
				case VertexBufferFormat._2F4Ub2F2F28F:
					{
						entry.VertexBuffer = factory.CreateBuffer(
							new BufferDescription(
								safeVertices.Size,
								BufferUsage.VertexBuffer
							)
						);
						graphicsDevice.UpdateBuffer(
							entry.VertexBuffer,
							0,
							(IntPtr)safeVertices._Data,
							safeVertices.Size);

						break;
					}
				case VertexBufferFormat._2F4Ub2F:
					{
						entry.VertexBuffer = factory.CreateBuffer(
							new BufferDescription(
								safeVertices.Size,
								BufferUsage.VertexBuffer
							)
						);
						graphicsDevice.UpdateBuffer(
							entry.VertexBuffer,
							0,
							(IntPtr)safeVertices._Data,
							safeVertices.Size);
						break;
					}
				default: throw new NotImplementedException(safeVertices.Format.ToString());
			}
			entry.IndiciesBuffer = factory.CreateBuffer(new(
				indicies.Size,
				BufferUsage.IndexBuffer));
			graphicsDevice.UpdateBuffer(entry.IndiciesBuffer, 0, (IntPtr)indicies._Data, indicies.Size);
		}

		private uint NextGeometryId()
		{
			var id = GeometryEntries.Add(new GeometryEntry()) + 1;
			Console.WriteLine($"GpuDriver.NextGeometryId() = {id}");
			return (uint)id;
		}

		private void EndSynchronize() { }
		private void BeginSynchronize() { }
	}
}
