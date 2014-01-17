using System.Threading;

namespace System.ServiceProcess.Definitions
{
    /// <summary>
    /// AdaptableServiceWorker class.
    /// </summary>
    public abstract class AdaptableServiceWorker : IServiceWorker
    {
        /// <summary>
        /// Continue event backing field.
        /// </summary>
        private readonly ManualResetEvent _continue;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AdaptableServiceWorker()
        {
            _continue = new ManualResetEvent(true);
            /* TODO: TBD: Look into do we go with a partitioner to break down
             * the Sudoku generation process event further? */
        }

        /// <summary>
        /// Starts the Thread running using its starter.
        /// </summary>
        /// <param name="args"></param>
        public virtual void Start(params string[] args)
        {
        }

        /// <summary>
        /// Signals the Thread to Stop.
        /// </summary>
        public virtual void Stop()
        {
        }

        /// <summary>
        /// Signals the Thread to Pause.
        /// </summary>
        public virtual void Pause()
        {
            /* TODO: Pause and/or Continue needs to remain Stop-aware.
             * Or, that is the responsibility of the Service-vendor to
             * pay attention to the MayContinue/CancelToken status. */
            lock (_continue) _continue.Reset();
        }

        /// <summary>
        /// Signals the Thread to Continue.
        /// </summary>
        public virtual void Continue()
        {
            lock (_continue) _continue.Set();
        }

        /// <summary>
        /// Returns whether the Worker MayContinue.
        /// </summary>
        /// <param name="timeoutMilliseconds">Timeout in milliseconds.</param>
        /// <returns>Whether the Worker MayContinue.</returns>
        protected virtual bool MayContinue(int timeoutMilliseconds)
        {
            return MayContinue(TimeSpan.FromMilliseconds(timeoutMilliseconds));
        }

        /// <summary>
        /// Returns whether the Worker MayContinue.
        /// </summary>
        /// <param name="timeout">Timeout value.</param>
        /// <returns>Whether the Worker MayContinue.</returns>
        protected virtual bool MayContinue(TimeSpan timeout)
        {
            lock (_continue) return _continue.WaitOne(timeout);
        }
    }
}
