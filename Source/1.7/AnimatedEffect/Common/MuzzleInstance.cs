using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MuzzleFlash
{
    /// <summary>
    /// Mirrors the element layout of the std430 buffer _MuzzleInstances in
    /// MuzzleFlash.frag.glsl, stride 32 bytes: color(16) + frame(4) + padding(12).
    /// The GLSL side expresses the padding as 3 floats (a vec3 member is 16-byte aligned
    /// in std430, so a real vec3 would inflate the stride to 48).
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
