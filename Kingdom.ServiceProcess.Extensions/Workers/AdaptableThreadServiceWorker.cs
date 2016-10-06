using System.Threading;

// ReSharper disable once CheckNamespace
namespace System.ServiceProcess.Definitions
{
    /// <summary>
    /// Thread service worker interface.
    /// </summary>
    public interface IThreadServiceWorker : IServiceWorker
    {
        /// <summary>
        /// Gets the worker Thread.
        /// </summary>
        Thread Thread { get; }

        /// <summary>
        /// Returns whether the ServiceWorker HasCompleted.
        /// </summary>
        /// <returns>Whether the ServiceWorker HasCompleted.</returns>
        bool HasCompleted();
    }

    /// <summary>
    /// Adaptable thread service worker class.
    /// </summary>
    public abstract class AdaptableThreadServiceWorker : AdaptableServiceWorker,
        IThreadServiceWorker
    {
        /// <summary>
        /// Thread backing field.
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// Gets the Thread.
        /// </summary>
        /// <remarks>Derived classes must provide their own Worker Thread.</remarks>
        public Thread Thread
        {
            get { return _thread ?? (_thread = NewThread()); }
        }

        /// <summary>
        /// Stop ResetEvent backing field.
        /// </summary>
        private readonly ManualResetEvent _stop;

        /// <summary>
        /// Completed ResetEvent backing field.
        /// </summary>
        private readonly ManualResetEvent _completed;

        /// <summary>
        /// Delegated StartHandler backing field.
        /// </summary>
        private readonly Action<IThreadServiceWorker> _startHandler;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="startHandler">Default handler starts Thread with no parameters.</param>
        protected AdaptableThreadServiceWorker(Action<IThreadServiceWorker> startHandler = null)
            : base()
        {
            _startHandler = startHandler ?? (x => x.Thread.Start());
            _stop = new ManualResetEvent(false);
            _completed = new ManualResetEvent(false);
        }

        /// <summary>
        /// Returns a new Thread.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Could be parameterized or not,
        ///  depending what the application requires.</remarks>
        protected abstract Thread NewThread();

        /// <summary>
        /// Starts the Thread running using its starter.
        /// </summary>
        /// <param name="args"></param>
        public override void Start(params string[] args)
        {
            base.Start(args);

            _startHandler(this);
        }

        /// <summary>
        /// Signals the Thread to Stop.
        /// </summary>
        public override void Stop()
        {
            base.Stop();

            lock (_stop) _stop.Set();
        }

        /// <summary>
        /// Sets whether the ServiceWorker HasCompleted.
        /// </summary>
        protected void SetCompleted()
        {
            lock (_completed) _completed.Set();
        }

        /// <summary>
        /// Returns whether Stop is requested.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        protected bool IsStopRequested(TimeSpan timeout)
        {
            lock (_stop) return _stop.WaitOne(timeout);
        }

        /// <summary>
        /// Returns whether Stop is requested.
        /// </summary>
        /// <param name="timeoutMilliseconds"></param>
        /// <returns></returns>
        protected bool IsStopRequested(double? timeoutMilliseconds = null)
        {
            //Timeout or Zero when Null.
            var actualTimeout = timeoutMilliseconds ?? 0d;
            return IsStopRequested(TimeSpan.FromMilliseconds(actualTimeout));
        }

        /// <summary>
        /// Returns whether the ServiceWorker HasCompleted.
        /// </summary>
        /// <returns>Whether the ServiceWorker HasCompleted.</returns>
        public virtual bool HasCompleted()
        {
            lock (_completed) return _completed.WaitOne(0);
        }
    }
}
