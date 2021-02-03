#version 450
precision highp float;
layout(binding = 3) uniform sampler2D iTex;

layout(location = 0) in vec2 fUv;
layout(location = 1) in vec4 fColor;

layout(location = 0) out vec4 oColor;

void main()
{
    oColor = texture(iTex, fUv);
}
