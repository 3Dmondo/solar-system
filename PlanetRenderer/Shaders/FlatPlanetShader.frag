#version 330
#define PI 3.1415926538

uniform sampler2D texture0;

uniform vec3 rotation;

in vec3 lightDir;


out vec4 outputColor;

mat4 rotationMatrix(vec3 axis, float angle) {
    axis = normalize(axis);
    float s = sin(angle);
    float c = cos(angle);
    float oc = 1.0 - c;
    
    return mat4(oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,  0.0,
                oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,  0.0,
                oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c,           0.0,
                0.0,                                0.0,                                0.0,                                1.0);
}

vec3 rotate(vec3 v, vec3 axis, float angle) {
	mat4 m = rotationMatrix(axis, angle);
	return (m * vec4(v, 1.0)).xyz;
}


void main()
{

    vec2 C = (gl_PointCoord - vec2(0.5, 0.5)) * 2;
    float mag = dot(C,C);
    if (mag > 1.0) discard;   // kill pixels outside circle


    vec3 N = vec3(C, sqrt(1 - mag));
    N = rotate(N,vec3(1,0,0), -0.4101524);
    float lat = acos(N.y) / PI ;
    float lon = rotation.x + 0.5 * sign(N.z) * acos(-N.x/length(N.xz)) / PI;
    float diffuse = max(0.0, dot(lightDir, N));
    outputColor = texture(texture0, vec2(lon,lat)) * diffuse;

}

