using UnityEngine;
using Waylith.Jobs;

namespace MuzzleFlash
{
    /// <summary>
    /// Replaces the 1.6 Unity IJobParallelFor version and the interim hand-rolled
    /// ReusableBatchTask: composes TRS matrices through the engine job runtime
    /// (WaylithJobRuntime, dedicated workers with work stealing), so a full queue
    /// draw runs without any mod-side allocation.
    /// </summary>
    public sealed unsafe class CalcMatricesJob
    {
        // Below this instance count a sequential loop beats thread dispatch latency
        // (<=1000 items is only a few microseconds of work).
        public const int MinParallelCount = 256;

        private struct Job : IWaylithParallelForJob
        {
            public Vector3* positions;
            public Vector2* rotations; // x = cos(yaw), y = sin(yaw)
            public Vector3* scales;
            public Matrix4x4* matrices;

            // Equal to Matrix4x4.TRS(pos, Quaternion.AngleAxis(yaw, Vector3.up), scale);
            // entities are 2D sprites, so the yaw rotation reduces to this fixed
            // column layout (verified numerically against the TRS reference path).
            public void Execute(int index)
            {
                Vector2 rot = rotations[index];
                Vector3 s = scales[index];
                Vector3 p = positions[index];
                Matrix4x4* m = &matrices[index];
                m->m00 = rot.x * s.x;  m->m01 = 0f;        m->m02 = rot.y * s.z;  m->m03 = p.x;
                m->m10 = 0f;           m->m11 = s.y;       m->m12 = 0f;           m->m13 = p.y;
                m->m20 = -rot.y * s.x; m->m21 = 0f;        m->m22 = rot.x * s.z;  m->m23 = p.z;
                m->m30 = 0f;           m->m31 = 0f;        m->m32 = 0f;           m->m33 = 1f;
            }
        }

        private int _handle;

        /// <summary>
        /// Starts the matrix calculation. Below <see cref="MinParallelCount"/> the whole batch
        /// runs inline and <see cref="EndCalc"/> becomes a no-op.
        /// </summary>
        public void BeginCalc(int count, NativeBuffer<Vector3> positions, NativeBuffer<Vector2> rotations, NativeBuffer<Vector3> scales, Matrix4x4* matrices)
        {
            var job = new Job
            {
                positions = positions.Raw,
                rotations = rotations.Raw,
                scales = scales.Raw,
                matrices = matrices,
            };

            if (count < MinParallelCount)
            {
                for (int i = 0; i < count; i++)
                {
                    job.Execute(i);
                }
                return;
            }

            _handle = WaylithJobRuntime.ScheduleFor(job, count, WaylithJobRuntime.IdealBatchCount(count));
        }

        public void EndCalc()
        {
            int handle = _handle;
            _handle = 0;
            WaylithJobRuntime.Complete(handle);
        }
    }
}
