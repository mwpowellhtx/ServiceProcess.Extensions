using System.ServiceProcess.Definitions;

namespace Kingdom.ServiceProcess.Extensions
{
    /// <summary>
    /// HarnessService class.
    /// </summary>
    internal class HarnessService : AdaptableServiceBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public HarnessService()
            : base(new IServiceWorker[]
            {
                new HarnessServiceWorker(),
                new HarnessServiceWorker(),
                new HarnessServiceWorker(),
            })
        {
        }
    }
}
