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
                new ThreadHarnessServiceWorker()
            })
        {
        }
    }
}
