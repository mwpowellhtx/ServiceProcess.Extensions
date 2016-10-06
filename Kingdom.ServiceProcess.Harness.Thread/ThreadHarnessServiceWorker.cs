using System;
using System.Collections.Generic;
using System.ServiceProcess.Definitions;
using System.Threading;

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
        // ReSharper disable once EmptyConstructor
        /// <summary>
        /// Constructor.
        /// </summary>
        internal ThreadHarnessServiceWorker()
        {
        }

        /// <summary>
        /// Returns the Worker Threads.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Thread> GetThreads()
        {
            // ReSharper disable once EmptyGeneralCatchClause
            ThreadStart start = () =>
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
                        Thread.Sleep(timeout);
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

            yield return new Thread(start);
        }
    }
}
