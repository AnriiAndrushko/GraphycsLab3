#version 330

uniform vec3 objectColor;
uniform vec3 lightColor; 
uniform vec3 lightPos;
uniform vec3 viewPos; 

in vec3 Normal; 
in vec3 FragPos;
in vec2 Tex0;

out vec4 PixelColor;

uniform sampler2D Texture;

void main()
{
   
    float ambientStrength = 0.1;//ambient
    vec3 ambient = ambientStrength * lightColor;

    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);//order is important!

   
    float diff = max(dot(norm, lightDir), 0.0); //cos must be positive
    vec3 diffuse = diff * lightColor;

    float specularStrength = 0.5;
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = specularStrength * spec * lightColor;//pure magic xd
    vec3 result = (ambient + diffuse + specular) * objectColor;

    PixelColor = texture(Texture, Tex0)*vec4(result, 1.0);
}