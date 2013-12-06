namespace System.ServiceProcess.Helpers
{
    /// <summary>
    /// ServiceUninstallHelper interface.
    /// </summary>
    public interface IServiceUninstallHelper
    {
        /// <summary>
        /// Uninstalls the service.
        /// </summary>
        void Uninstall();
    }

    /// <summary>
    /// ServiceUninstallHelper class.
    /// </summary>
    internal class ServiceUninstallHelper : InstallHelperBase<ServiceUninstallHelper>, IServiceUninstallHelper
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ServiceUninstallHelper()
            : base()
        {
        }

        /// <summary>
        /// Uninstalls the service.
        /// </summary>
        public virtual void Uninstall()
        {
            //Just run the helper with the FullName.
            RunHelper(@"/u", EntryPath.FullName);
        }
    }
}
