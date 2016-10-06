// ReSharper disable once CheckNamespace
namespace System.ServiceProcess.Definitions
{
    /// <summary>
    /// ServiceWorker interface.
    /// </summary>
    public interface IServiceWorker
    {
        /// <summary>
        /// Signals the ServiceWorker to Start.
        /// </summary>
        /// <param name="args"></param>
        void Start(params string[] args);

        /// <summary>
        /// Signals the ServiceWorker to Stop.
        /// </summary>
        void Stop();

        /// <summary>
        /// Signals the ServiceWorker to Pause.
        /// </summary>
        void Pause();

        /// <summary>
        /// Signals the ServiceWorker to Continue.
        /// </summary>
        void Continue();
    }
}
