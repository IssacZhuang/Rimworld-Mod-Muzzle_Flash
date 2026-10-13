using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MuzzleFlash
{
    /// <summary>
    /// 与 MuzzleFlash.frag.glsl 中 std430 buffer _MuzzleInstances 的元素布局一一对应，
    /// stride 32 字节: color(16) + frame(4) + padding(12)。
    /// GLSL 侧用 3 个 float 表达 padding（std430 中 vec3 成员对齐为 16，直接用 vec3 会把 stride 撑到 48）。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MuzzleInstance
    {
        public Vector4 color;
        public float frame;
        public Vector3 padding;

        public const int Stride = 32;

        public MuzzleInstance(Color color, float frame)
        {
            this.color = color;
            this.frame = frame;
            padding = default;
        }
    }
}
