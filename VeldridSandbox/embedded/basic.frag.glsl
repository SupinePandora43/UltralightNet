#version 450
precision highp float;

layout(binding = 0) uniform sampler _sampler;
layout(binding = 1) uniform texture2D _texture;

layout(location = 0) in vec2 out_uv;

layout(location = 0) out vec4 out_Color;

void main()
{
	out_Color = texture(sampler2D(_texture, _sampler), out_uv);
}
