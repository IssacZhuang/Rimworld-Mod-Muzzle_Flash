#version 450

// 诊断用 shader (DevMode): 与 MuzzleFlash.frag.glsl 相同的 SSBO 声明,
// 品红 = 绘制管线通; 叠加绿色 = _MuzzleInstances SSBO 数据到达
#ifdef INSTANCED
// Waylith 从片元着色器提取材质绑定，顶点阶段使用的缓冲区也必须在此声明。
#include "InstanceTransforms.glsl"

layout(location = 1) flat in int vInstanceIndex;

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

void main()
{
    vec3 col = vec3(1.0, 0.0, 1.0);
#ifdef INSTANCED
    float g = muzzleInstances[vInstanceIndex].color.g;
    outColor = vec4(col + vec3(0.0, g, 0.0), 0.8);
#else
    outColor = vec4(col, 0.8);
#endif
}
