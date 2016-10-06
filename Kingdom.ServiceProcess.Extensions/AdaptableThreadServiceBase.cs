using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace System.ServiceProcess.Definitions
{
    /// <summary>
    /// AdaptableThreadServiceBase class.
    /// </summary>
    public abstract class AdaptableThreadServiceBase : AdaptableServiceBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="workers"></param>
        protected AdaptableThreadServiceBase(IEnumerable<IServiceWorker> workers)
            // ReSharper disable once PossibleMultipleEnumeration
            : base(workers)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            workers.VerifyServiceWorkers<IThreadServiceWorker>();
        }

        /// <summary>
        /// Waits for task parallel ServiceWorkers to complete.
        /// </summary>
        /// <param name="workers"></param>
        protected override void WaitForWorkers(params IServiceWorker[] workers)
        {
            var tpsws = workers.OfType<IThreadServiceWorker>();

            /* TODO: TBD: May want to wire up some sort of event should there be any help we need
             * to inject into the thread-completion process. Might even be as simple as requesting
             * another block of time from the SC, but which cannot be known at this framework level. */

            // ReSharper disable once PossibleMultipleEnumeration
            while (tpsws.Any(x => !x.HasCompleted()))
            {
            }
        }
    }
}
