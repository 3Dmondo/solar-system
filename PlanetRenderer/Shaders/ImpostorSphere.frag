#version 330
#define PI 3.1415926538

uniform sampler2D SurfaceTexture;

uniform	mat4 cameraToClipMatrix;
uniform mat4 view;
uniform mat3 orientation;
uniform vec3 lightPos;
uniform float rotation;

in vec2 mapping;

out vec4 outputColor;

const float sphereRadius = 1.0;
uniform vec3 cameraSpherePos;

void Impostor(out vec3 cameraPos, out vec3 cameraNormal)
{
	vec3 cameraPlanePos = vec3(mapping * sphereRadius, 0.0) + cameraSpherePos;
	vec3 rayDirection = normalize(cameraPlanePos);
	
	float B = 2.0 * dot(rayDirection, -cameraSpherePos);
	float C = dot(cameraSpherePos, cameraSpherePos) - (sphereRadius * sphereRadius);
	
	float det = (B * B) - (4 * C);
	if(det < 0.0)
		discard;
		
	float sqrtDet = sqrt(det);
	float posT = (-B + sqrtDet)/2;
	float negT = (-B - sqrtDet)/2;
	
	float intersectT = min(posT, negT);
	cameraPos = rayDirection * intersectT;
	cameraNormal = normalize(cameraPos - cameraSpherePos);
}

void main()
{
	vec3 cameraPos;
	vec3 cameraNormal;
	
	Impostor(cameraPos, cameraNormal);

  cameraNormal = mat3(view) * cameraNormal;
  cameraNormal = orientation * cameraNormal;

  vec3 lightDir = normalize(lightPos -  cameraSpherePos );
  float diffuse = max(0.2,dot(cameraNormal,lightDir));

  vec2 uv = vec2(
    rotation + atan(cameraNormal.z, cameraNormal.x) / (PI * 2.0),
    acos(-cameraNormal.y) / PI);
	
	outputColor = texture(SurfaceTexture, uv) * diffuse; //2.0 gamma correction
}