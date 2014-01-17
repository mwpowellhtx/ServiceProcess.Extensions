using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.ServiceProcess.Definitions
{
    /// <summary>
    /// Adaptable task parallel service base class.
    /// </summary>
    public abstract class AdaptableTaskParallelServiceBase : AdaptableServiceBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="workers"></param>
        protected AdaptableTaskParallelServiceBase(IEnumerable<IServiceWorker> workers)
            : base(workers)
        {
            workers.VerifyServiceWorkers<ITaskParallelServiceWorker>();
        }

        /// <summary>
        /// Waits for task parallel ServiceWorkers to complete.
        /// </summary>
        /// <param name="workers"></param>
        protected override void WaitForWorkers(params IServiceWorker[] workers)
        {
            var tpsws = workers.OfType<ITaskParallelServiceWorker>()
                .Select(x => x.Task).ToArray();
            Task.WaitAll(tpsws);
        }
    }
}
