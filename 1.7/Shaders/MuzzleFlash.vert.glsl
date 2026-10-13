#version 450

#ifdef INSTANCED
#include "InstanceTransforms.glsl"

layout(location = 1) flat out int vInstanceIndex;
#endif

// MeshPool.plane10 顶点布局, 与本体 Mote.vert.glsl 相同的输入约定
layout(location = 0) in vec3 inPosition;  // @vertex Position required
layout(location = 3) in vec4 inUv0;       // @vertex TexCoord0 optional @default = vec4(0.0)

layout(push_constant) uniform Push { mat4 modelMatrix; } push;

layout(location = 0) out vec2 vUv;

layout(set = 0, binding = 0) uniform VertexUniforms
{
    mat4 ViewProj;
} uVertex;

void main()
{
#ifdef INSTANCED
    vInstanceIndex = gl_InstanceIndex;
    vec4 world = instanceTransforms[gl_InstanceIndex] * vec4(inPosition, 1.0);
#else
    vec4 world = push.modelMatrix * vec4(inPosition, 1.0);
#endif
    gl_Position = uVertex.ViewProj * world;

    vUv = inUv0.xy;
}
