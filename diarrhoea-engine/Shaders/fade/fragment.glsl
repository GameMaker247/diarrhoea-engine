#version 330

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform sampler2D texture1;

uniform float fade;

void main()
{
    outputColor = mix(texture(texture0, texCoord), texture(texture1, texCoord), fade);
}