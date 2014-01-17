using System.ServiceProcess.Definitions;

namespace Kingdom.ServiceProcess.Harness
{
    /// <summary>
    /// Thread HarnessService class.
    /// </summary>
    internal class ThreadHarnessService : AdaptableThreadServiceBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ThreadHarnessService()
            : base(new IServiceWorker[]
            {
                new ThreadHarnessServiceWorker(),
                new ThreadHarnessServiceWorker(),
                new ThreadHarnessServiceWorker(),
            })
        {
        }

        /// <summary>
        /// Waits for the Workers to have completed.
        /// </summary>
        /// <param name="workers"></param>
        protected override void WaitForWorkers(params IServiceWorker[] workers)
        {
        }
    }
}
