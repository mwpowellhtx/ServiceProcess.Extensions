using System;
using System.ServiceProcess.Definitions;
using System.Threading;
using System.Threading.Tasks;

namespace Kingdom.ServiceProcess.Harness
{
    /// <summary>
    /// Task parallel HarnessServiceWorker interface.
    /// </summary>
    internal interface ITaskParallelHarnessServiceWorker : ITaskParallelServiceWorker
    {
    }

    /// <summary>
    /// Task parallel HarnessServiceWorker class.
    /// </summary>
    internal class TaskParallelHarnessServiceWorker : AdaptableTaskParallelServiceWorker,
        ITaskParallelHarnessServiceWorker
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal TaskParallelHarnessServiceWorker()
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
