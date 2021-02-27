#version 450
precision highp float;

layout(location = 0) in vec2 vPos;
layout(location = 1) in vec2 fUV;

layout(location = 0) out vec2 fUv;

void main()
{
    //gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
    gl_Position = vec4(vPos, 0, 1);
	//fUv = 0.5 * vPos.xy + vec2(0.5,0.5);
	fUv = fUV;
}
