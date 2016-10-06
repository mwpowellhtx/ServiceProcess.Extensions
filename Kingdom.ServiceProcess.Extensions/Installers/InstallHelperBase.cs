using System.Configuration.Install;
using System.IO;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace System.ServiceProcess.Helpers
{
    /// <summary>
    /// InstallHelperBase class.
    /// </summary>
    internal abstract class InstallHelperBase<T>
        where T : InstallHelperBase<T>, new()
    {
        /// <summary>
        /// Instance backing field.
        /// </summary>
        internal static readonly T Instance = new T();

        /// <summary>
        /// Constructor.
        /// </summary>
        protected InstallHelperBase()
        {
        }

        /// <summary>
        /// Gets the EntryPath.
        /// </summary>
        protected FileInfo EntryPath
        {
            get
            {
                var path = Assembly.GetEntryAssembly().Location;
                return new FileInfo(path);
            }
        }

        /// <summary>
        /// Runs the helper.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual bool RunHelper(params string[] args)
        {
            try
            {
                ManagedInstallerClass.InstallHelper(args);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
