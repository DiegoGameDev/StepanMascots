#version 400 core

uniform sampler2D u_texture;


in vec2 texCoord;

out vec4 OutPutColor;

void main()
{
	vec4 texColor = texture(u_texture, texCoord);

	if (texColor.a < 0.1)
		discard;

	OutPutColor = texColor;
}