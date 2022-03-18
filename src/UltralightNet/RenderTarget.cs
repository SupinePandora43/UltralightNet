using System.Runtime.InteropServices;

namespace UltralightNet
{
	/// <summary>Rendering details for a View, to be used with your own GPUDriver</summary>
	[NativeMarshalling(typeof(RenderTargetNative))]
	public struct RenderTarget
	{
		/// <summary>Whether this target is empty (null texture)</summary>
		public bool is_empty;
		/// <summary>The viewport width (in device coordinates).</summary>
		public uint width;
		/// <summary>The viewport height (in device coordinates).</summary>
		public uint height;
		/// <summary>The GPUDriver-specific texture ID (you should bind the texture using your implementation of GPUDriver::BindTexture before drawing a quad).</summary>
		public uint texture_id;
		/// <summary>The texture width (in pixels). This may be padded.</summary>
		public uint texture_width;
		/// <summary>The texture height (in pixels). This may be padded.</summary>
		public uint texture_height;
		/// <summary>The pixel format of the texture.</summary>
		public ULBitmapFormat texture_format;
		/// <summary>UV coordinates of the texture (this is needed because the texture may be padded).</summary>
		public ULRect uv_coords;
		/// <summary>The GPUDriver-specific render buffer ID.</summary>
		public uint render_buffer_id;
	}

	[BlittableType]
	internal struct RenderTargetNative
	{
		public byte is_empty;
		public uint width;
		public uint height;
		public uint texture_id;
		public uint texture_width;
		public uint texture_height;
		public int texture_format;
		public ULRect uv_coords;
		public uint render_buffer_id;

		public RenderTargetNative(RenderTarget rt)
		{
			is_empty = rt.is_empty ? (byte)1 : (byte)0;
			width = rt.width;
			height = rt.height;
			texture_id = rt.texture_id;
			texture_width = rt.texture_width;
			texture_height = rt.texture_height;
			texture_format = (int)rt.texture_format;
			uv_coords = rt.uv_coords;
			render_buffer_id = rt.render_buffer_id;
		}

		public RenderTarget ToManaged() => new()
		{
			is_empty = is_empty != 0,
			width = width,
			height = height,
			texture_id = texture_id,
			texture_height = texture_height,
			texture_width = texture_width,
			texture_format = (ULBitmapFormat)texture_format,
			uv_coords = uv_coords,
			render_buffer_id = render_buffer_id
		};
	}
}
