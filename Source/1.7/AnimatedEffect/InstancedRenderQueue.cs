using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace MuzzleFlash
{
    public class InstancedRenderQueue : IDisposable
    {
        private readonly InstancedRenderer _renderer;

        private readonly NativeBuffer<Vector3> _positionBuffer;
        private readonly NativeBuffer<Quaternion> _rotationBuffer;
        private readonly NativeBuffer<Vector3> _scaleBuffer;

        private readonly CalcMatricesBatchTask _matricesTask;

        protected int _count;

        public Mesh Mesh => _renderer.Mesh;

        public Material Material => _renderer.Material;

        public InstancedRenderQueue(Mesh mesh, Material mat)
        {
            if (mesh == null) throw ExceptionRendering.MeshIsMissing;
            if (mat == null) throw ExceptionRendering.MaterialIsMissing;
            if (!mat.enableInstancing) throw ExceptionRendering.MaterialNotInstanced;

            _renderer = new InstancedRenderer
            {
                Mesh = mesh,
                Material = mat,
                onUpdateData = UpdateData
            };

            _positionBuffer = new NativeBuffer<Vector3>(InstancedRenderer.MAX_COUNT_IN_BATCH);
            _rotationBuffer = new NativeBuffer<Quaternion>(InstancedRenderer.MAX_COUNT_IN_BATCH);
            _scaleBuffer = new NativeBuffer<Vector3>(InstancedRenderer.MAX_COUNT_IN_BATCH);
            _matricesTask = new CalcMatricesBatchTask();

            _count = 0;
        }

        public void Draw()
        {
            _renderer.Draw(_count);
        }

        public void AddInstance(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (_count >= InstancedRenderer.MAX_COUNT_IN_BATCH - 1)
            {
                Draw();
                return;
            }

            _positionBuffer[_count] = position;
            _rotationBuffer[_count] = rotation;
            _scaleBuffer[_count] = scale;
            _count++;
        }

        protected unsafe void UpdateData(int start, int length, StructuredBuffer<Matrix4x4> matrices, MaterialPropertyBlock propertyBlock)
        {
            if (start > 0) return; // TODO: warning
            fixed (Matrix4x4* ptr = matrices.Raw)
            {
                // BeginCalc only dispatches above CalcMatricesBatchTask.MinParallelCount; the
                // property update between Begin/End overlaps the worker fill, mirroring the
                // old Schedule ... Complete pairing.
                _matricesTask.BeginCalc(length, _positionBuffer, _rotationBuffer, _scaleBuffer, ptr);
                UpdateMaterialProperty(propertyBlock);
                _matricesTask.EndCalc();
                ResetBuffer();
            }

        }

        protected virtual void UpdateMaterialProperty(MaterialPropertyBlock propertyBlock)
        {

        }

        public virtual void Dispose()
        {
            _positionBuffer.Dispose();
            _rotationBuffer.Dispose();
            _scaleBuffer.Dispose();
            _matricesTask.Dispose();
        }

        private void ResetBuffer()
        {
            _count = 0;
        }


    }
}
