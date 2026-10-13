using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;


namespace MuzzleFlash
{
    public delegate void SetDataForBatch(int start, int length, Matrix4x4[] matrices, MaterialPropertyBlock propertyBlock);

    public class InstancedRenderer
    {
        public const int MAX_COUNT_IN_BATCH = 1000;

        private Mesh _mesh;
        private Material _material;

        private readonly Matrix4x4[] _matrixBuffer;
        private readonly MaterialPropertyBlock _propertyBlock;

        public SetDataForBatch onUpdateData;

        public Mesh Mesh
        {
            get { return _mesh; }
            set { _mesh = value; }
        }

        public Material Material
        {
            get { return _material; }
            set { _material = value; }
        }

        public Matrix4x4[] MatrixBuffer => _matrixBuffer;
        public MaterialPropertyBlock PropertyBlock => _propertyBlock;

        public InstancedRenderer()
        {
            _matrixBuffer = new Matrix4x4[MAX_COUNT_IN_BATCH];
            _propertyBlock = new MaterialPropertyBlock();
        }

        public InstancedRenderer(Mesh mesh, Material material)
        {
            _matrixBuffer = new Matrix4x4[MAX_COUNT_IN_BATCH];
            _propertyBlock = new MaterialPropertyBlock();

            Mesh = mesh;
            Material = material;
        }

        public InstancedRenderer(Mesh mesh, Shader shader)
        {
            _matrixBuffer = new Matrix4x4[MAX_COUNT_IN_BATCH];
            _propertyBlock = new MaterialPropertyBlock();

            Mesh = mesh;
            Material = UtilsMaterial.CreateMaterial(shader);
        }

        public void Draw(int count)
        {
            int fullBatchLength = count / MAX_COUNT_IN_BATCH;
            for (int i = 0; i < fullBatchLength; i++)
            {
                onUpdateData?.Invoke(i * MAX_COUNT_IN_BATCH, MAX_COUNT_IN_BATCH, _matrixBuffer, _propertyBlock);
                Graphics.DrawMeshInstanced(Mesh, 0, Material, _matrixBuffer, MAX_COUNT_IN_BATCH, _propertyBlock);
            }

            int remain = count % MAX_COUNT_IN_BATCH;
            if (remain > 0)
            {
                onUpdateData?.Invoke(fullBatchLength * MAX_COUNT_IN_BATCH, remain, _matrixBuffer, _propertyBlock);
                Graphics.DrawMeshInstanced(Mesh, 0, Material, _matrixBuffer, remain, PropertyBlock);
            }
            PropertyBlock.Clear();
        }
    }
}
