using System.Collections.Generic;
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
        /// Gets the Worker Threads.
        /// </summary>
        IEnumerable<Thread> Threads { get; }

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
        private Lazy<IEnumerable<Thread>> _lazyThreads;

        /// <summary>
        /// Gets the Thread.
        /// </summary>
        /// <remarks>Derived classes must provide their own Worker Thread.</remarks>
        public IEnumerable<Thread> Threads
        {
            get { return _lazyThreads.Value; }
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
        {
            _startHandler = startHandler ?? (x =>
            {
                foreach (var thread in x.Threads)
                    thread.Start();
            });

            _stop = new ManualResetEvent(false);
            _completed = new ManualResetEvent(false);
        }

        /// <summary>
        /// Returns a range of Threads for the Worker.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Could be parameterized or not,
        ///  depending what the application requires.</remarks>
        protected abstract IEnumerable<Thread> GetThreads();

        /// <summary>
        /// Starts the Thread running using its starter.
        /// </summary>
        /// <param name="args"></param>
        public override void Start(params string[] args)
        {
            base.Start(args);

            _lazyThreads = new Lazy<IEnumerable<Thread>>(GetThreads);

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
        protected virtual bool IsStopRequested(TimeSpan timeout)
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
