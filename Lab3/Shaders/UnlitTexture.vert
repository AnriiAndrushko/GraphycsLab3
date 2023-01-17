#version 330

layout (location = 2)in vec3 RawPosition;
layout (location = 3)in vec2 RawTex0;
layout (location = 4) in vec3 RawNormal;

out vec2 Tex0;
out vec3 Normal;
out vec3 FragPos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	Tex0 = RawTex0;

	gl_Position = vec4(RawPosition, 1) * model*view*projection;

      FragPos = vec3(vec4(RawPosition, 1.0) * model);
      Normal = RawNormal * mat3(transpose(inverse(model)));
}