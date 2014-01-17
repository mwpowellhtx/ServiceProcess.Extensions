using System;
using System.ServiceProcess.Definitions;

namespace Kingdom.ServiceProcess.Harness
{
    /// <summary>
    /// Thread HarnessServiceWorker interface.
    /// </summary>
    internal interface IThreadHarnessServiceWorker : IThreadServiceWorker
    {
    }

    /// <summary>
    /// Thread HarnessServiceWorker class.
    /// </summary>
    internal class ThreadHarnessServiceWorker : AdaptableThreadServiceWorker,
        IThreadHarnessServiceWorker
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal ThreadHarnessServiceWorker()
            : base()
        {
        }

        /// <summary>
        /// Returns a new Thread.
        /// </summary>
        /// <returns></returns>
        protected override System.Threading.Thread NewThread()
        {
            System.Threading.ThreadStart start = () =>
            {
                try
                {
                    var timeout = TimeSpan.FromMilliseconds(100);

                    while (true)
                    {
                        do
                        {
                            if (IsStopRequested()) return;
                        } while (!MayContinue(timeout));

                        //Basically a do-nothing worker Task for harness purposes.
                        System.Threading.Thread.Sleep(timeout);
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    SetCompleted();
                }
            };

            return new System.Threading.Thread(start);
        }
    }
}
