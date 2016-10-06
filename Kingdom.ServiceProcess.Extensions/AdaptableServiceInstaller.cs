using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

// ReSharper disable once CheckNamespace
namespace Kingdom.ServiceProcess
{
    //TODO: TBD: Whether to go ahead and attach the RunInstaller(true) to the base-class.
    /// <summary>
    /// AdaptableServiceInstaller Installer class.
    /// </summary>
    /// <seealso cref="RunInstallerAttribute"/>
    public abstract class AdaptableServiceInstaller : Installer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="serviceInstallers"></param>
        protected AdaptableServiceInstaller(ServiceAccount account, params ServiceInstaller[] serviceInstallers)
        {
            var processInstaller = new ServiceProcessInstaller() {Account = account,};

            Installers.Add(processInstaller);

            var temp = new List<Installer>();
            temp.AddRange(serviceInstallers);

            Installers.AddRange(temp.ToArray());
        }
    }
}
