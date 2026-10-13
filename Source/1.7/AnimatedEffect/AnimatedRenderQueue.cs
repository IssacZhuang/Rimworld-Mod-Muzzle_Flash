using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using MuzzleFlash;

namespace MuzzleFlash
{
    /// <summary>
    /// RimWorld 1.7 (Waylith): per-instance data (color + frame) is packed into the
    /// custom struct MuzzleInstance, uploaded via ComputeBuffer(Structured) and consumed
    /// by the _MuzzleInstances SSBO in the shader, instead of per-instance array
    /// properties on a MaterialPropertyBlock.
    /// </summary>
    public class AnimatedRenderQueue : InstancedRenderQueue
    {
        private readonly MuzzleInstance[] _instanceBuffer;
        private readonly ComputeBuffer _gpuInstanceBuffer;

        public AnimatedRenderQueue(Mesh mesh, Material mat) : base(mesh, mat)
        {
            _instanceBuffer = new MuzzleInstance[InstancedRenderer.MAX_COUNT_IN_BATCH];
            _gpuInstanceBuffer = new ComputeBuffer(InstancedRenderer.MAX_COUNT_IN_BATCH, MuzzleInstance.Stride, ComputeBufferType.Structured);
        }

        public void AddInstance(Vector3 position, float rotCos, float rotSin, Vector3 scale, Color color, float frame)
        {
            _instanceBuffer[_count] = new MuzzleInstance(color, frame);
            AddInstance(position, rotCos, rotSin, scale);
        }

        protected override void UpdateMaterialProperty(MaterialPropertyBlock propertyBlock)
        {
            // UpdateData runs before ResetBuffer, so _count still equals this batch's instance count
            _gpuInstanceBuffer.SetData(_instanceBuffer, 0, 0, _count);
            propertyBlock.SetBuffer(ShaderPropertyID.muzzleInstances, _gpuInstanceBuffer);
        }

        public override void Dispose()
        {
            base.Dispose();
            _gpuInstanceBuffer.Dispose();
        }
    }
}
