#version 330 core

layout(location = 0) in vec3 aPosition;

out vec3 inputColor;

uniform vec3 aColor;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(void)
{
    inputColor = aColor;
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
}