using System;
using System.Threading;

namespace MuzzleFlash
{
    /// <summary>
    /// Base class for objects with complex life cycle: Dispose runs at most once,
    /// either manually or from the finalizer. Ported from Alco.
    /// </summary>
    public abstract class AutoDisposable : IDisposable
    {
        private volatile uint _disposed;

        public bool IsDisposed => _disposed != 0;

        ~AutoDisposable()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                Dispose(false);
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        protected abstract void Dispose(bool disposing);
    }
}
