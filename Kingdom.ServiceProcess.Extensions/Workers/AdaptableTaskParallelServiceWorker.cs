using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceProcess.Definitions
{
    /// <summary>
    /// Task parallel service worker interface.
    /// </summary>
    public interface ITaskParallelServiceWorker : IServiceWorker
    {
        /// <summary>
        /// Gets the ServiceWorker Task.
        /// </summary>
        /// <remarks>It gets very complicated if we want to coordinate with Threads for
        /// .NET 3.5. So we'll just go with 4.0+ for now, and decide whether .NET 3.5 is
        /// worth the effort later. There is a TPL back-port if we want to maintain some
        /// degree of consistency with Tasks if needs be, however.</remarks>
        Task Task { get; }
    }

    /// <summary>
    /// Adaptable task parallel service worker class.
    /// </summary>
    public abstract class AdaptableTaskParallelServiceWorker : AdaptableServiceWorker,
        ITaskParallelServiceWorker
    {
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
        protected AdaptableTaskParallelServiceWorker()
            : base()
        {
            //TODO: Can register a Token delegate that runs when canceled.
            _cancelToken = new CancellationTokenSource();
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
        public override void Start(params string[] args)
        {
            base.Start(args);

            //Start the Task with the Scheduler.
            Task.Start(Scheduler);
        }

        /// <summary>
        /// Signals the Thread to Stop.
        /// </summary>
        public override void Stop()
        {
            base.Stop();

            //TODO: throwOnFirstException? Wire up a cancel callback?
            CancelToken.Cancel();
        }
    }
}
