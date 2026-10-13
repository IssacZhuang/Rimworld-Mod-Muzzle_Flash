using UnityEngine;

namespace MuzzleFlash
{
    /// <summary>
    /// Replaces the 1.6 Unity IJobParallelFor version (JobCalcMatricesUnsafe): composes
    /// TRS matrices from the raw instance buffers through a reusable batch task, so a
    /// full tick of queue draws runs without any allocation.
    /// </summary>
    public sealed unsafe class CalcMatricesBatchTask : ReusableBatchTask
    {
        // Below this instance count a sequential loop beats thread dispatch latency
        // (<=1000 items is only a few microseconds of work).
        public const int MinParallelCount = 256;

        private NativeBuffer<Vector3> _positions;
        private NativeBuffer<Quaternion> _rotations;
        private NativeBuffer<Vector3> _scales;
        // Managed array memory: the caller must keep it pinned (fixed) for the whole
        // BeginCalc..EndCalc window, so it is passed as a raw pointer.
        private Matrix4x4* _matrices;
        private bool _parallelPending;

        /// <summary>
        /// Starts the matrix calculation. Below <see cref="MinParallelCount"/> the whole batch
        /// runs inline and <see cref="EndCalc"/> becomes a no-op.
        /// </summary>
        public void BeginCalc(int count, NativeBuffer<Vector3> positions, NativeBuffer<Quaternion> rotations, NativeBuffer<Vector3> scales, Matrix4x4* matrices)
        {
            _positions = positions;
            _rotations = rotations;
            _scales = scales;
            _matrices = matrices;

            if (count < MinParallelCount)
            {
                _parallelPending = false;
                Calc(count);
                return;
            }

            _parallelPending = true;
            BeginParallel(count);
        }

        public void EndCalc()
        {
            if (!_parallelPending) return;
            _parallelPending = false;
            EndParallel();
        }

        private void Calc(int count)
        {
            for (int i = 0; i < count; i++)
            {
                ExecuteCore(i);
            }
        }

        protected override void ExecuteCore(int index)
        {
            _matrices[index].SetTRS(_positions.Raw[index], _rotations.Raw[index], _scales.Raw[index]);
        }
    }
}
