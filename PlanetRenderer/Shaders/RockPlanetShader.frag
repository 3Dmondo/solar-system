#version 330
#define PI 3.1415926538

uniform sampler2D SurfaceTexture;
uniform sampler2D NormalTexture;

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

vec3 tangent(vec3 normal){
  float theta = acos(normal.z);
  float phi = atan(normal.y,normal.x);
  theta += PI * 0.5;
  return vec3(
    sin(theta) * cos(phi),
    sin(theta) * sin(phi),
    cos(theta)
    );
}

void main()
{

    vec2 C = (gl_PointCoord - vec2(0.5, 0.5)) * 2;
    float mag = dot(C,C);
    if (mag > 1.0) discard;   // kill pixels outside circle
        vec3 N = vec3(C, sqrt(1 - mag));

    vec3 T = tangent(N);
    vec3 B = normalize(cross(T,N));
    T = normalize(cross(N,B));
    mat3 TBN = mat3(T, B, normalize(N));

    N = rotate(N,vec3(1,0,0), -0.4101524);
    float lat = acos(N.y) / PI ;
    float lon = rotation.x + 0.5 * sign(N.z) * acos(-N.x/length(N.xz)) / PI;

    vec3 normal = texture(NormalTexture, vec2(lon,lat)).xyz;

    normal = vec3(normal.x,1-normal.y,normal.z);
    normal = normal * 2.0 - 1.0;   
    normal = normalize(TBN * normal); 

    float diffuse = max(0.0, dot(lightDir, normal));

    outputColor = texture(SurfaceTexture, vec2(lon,lat)) * diffuse;

}

