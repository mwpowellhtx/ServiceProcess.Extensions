using System.Threading.Tasks;

namespace System.ServiceProcess.Definitions
{
    /// <summary>
    /// ServiceWorker interface.
    /// </summary>
    public interface IServiceWorker
    {
        /// <summary>
        /// Gets the ServiceWorker Task.
        /// </summary>
        /// <remarks>It gets very complicated if we want to coordinate with Threads for
        /// .NET 3.5. So we'll just go with 4.0+ for now, and decide whether .NET 3.5 is
        /// worth the effort later. There is a TPL back-port if we want to maintain some
        /// degree of consistency with Tasks if needs be, however.</remarks>
        Task Task { get; }

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
