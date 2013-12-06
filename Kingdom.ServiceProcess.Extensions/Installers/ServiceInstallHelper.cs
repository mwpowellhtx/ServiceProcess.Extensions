namespace System.ServiceProcess.Helpers
{
    /// <summary>
    /// ServiceInstallHelper interface.
    /// </summary>
    internal interface IServiceInstallHelper
    {
        /// <summary>
        /// Installs the service.
        /// </summary>
        void Install();
    }

    /// <summary>
    /// ServiceInstallHelper class.
    /// </summary>
    internal class ServiceInstallHelper : InstallHelperBase<ServiceInstallHelper>, IServiceInstallHelper
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ServiceInstallHelper()
            : base()
        {
        }

        /// <summary>
        /// Installs the service.
        /// </summary>
        public virtual void Install()
        {
            //Just run the helper with the FullName.
            RunHelper(EntryPath.FullName);
        }
    }
}
