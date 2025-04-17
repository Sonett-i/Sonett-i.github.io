#version 330 core

struct Material
{
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
};

struct Light
{
    vec3 position;
    vec4 colour;
    vec3 ambient;

	float a;
	float b;
};

uniform Material material;
uniform Light light;

out vec4 FragColor;

in vec2 uv;
in vec3 Normal;
in vec3 FragPos;

uniform vec3 camPos;

uniform float nearClip;
uniform float farClip;


vec4 pointLight()
{   
    // Vector from fragment to light
    vec3 lightVec = light.position - FragPos;

    // Light intensity over distance
    float dist = length(lightVec);

	float a = 1.0f;
	float b = 0.04f;

    float inten = 1.0f / (a * dist * dist + b * dist + 1.0f);

    // Ambient lighting
    float ambient = light.ambient.r;

    // Diffuse lighting
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightVec);
    float diffuse = max(dot(norm, lightDir), 0.0f);

	// Specular lighting
	float specularLight = 0.5f;
    
    vec3 viewDir = normalize(camPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float specAmount = pow(max(dot(viewDir, reflectDir), 0.0f), 16);
    float specular = specAmount * specularLight;

    // Combine lighting components
	return (texture(material.diffuse, uv) * (diffuse * inten + ambient) + texture(material.specular, uv).r * specular * inten) * light.colour;
}

vec4 directLight()
{
    // Ambient lighting
    float ambient = 0.2f;

    // Diffuse lighting
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(light.position);
    float diffuse = max(dot(norm, lightDir), 0.0f);

	// Specular lighting
	float specularLight = 0.5f;
    
    vec3 viewDir = normalize(camPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float specAmount = pow(max(dot(viewDir, reflectDir), 0.0f), 16); // dot product between camera position and the fragment position raised to power of 16 for specular amount
    float specular = specAmount * specularLight;

    // Combine lighting components
	return (texture(material.diffuse, uv) * (diffuse + ambient) + texture(material.specular, uv).r * specular) * light.colour;
}

vec4 spotLight()
{
    float outerCone = 0.90f;
    float innerCone = 0.95f;

    vec3 lightVec = light.position - FragPos;

    float ambient = 0.2f;

    // Diffuse lighting
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightVec);
    float diffuse = max(dot(norm, lightDir), 0.0f);

	// Specular lighting
	float specularLight = 0.5f;
    
    vec3 viewDir = normalize(camPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float specAmount = pow(max(dot(viewDir, reflectDir), 0.0f), 16);
    float specular = specAmount * specularLight;

    float angle = dot(vec3(0.0f, -1.0f, 0.0f), -lightDir);

    float inten = clamp((angle - outerCone) / (innerCone - outerCone), 0.0f, 1.0f);


    // Combine lighting components
	return (texture(material.diffuse, uv) * (diffuse * inten + ambient) + texture(material.specular, uv) * specular * inten) * light.colour;    
}


float linearDepth(float depth)
{
    return (2.0 * nearClip * farClip) / (farClip + nearClip - (depth * 2.0 - 1.0) * (farClip - nearClip));
}

float logisticDepth(float depth, float steepness = 0.015f, float offset = 500.0f)
{
	float zVal = linearDepth(depth);
	return (1 / (1 + exp(-steepness * (zVal - offset))));
}

void main()
{
    float depth = logisticDepth(gl_FragCoord.z);
    FragColor = directLight() * (1.0f - depth) + vec4(depth * vec3(0.85f, 0.85f, 0.90f) * 0.5f, 1.0f);
}
