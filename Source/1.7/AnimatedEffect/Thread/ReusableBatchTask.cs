using System;
using System.Collections.Generic;
using System.Threading;

namespace MuzzleFlash
{
    /// <summary>
    /// Reusable zero-allocation parallel batch task, ported from Alco (<c>Src/Alco/Thread/ReusableBatchTask.cs</c>).
    /// Work items are pooled and queued via <see cref="IThreadPoolWorkItem"/>, so steady-state runs
    /// (e.g. every tick) do not allocate. ExecuteCore must only touch memory that stays valid and
    /// unmutated until the run completes.
    /// </summary>
    public abstract class ReusableBatchTask : AutoDisposable
    {
        private class TaskItem : IThreadPoolWorkItem
        {
            public volatile int IndexStart;
            public volatile int Count;
            public volatile Exception Exception;

            private readonly ReusableBatchTask _task;

            public TaskItem(ReusableBatchTask task)
            {
                _task = task;
            }

            public void Execute()
            {
                Exception = null;
                try
                {
                    for (int i = IndexStart; i < IndexStart + Count; i++)
                    {
                        _task.ExecuteCore(i);
                    }
                }
                catch (Exception e)
                {
                    Exception = e;
                }
                finally
                {
                    _task._completionCount.Release();
                }
            }
        }

        private readonly SemaphoreSlim _completionCount;
        private readonly int _maxConcurrency;
        private readonly List<TaskItem> _taskItems;
        private readonly List<Exception> _exceptions;
        private int _pendingTaskCount;

        /// <param name="maxConcurrency">Upper bound on work items queued per run; 0 = processor count.</param>
        public ReusableBatchTask(int maxConcurrency = 0)
        {
            _maxConcurrency = maxConcurrency <= 0 ? Environment.ProcessorCount : maxConcurrency;
            _completionCount = new SemaphoreSlim(0, int.MaxValue);
            _taskItems = new List<TaskItem>();
            _exceptions = new List<Exception>();
        }

        protected abstract void ExecuteCore(int index);

        /// <summary>
        /// Runs all indices in parallel and blocks until the whole batch finished.
        /// </summary>
        /// <param name="batchSize">Items per work item; null = totalCount split evenly over maxConcurrency.</param>
        public void RunParallel(int totalCount, int? batchSize = null)
        {
            BeginParallel(totalCount, batchSize);
            EndParallel();
        }

        /// <summary>
        /// Queues the batch and returns immediately. The owner thread must call <see cref="EndParallel"/>
        /// before inspecting results or starting the next run; Begin/End must not nest or interleave.
        /// </summary>
        public void BeginParallel(int totalCount, int? batchSize = null)
        {
            _pendingTaskCount = 0;
            if (totalCount <= 0) return;

            int actualBatchSize = batchSize ?? Math.Max(1, totalCount / _maxConcurrency);
            int taskCount = (totalCount + actualBatchSize - 1) / actualBatchSize;

            while (_taskItems.Count < taskCount)
            {
                _taskItems.Add(new TaskItem(this));
            }

            for (int i = 0; i < taskCount; i++)
            {
                int start = i * actualBatchSize;
                int count = Math.Min(actualBatchSize, totalCount - start);

                var item = _taskItems[i];
                item.IndexStart = start;
                item.Count = count;
                item.Exception = null;
                ThreadPool.UnsafeQueueUserWorkItem(item, false);
            }

            _pendingTaskCount = taskCount;
        }

        /// <summary>
        /// Blocks until every item queued by <see cref="BeginParallel"/> completed, then rethrows
        /// worker exceptions as an <see cref="AggregateException"/>.
        /// </summary>
        public void EndParallel()
        {
            int taskCount = _pendingTaskCount;
            _pendingTaskCount = 0;
            if (taskCount == 0) return;

            for (int i = 0; i < taskCount; i++)
            {
                _completionCount.Wait();
            }

            _exceptions.Clear();
            for (int i = 0; i < taskCount; i++)
            {
                var item = _taskItems[i];
                if (item.Exception != null)
                {
                    _exceptions.Add(item.Exception);
                }
            }

            if (_exceptions.Count > 0)
            {
                throw new AggregateException("One or more exceptions occurred during parallel execution.", _exceptions);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _completionCount?.Dispose();
            }
        }
    }
}
