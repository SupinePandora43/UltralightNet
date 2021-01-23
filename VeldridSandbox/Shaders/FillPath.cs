using ShaderGen;
using System.Numerics;
using static ShaderGen.ShaderBuiltins;

[assembly: ShaderSet("FillPath", "VeldridSandbox.Shaders.FillPath.VertexShaderFunc", "VeldridSandbox.Shaders.FillPath.FragmentShaderFunc")]

namespace VeldridSandbox.Shaders
{
	class FillPath
	{
#pragma warning disable CS0649
		public Vector4 State;
		public Matrix4x4 Mat;
		public float GetWidth()
		{
			return State.Y;
		}
		public struct VertexInput
		{
			[PositionSemantic] public Vector2 Position;
			[TextureCoordinateSemantic] public Vector4 Color;
		}

		public struct FragmentInput
		{
			[SystemPositionSemantic] public Vector4 Position;
			[TextureCoordinateSemantic] public Vector4 Color;
		}

		[VertexShader]
		public FragmentInput VertexShaderFunc(VertexInput input)
		{
			FragmentInput output;
			output.Position = new Vector4(input.Position, GetWidth()+ Mat.M11, 1);
			output.Color = input.Color;
			return output;
		}

		[FragmentShader]
		public Vector4 FragmentShaderFunc(FragmentInput input)
		{
			return input.Color;
		}
		
	}
}
