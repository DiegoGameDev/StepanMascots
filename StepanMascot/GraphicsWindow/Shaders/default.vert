#version 400 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec2 aTexCoord;

uniform mat4 model;
uniform vec4 instanceUV;


out vec2 texCoord;

void main()
{
//uv = instanceUV.xy + aUV * (instanceUV.zw - instanceUV.xy);
	
	gl_Position = vec4(aPos, 1.0);
	texCoord = instanceUV.xy + aTexCoord * (instanceUV.zw - instanceUV.xy);
}