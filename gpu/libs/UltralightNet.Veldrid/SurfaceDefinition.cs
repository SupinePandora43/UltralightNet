using System.Runtime.InteropServices;
using UltralightNet.GPUCommon;
using UltralightNet.Platform;
using Veldrid;

namespace UltralightNet.GPU.Veldrid;

public class SurfaceDefinition : ISurfaceDefinition
{
	readonly ResourceList<SurfaceEntry> surfaces = new();
	readonly GraphicsDevice graphicsDevice;

	public CommandList CommandList { private get; set; } = null;

	public SurfaceDefinition(GraphicsDevice graphicsDevice)
	{
		this.graphicsDevice = graphicsDevice;
	}

	SurfaceEntry CreateSurface(uint width, uint height)
	{
		var textureCreateInfo = new TextureDescription(width, height, 1, 1, 1, PixelFormat.B8_G8_R8_A8_UNorm_SRgb, TextureUsage.Sampled, TextureType.Texture2D);
		var deviceLocalTexture = graphicsDevice.ResourceFactory.CreateTexture(ref textureCreateInfo);
		textureCreateInfo.Usage = TextureUsage.Staging;
		return new(deviceLocalTexture, graphicsDevice.ResourceFactory.CreateTexture(textureCreateInfo), width * height * 4, width, height);
	}
	nint ISurfaceDefinition.Create(uint width, uint height)
	{
		var id = surfaces.GetNewId();
		surfaces[id] = CreateSurface(width, height);
		return id;
	}
	void ISurfaceDefinition.Destroy(nint id)
	{
		surfaces[(int)id].Texture.Dispose();
		surfaces[(int)id].StagingTexture.Dispose();
	}

	uint ISurfaceDefinition.GetWidth(nint id) => surfaces[(int)id].Width;
	uint ISurfaceDefinition.GetHeight(nint id) => surfaces[(int)id].Height;
	uint ISurfaceDefinition.GetRowBytes(nint id) => surfaces[(int)id].Width * 4;
	nuint ISurfaceDefinition.GetSize(nint id) => surfaces[(int)id].Size;

	unsafe byte* ISurfaceDefinition.LockPixels(nint id)
	{
		ref var surface = ref surfaces[(int)id];
		return (byte*)graphicsDevice.Map(surfaces[(int)id].StagingTexture, MapMode.ReadWrite).Data;
	}
	void ISurfaceDefinition.UnlockPixels(nint id)
	{
		ref var surface = ref surfaces[(int)id];
		graphicsDevice.Unmap(surface.StagingTexture);

		CommandList.CopyTexture(surface.StagingTexture, surface.Texture);
	}

	void ISurfaceDefinition.Resize(nint id, uint width, uint height)
	{
		surfaces[(int)id] = CreateSurface(width, height);
	}

	[StructLayout(LayoutKind.Auto)]
	readonly record struct SurfaceEntry(Texture Texture, Texture StagingTexture, nuint Size, uint Width, uint Height);
}
