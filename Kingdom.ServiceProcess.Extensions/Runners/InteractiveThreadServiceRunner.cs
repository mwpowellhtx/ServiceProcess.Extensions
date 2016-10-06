using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace System.ServiceProcess.Definitions
{
    /// <summary>
    /// InteractiveThreadServiceRunner class.
    /// </summary>
    public abstract class InteractiveThreadServiceRunner : InteractiveServiceRunnerBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="workers"></param>
        protected InteractiveThreadServiceRunner(IEnumerable<IServiceWorker> workers)
            // ReSharper disable once PossibleMultipleEnumeration
            : base(workers)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            workers.VerifyServiceWorkers<IThreadServiceWorker>();
        }

        /// <summary>
        /// Waits for the ServiceWorkers.
        /// </summary>
        /// <param name="workers"></param>
        protected sealed override void WaitForWorkers(params IServiceWorker[] workers)
        {
            var tsws = workers.OfType<IThreadServiceWorker>();

            while (tsws.Any(x => !x.HasCompleted()))
            {
                /* TODO: Spin on this one? May want to await response.
                 * i.e. Poll event-listener(s) what to do with every iteration. */
            }
        }
    }
}
