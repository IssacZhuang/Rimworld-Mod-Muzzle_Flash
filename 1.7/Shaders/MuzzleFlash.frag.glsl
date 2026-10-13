#version 450

#ifdef INSTANCED
// Waylith 从片元着色器提取材质绑定，顶点阶段使用的缓冲区也必须在此声明。
#include "InstanceTransforms.glsl"

layout(location = 1) flat in int vInstanceIndex;

// 与 C# MuzzleInstance 一一对应, stride 32 字节。
// std430 中 vec3 成员对齐为 16 字节, 直接写 vec3 _padding 会把 stride 撑到 48,
// 因此 padding 用 3 个 float 表达 (C# 侧为 float frame + Vector3 padding, 布局等价)。
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
    vec4 _Splits;          // @default = vec4(5.0, 5.0, 3.0, 5.0)  x=列数 y=行数
} uMaterial;

layout(set = 2, binding = 1) uniform sampler2D _MainTex;

#ifdef INSTANCED
float InstanceFrame() { return muzzleInstances[vInstanceIndex].frame; }
vec4 InstanceTint()   { return muzzleInstances[vInstanceIndex].color; }
#else
float InstanceFrame() { return 0.0; }
vec4 InstanceTint()   { return uMaterial._Color; }
#endif

// 精灵图集采样: stage 为帧序号
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

    // frame 为浮点帧号, 对相邻两帧插值实现平滑过渡
    float stage = mod(InstanceFrame(), float(stages));
    int current = int(floor(stage));
    int next = int(mod(stage + 1.0, float(stages)));

    vec2 uv = vUv * uMaterial._MainTex_ST.zw + uMaterial._MainTex_ST.xy;
    vec4 frameColor = mix(shot(uv, dx, dy, current), shot(uv, dx, dy, next), stage - float(current));

    outColor = frameColor * InstanceTint() * uMaterial._LightIntensity;
    outColor.a = clamp(outColor.a, 0.0, 1.0);
}
