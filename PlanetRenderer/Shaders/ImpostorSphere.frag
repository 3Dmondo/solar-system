#version 330

in vec2 mapping;

out vec4 outputColor;

const float sphereRadius = 0.5;
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
	
	outputColor = vec4(cameraPos,1); //2.0 gamma correction
}