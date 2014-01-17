using System.Collections.Generic;

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
            : base(workers)
        {
        }
    }
}
