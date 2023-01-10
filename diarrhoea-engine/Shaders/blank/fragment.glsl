#version 330

out vec4 outputColor;
in vec2 texCoord;

void main()
{
    outputColor = texture(texture0, texCoord)*vec4(1.0, 0.0, 0.0, 1.0);
}