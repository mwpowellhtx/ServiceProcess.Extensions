using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess.Definitions;
using System.ServiceProcess.Helpers;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceProcess.Runners
{
    /// <summary>
    /// InteractiveServiceRunner interface.
    /// </summary>
    /// <remarks>Recommend using Environment.UserInteractive in your favorite
    /// DI container provider(s) in order to determine which ServiceRunner to
    /// provide.</remarks>
    public interface IInteractiveServiceRunner : IServiceRunner
    {
    }

    /// <summary>
    /// InteractiveServiceRunnerBase class.
    /// </summary>
    public abstract class InteractiveServiceRunnerBase : AdaptableServiceRunner, IInteractiveServiceRunner
    {
        /// <summary>
        /// Workers backing field.
        /// </summary>
        private readonly IEnumerable<IServiceWorker> _workers;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="workers"></param>
        /// <remarks>Allows there to be DI connections.</remarks>
        protected InteractiveServiceRunnerBase(IEnumerable<IServiceWorker> workers)
            : base()
        {
            _workers = workers;
        }

        #region ServiceRunner Members

        /// <summary>
        /// Args backing field.
        /// </summary>
        private IEnumerable<string> _args;

        /// <summary>
        /// Parses the Args.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override bool TryParse(params string[] args)
        {
            //Or in this case, reserves them for later parsing.
            _args = args;
            return true;
        }

        /// <summary>
        /// Gets whether to RunInstaller.
        /// </summary>
        protected abstract bool RunInstaller { get; }

        /// <summary>
        /// Gets whether to RunUninstaller.
        /// </summary>
        protected abstract bool RunUninstaller { get; }

        /// <summary>
        /// Runs the Service.
        /// </summary>
        /// <param name="workers"></param>
        /// <param name="args"></param>
        protected virtual void RunService(List<IServiceWorker> workers, string[] args)
        {
            var running = true;

            //Args is a straight-up array: as contrasted with params.
            workers.ForEach(x => x.Start(args));

            var writer = Console.Out;

            var keyedActions = new Dictionary<ConsoleKey, Action<List<IServiceWorker>>>()
            {
                {
                    ConsoleKey.Q, w =>
                    {
                        //Signal not running, and signal the workers.
                        running = false;
                        w.ForEach(x => x.Stop());
                        writer.WriteLine("Running signaled.");
                    }
                },
                {
                    ConsoleKey.P, w =>
                    {
                        //Just signal the workers.
                        w.ForEach(x => x.Pause());
                        writer.WriteLine("Workers paused.");
                    }
                },
                {
                    ConsoleKey.R, w =>
                    {
                        //Just signal the workers.
                        w.ForEach(x => x.Continue());
                        writer.WriteLine("Workers resumed.");
                    }
                },
            };

            while (running)
            {
                writer.WriteLine("Enter [P]ause, [R]esume, or [Q]uit.");

                //Read the Console Key, but do not display it.
                var info = Console.ReadKey(true);

                if (!keyedActions.Keys.Contains(info.Key)) continue;

                var action = keyedActions[info.Key];

                action(workers);
            }

            //Wait for all the Workers Tasks to be completed.
            Task.WaitAll(workers.Select(x => x.Task).ToArray());
        }

        //TODO: May want to make this one sealed-override.
        /// <summary>
        /// Runs the Service.
        /// </summary>
        public override void Run()
        {
            //TODO: How about for conflicts?
            if (RunInstaller)
                ServiceInstallHelper.Instance.Install();
            else if (RunUninstaller)
                ServiceUninstallHelper.Instance.Uninstall();
            else
                RunService(_workers.ToList(), _args.ToArray());
            /* TODO: Or actions other than RunService:
             * i.e. Restart or other Service-control? */
        }

        #endregion
    }
}
