using System.IO;
using System.Reflection;
using System.Text;
using Veldrid;
using Veldrid.SPIRV;

namespace UltralightNet.Veldrid.Shaders
{
	public static class SpirvCross
	{
		private static byte[] GetShaderBytes(string name)
		{
			Assembly assembly = typeof(SpirvCross).Assembly;
			Stream stream = assembly.GetManifestResourceStream("UltralightNet.Veldrid.shaders.spirv_cross." + name);
			StreamReader resourceStreamReader = new(stream, Encoding.UTF8, false, 16, true);
			string shaderCode = resourceStreamReader.ReadToEnd();
			return Encoding.UTF8.GetBytes(shaderCode);
		}
		public static Shader[] ul(GraphicsDevice graphicsDevice)
		{
			ShaderDescription vertexShaderShaderDescription = new(ShaderStages.Vertex, GetShaderBytes("shader_fill.vert.glsl"), "main");
			ShaderDescription fragmentShaderShaderDescription = new(ShaderStages.Fragment, GetShaderBytes("shader_fill.frag.glsl"), "main");
			return graphicsDevice.ResourceFactory.CreateFromSpirv(vertexShaderShaderDescription, fragmentShaderShaderDescription);
		}
	}
}
