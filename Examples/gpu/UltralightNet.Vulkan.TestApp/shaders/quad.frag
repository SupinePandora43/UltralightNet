#version 450
precision highp float;

layout(set=0, binding = 0) uniform sampler2D image;

layout(location = 0) in vec2 out_uv;

layout(location = 0) out vec4 out_Color;

// https://gamedev.stackexchange.com/a/148088
vec4 toLinear(vec4 sRGB)
{
    bvec4 cutoff = lessThan(sRGB, vec4(0.04045));
    vec4 higher = pow((sRGB + vec4(0.055))/vec4(1.055), vec4(2.4));
    vec4 lower = sRGB/vec4(12.92);

    return mix(higher, lower, cutoff);
}

void main()
{
	// out_Color = vec4(out_uv.x, out_uv.y, 1, 1);
	// out_Color = toLinear(texture(image, out_uv));
	out_Color = texelFetch(image, ivec2(gl_FragCoord.x, gl_FragCoord.y), 0);
}
