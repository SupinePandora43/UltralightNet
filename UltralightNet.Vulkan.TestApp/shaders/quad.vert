#version 450

layout(location = 0) in vec4 PositionUV;

layout(location = 0) out vec2 out_uv;

void main()
{
    gl_Position = vec4(PositionUV.x, PositionUV.y, 0, 1);
	out_uv = vec2(PositionUV.z, PositionUV.w);
}
