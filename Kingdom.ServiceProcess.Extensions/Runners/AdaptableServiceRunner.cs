namespace System.ServiceProcess.Runners
{
    /* TODO: It would be better to perhaps separate interfaces from implementations,
     * especially since that would make wiring up DI easier I think. */
    /// <summary>
    /// ServiceRunner interface.
    /// </summary>
    public interface IServiceRunner : IDisposable
    {
        /// <summary>
        /// Parses the Args.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Whether parsing was successful.</returns>
        bool TryParse(params string[] args);

        /// <summary>
        /// Runs the Service.
        /// </summary>
        void Run();
    }

    /// <summary>
    /// AdaptableServiceRunner class.
    /// </summary>
    public abstract class AdaptableServiceRunner : IServiceRunner
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        protected AdaptableServiceRunner()
        {
        }

        #region ServiceRunner Members

        //TODO: These may go better as abstract.
        /// <summary>
        /// Parses the Args.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Whether parsing was successful.</returns>
        public abstract bool TryParse(params string[] args);

        /// <summary>
        /// Runs the Service.
        /// </summary>
        public abstract void Run();

        #endregion

        #region Disposable Members

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion
    }
}
