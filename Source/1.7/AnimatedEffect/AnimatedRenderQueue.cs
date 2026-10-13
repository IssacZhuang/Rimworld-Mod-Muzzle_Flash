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
    /// RimWorld 1.7 (Waylith): 每实例数据(color+frame)打包进自定义结构体 MuzzleInstance，
    /// 经 ComputeBuffer(Structured) 上传 GPU，由 shader 中的 _MuzzleInstances SSBO 消费，
    /// 不再使用 MaterialPropertyBlock 的逐实例数组属性。
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

        public void AddInstance(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float frame)
        {
            _instanceBuffer[_count] = new MuzzleInstance(color, frame);
            AddInstance(position, rotation, scale);
        }

        protected override void UpdateMaterialProperty(MaterialPropertyBlock propertyBlock)
        {
            // UpdateData 在 ResetBuffer 之前调用, 此时 _count 即本批实例数
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
