#version 330 core

layout (location=0) in vec3 aPosition;
layout (location=1) in vec3 aNormal;
layout (location=2) in vec3 aTangent;
layout (location=3) in vec2 aTexCoord;

uniform float scale;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform vec4 colour;

out vec3 Normal;
out vec2 uv;
out vec3 FragPos;

void main()
{
	FragPos = vec3(model * vec4(aPosition, 1.0f));
	gl_Position = projection * view * model * vec4(aPosition, 1);

	uv = aTexCoord;
	Normal = normalize(mat3(model) * aNormal);
}