using System;
using System.ServiceProcess.Definitions;
using System.Threading;
using System.Threading.Tasks;

namespace Kingdom.ServiceProcess.Extensions
{
    /// <summary>
    /// HarnessServiceWorker interface.
    /// </summary>
    internal interface IHarnessServiceWorker : IServiceWorker
    {
    }

    /// <summary>
    /// HarnessServiceWorker class.
    /// </summary>
    internal class HarnessServiceWorker : AdaptableServiceWorker, IHarnessServiceWorker
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal HarnessServiceWorker()
            : base()
        {
        }

        /// <summary>
        /// Returns a new Task.
        /// </summary>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        protected override Task NewTask(TaskScheduler scheduler)
        {
            Action start = () =>
            {
                while (true)
                {
                    do
                    {
                        if (CancelToken.IsCancellationRequested) return;
                    } while (!MayContinue(TimeSpan.FromMilliseconds(100)));

                    //Basically a do-nothing worker Task for harness purposes.
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                }
            };

            return new Task(start, CancelToken.Token);
        }
    }
}
