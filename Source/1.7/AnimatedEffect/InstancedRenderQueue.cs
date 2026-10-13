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
        private readonly NativeBuffer<Vector2> _rotationBuffer;
        private readonly NativeBuffer<Vector3> _scaleBuffer;

        private readonly CalcMatricesJob _matricesJob;

        protected int _count;

        public Mesh Mesh => _renderer.Mesh;

        public Material Material => _renderer.Material;

        public InstancedRenderQueue(Mesh mesh, Material mat)
        {
            if (mesh == null) throw ExceptionRendering.MeshIsMissing;
            if (mat == null) throw ExceptionRendering.MaterialIsMissing;

            _renderer = new InstancedRenderer
            {
                Mesh = mesh,
                Material = mat,
                onUpdateData = UpdateData
            };

            _positionBuffer = new NativeBuffer<Vector3>(InstancedRenderer.MAX_COUNT_IN_BATCH);
            _rotationBuffer = new NativeBuffer<Vector2>(InstancedRenderer.MAX_COUNT_IN_BATCH);
            _scaleBuffer = new NativeBuffer<Vector3>(InstancedRenderer.MAX_COUNT_IN_BATCH);
            _matricesJob = new CalcMatricesJob();

            _count = 0;
        }

        public void Draw()
        {
            _renderer.Draw(_count);
        }

        public void AddInstance(Vector3 position, float rotCos, float rotSin, Vector3 scale)
        {
            if (_count >= InstancedRenderer.MAX_COUNT_IN_BATCH - 1)
            {
                Draw();
                return;
            }

            _positionBuffer[_count] = position;
            _rotationBuffer[_count] = new Vector2(rotCos, rotSin);
            _scaleBuffer[_count] = scale;
            _count++;
        }

        protected unsafe void UpdateData(int start, int length, Matrix4x4[] matrices, MaterialPropertyBlock propertyBlock)
        {
            if (start > 0) return; // TODO: warning
            fixed (Matrix4x4* ptr = matrices)
            {
                // BeginCalc only dispatches above CalcMatricesJob.MinParallelCount; the
                // property update between Begin/End overlaps the worker fill.
                _matricesJob.BeginCalc(length, _positionBuffer, _rotationBuffer, _scaleBuffer, ptr);
                UpdateMaterialProperty(propertyBlock);
                _matricesJob.EndCalc();
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
        }

        private void ResetBuffer()
        {
            _count = 0;
        }


    }
}
