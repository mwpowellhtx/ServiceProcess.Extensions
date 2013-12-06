using System.ServiceProcess;

namespace Kingdom.ServiceProcess.Extensions
{
    /// <summary>
    /// HarnessServiceInstaller class.
    /// </summary>
    public class HarnessServiceInstaller : ServiceInstaller
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal HarnessServiceInstaller()
            : base()
        {
            /* TODO: TBD: Anything else that could be exposed, such as through options?
             * Should worker(s) be responsible for informing about their installer(s)? */
            base.ServiceName = "HarnessService";
            base.DisplayName = "Harness Service";
            base.DelayedAutoStart = false;
            base.StartType = ServiceStartMode.Automatic;
        }
    }
}
