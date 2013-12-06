using System.Threading;
using System.Threading.Tasks;

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
        /// Task backing field.
        /// </summary>
        private Task _task;

        /// <summary>
        /// Gets the Task.
        /// </summary>
        /// <remarks>Derived classes must provide their own Worker Task.</remarks>
        public Task Task
        {
            get { return _task ?? (_task = NewTask(Scheduler)); }
        }

        /// <summary>
        /// CancelToken backing field.
        /// </summary>
        private readonly CancellationTokenSource _cancelToken;

        /// <summary>
        /// Gets the Cancellation.
        /// Derived classes should make use of this property
        /// in order to synchronize stoppage requests.
        /// </summary>
        /// <seealso cref="CancellationTokenSource"/>
        /// <seealso cref="CancellationTokenSource.IsCancellationRequested"/>
        protected CancellationTokenSource CancelToken
        {
            get { lock (_cancelToken) return _cancelToken; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AdaptableServiceWorker()
        {
            //TODO: Can register a Token delegate that runs when canceled.
            _cancelToken = new CancellationTokenSource();
            _continue = new ManualResetEvent(true);
            /* TODO: TBD: Look into do we go with a partitioner to break down
             * the Sudoku generation process event further? */
        }

        /// <summary>
        /// Returns a new Task.
        /// </summary>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        protected abstract Task NewTask(TaskScheduler scheduler);

        /// <summary>
        /// Starts the Task with the specified TaskScheduler.
        /// </summary>
        /// <remarks>Starting with the Default TaskScheduler.</remarks>
        protected virtual TaskScheduler Scheduler
        {
            get { return TaskScheduler.Default; }
        }

        /// <summary>
        /// Starts the Thread running using its starter.
        /// </summary>
        /// <param name="args"></param>
        public virtual void Start(params string[] args)
        {
            //Start the Task with the Scheduler.
            Task.Start(Scheduler);
        }

        /// <summary>
        /// Signals the Thread to Stop.
        /// </summary>
        public virtual void Stop()
        {
            //TODO: throwOnFirstException? Wire up a cancel callback?
            CancelToken.Cancel();
        }

        /// <summary>
        /// Signals the Thread to Pause.
        /// </summary>
        public virtual void Pause()
        {
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
