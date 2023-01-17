#version 330

// Vertex Data Input
layout (location = 2)in vec3 RawPosition;
layout (location = 3)in vec2 RawTex0;

// Output
out vec2 Tex0;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	Tex0 = RawTex0;

	mat4 MVP = projection * view * model;
	gl_Position = MVP * vec4(RawPosition, 1);
}