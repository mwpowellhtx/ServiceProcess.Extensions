using System.ServiceProcess.Definitions;

namespace Kingdom.ServiceProcess.Harness
{
    /// <summary>
    /// Task parallel HarnessService class.
    /// </summary>
    internal class TaskParallelHarnessService : AdaptableTaskParallelServiceBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TaskParallelHarnessService()
            : base(new IServiceWorker[]
            {
                new TaskParallelHarnessServiceWorker(),
                new TaskParallelHarnessServiceWorker(),
                new TaskParallelHarnessServiceWorker(),
            })
        {
        }
    }
}
