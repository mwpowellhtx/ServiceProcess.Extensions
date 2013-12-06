using System.ComponentModel;
using System.ServiceProcess;

namespace Kingdom.ServiceProcess.Extensions
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
