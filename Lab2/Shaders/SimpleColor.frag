#version 330

out vec4 outputColor;
in vec3 inputColor;

void main()
{
    outputColor = vec4(inputColor,1);
}