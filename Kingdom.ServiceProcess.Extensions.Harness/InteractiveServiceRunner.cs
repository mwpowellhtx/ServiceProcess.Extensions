using System;
using System.ServiceProcess.Definitions;
using System.ServiceProcess.Runners;

namespace Kingdom.ServiceProcess.Extensions
{
    /// <summary>
    /// InteractiveServiceRunner class.
    /// </summary>
    internal class InteractiveServiceRunner : InteractiveServiceRunnerBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public InteractiveServiceRunner()
            : base(new IServiceWorker[]
            {
                new HarnessServiceWorker(),
                new HarnessServiceWorker(),
                new HarnessServiceWorker(),
            })
        {
        }

        /// <summary>
        /// Tries to Parse the args.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override bool TryParse(params string[] args)
        {
            /* Really and truly, this is all a command line parser needs to be,
             * but from time to time it's helpful to want to abstract it out into
             * more useful scaffolding, i.e. through NDesk.Option. */
            foreach (var arg in args)
            {
                switch (arg.ToLower())
                {
                    case "/install":
                        _runInstaller = true;
                        break;

                    case "/uninstall":
                        _runUninstaller = true;
                        break;
                }
            }

            //TODO: Ignores extraneous options.
            var result = base.TryParse(args);

            //TODO: This is where something like a ConsoleManager might be useful.
            if (!result)
                Console.Out.WriteLine("Invalid arguments.");

            return result;
        }

        /// <summary>
        /// RunInstaller backing field.
        /// </summary>
        private bool _runInstaller = false;

        /// <summary>
        /// RunUninstaller backing field.
        /// </summary>
        private bool _runUninstaller = false;

        /// <summary>
        /// Gets whether to RunInstaller.
        /// </summary>
        protected override bool RunInstaller
        {
            get { return _runInstaller; }
        }

        /// <summary>
        /// Gets whether to RunUninstaller.
        /// </summary>
        protected override bool RunUninstaller
        {
            get { return _runUninstaller; }
        }
    }
}
