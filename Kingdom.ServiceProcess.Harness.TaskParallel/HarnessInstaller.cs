using System.ComponentModel;
using System.ServiceProcess;

namespace Kingdom.ServiceProcess.Harness
{
    /// <summary>
    /// HarnessInstaller class.
    /// </summary>
    [RunInstaller(true)]
    public class HarnessInstaller : AdaptableServiceInstaller
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public HarnessInstaller()
            : base(ServiceAccount.LocalSystem, new HarnessServiceInstaller())
        {
        }
    }
}
