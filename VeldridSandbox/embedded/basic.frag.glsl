#version 450
precision highp float;

layout(binding = 0) uniform sampler SurfaceSampler;
layout(binding = 1) uniform texture2D SurfaceTexture;

layout(location = 0) in vec2 fUv;
//layout(location = 1) in vec4 fColor;

layout(location = 0) out vec4 oColor;

void main()
{
    oColor = texture(sampler2D(SurfaceTexture, SurfaceSampler), fUv);
}
