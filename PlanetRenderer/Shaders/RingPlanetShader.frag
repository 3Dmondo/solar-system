#version 330
#define PI 3.1415926538

uniform sampler2D SurfaceTexture;
uniform sampler2D RingTexture;
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
    float r1 = 1.10774;
    float r2 = 2.98906;
    float dr = r2 - r1;

    vec2 C = (gl_PointCoord - vec2(0.5, 0.5)) * 2;
    float mag = dot(C,C);
    if (mag > 1.0) discard;   // kill pixels outside circle

    vec3 N = vec3(C, sqrt(1 - mag));

    N = rotate(N,vec3(1,0,0), -0.4101524);

    float lat = acos(N.y) / PI ;
    float lon = rotation.x + 0.5 * sign(N.z) * acos(-N.x/length(N.xz)) / PI;

    float lightTheta = acos(lightDir.y);
    float lightPhi = atan(lightDir.z,lightDir.x);
    float lx = tan(lightTheta);
    float ly = cos(lightPhi);
    float lz = sin(lightPhi);

    float signDir = 1;
    if (lightTheta > PI * 0.5)
      signDir = -1;

    float x1 = N.y * lx * ly;
    float y1 = N.y * lx * lz;

    float R = length(vec2(x1 + N.x, y1 + N.z));

    float diffuse = max(0.0, dot(lightDir, N));

    if( R >= r1 && R <= r2 && N.y * signDir > 0)
    {
      vec2 tex = vec2(0.5, (R - r1) / dr);
      vec4 ringColor = texture(RingTexture, tex);
      float ringShadow = (ringColor.r + ringColor.g + ringColor.b) / 3 ;
      outputColor = texture(SurfaceTexture, vec2(lon,lat)) * diffuse * max(0.2,(1 - ringShadow));
    }
    else
     outputColor = texture(SurfaceTexture, vec2(lon,lat)) * diffuse;

}

