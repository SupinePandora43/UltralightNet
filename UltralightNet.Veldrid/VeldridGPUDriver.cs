using System;
using Veldrid;

namespace UltralightNet.Veldrid
{
    public class VeldridGPUDriver
    {
		private readonly GraphicsDevice graphicsDevice;

		public bool GenerateMipMaps = true;

		public VeldridGPUDriver(GraphicsDevice graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;
		}
    }
}
