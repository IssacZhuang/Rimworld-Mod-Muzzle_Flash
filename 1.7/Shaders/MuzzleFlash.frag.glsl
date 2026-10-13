#version 450

#ifdef INSTANCED
// Waylith extracts material bindings from the fragment shader, so buffers used by the
// vertex stage must be declared here as well.
#include "InstanceTransforms.glsl"

layout(location = 1) flat in int vInstanceIndex;

// Mirrors the C# MuzzleInstance struct, stride 32 bytes.
// A vec3 member is 16-byte aligned in std430, so a real vec3 _padding would inflate the
// stride to 48; the padding is therefore expressed as 3 floats (C# side: float frame +
// Vector3 padding, equivalent layout).
struct MuzzleInstance
{
    vec4 color;
    float frame;
    float _padding0;
    float _padding1;
    float _padding2;
};

layout(std430, set = 2, binding = 6) readonly buffer _MuzzleInstances
{
    MuzzleInstance muzzleInstances[];
};
#endif

layout(location = 0) in vec2 vUv;
layout(location = 0) out vec4 outColor;

layout(set = 2, binding = 0) uniform MaterialUniforms
{
    vec4 _Color;           // @color @default = vec4(1.0, 1.0, 1.0, 1.0)
    vec4 _MainTex_ST;      // @default = vec4(0.0, 0.0, 1.0, 1.0)
    float _LightIntensity; // @default = 1.0
    vec4 _Splits;          // @default = vec4(5.0, 5.0, 3.0, 5.0)  x=columns y=rows
} uMaterial;

layout(set = 2, binding = 1) uniform sampler2D _MainTex;

#ifdef INSTANCED
float InstanceFrame() { return muzzleInstances[vInstanceIndex].frame; }
vec4 InstanceTint()   { return muzzleInstances[vInstanceIndex].color; }
#else
float InstanceFrame() { return 0.0; }
vec4 InstanceTint()   { return uMaterial._Color; }
#endif

// Sprite atlas sampling: stage is the frame index
vec4 shot(vec2 uv, float dx, float dy, int stage)
{
    return texture(_MainTex, vec2(
        uv.x * dx + float(stage % max(int(uMaterial._Splits.x), 1)) * dx,
        1.0 - (uv.y * dy + float(stage / max(int(uMaterial._Splits.x), 1)) * dy)));
}

void main()
{
    int cols = max(int(uMaterial._Splits.x), 1);
    int rows = max(int(uMaterial._Splits.y), 1);
    int stages = cols * rows;
    float dx = 1.0 / float(cols);
    float dy = 1.0 / float(rows);

    // frame is a fractional frame number; blend neighbouring frames for smooth playback
    float stage = mod(InstanceFrame(), float(stages));
    int current = int(floor(stage));
    int next = int(mod(stage + 1.0, float(stages)));

    vec2 uv = vUv * uMaterial._MainTex_ST.zw + uMaterial._MainTex_ST.xy;
    vec4 frameColor = mix(shot(uv, dx, dy, current), shot(uv, dx, dy, next), stage - float(current));

    outColor = frameColor * InstanceTint() * uMaterial._LightIntensity;
    outColor.a = clamp(outColor.a, 0.0, 1.0);
}
