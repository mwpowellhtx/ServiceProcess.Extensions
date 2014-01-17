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
        public HarnessInstaller()
            : base(ServiceAccount.LocalSystem, new HarnessServiceInstaller())
        {
        }
    }
}
