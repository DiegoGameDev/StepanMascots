#version 400 core

layout(location = 0) in vec3 aPos;
layout(location = 1) in vec2 aTexCoord;

layout(location = 2) in vec4 instanceUV;
layout(location = 3) in vec4 instanceColor;
layout(location = 4) in vec4 line1;
layout(location = 5) in vec4 line2;
layout(location = 6) in vec4 line3;
layout(location = 7) in vec4 line4;

out vec2 texCoord;
out vec4 fragColor;

uniform mat4 VP;

void main()
{
	mat4 model = mat4(line1, line2, line3, line4);
	
	gl_Position = VP * model * vec4(aPos, 1.0);
	texCoord = instanceUV.xy + aTexCoord * (instanceUV.zw - instanceUV.xy);
	fragColor = instanceColor;
}