using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess.Helpers;

// ReSharper disable once CheckNamespace
namespace System.ServiceProcess.Definitions
{
    //TODO: Might inject the kind of ServiceWorker interface as a generic argument.
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

            WaitForWorkers(workers.ToArray());
        }

        /// <summary>
        /// Waits for the ServiceWorkers.
        /// </summary>
        /// <param name="workers"></param>
        protected abstract void WaitForWorkers(params IServiceWorker[] workers);

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

    /// <summary>
    /// ServiceRunnerExtensionMethods class.
    /// </summary>
    public static class ServiceRunnerExtensionMethods
    {
        /// <summary>
        /// Verifies the ServiceWorkers.
        /// </summary>
        /// <typeparam name="TServiceWorker"></typeparam>
        /// <param name="workers"></param>
        [Conditional("DEBUG")]
        public static void VerifyServiceWorkers<TServiceWorker>(this IEnumerable<IServiceWorker> workers)
            where TServiceWorker : IServiceWorker
        {
            //Should only run when UserInteractive.
            if (!Environment.UserInteractive) return;

            /* Since we shouldn't be deploying Debug mode into services.
             * And since the errors are debug. */
            Debug.Assert(workers != null, @"Workers should not be null.");
            Debug.Assert(workers.Any(), @"There should be one or more workers.");
            Debug.Assert(workers.All(x => x is TServiceWorker),
                string.Format(@"Workers should all be of type {0}.", typeof (TServiceWorker)));
        }
    }
}
