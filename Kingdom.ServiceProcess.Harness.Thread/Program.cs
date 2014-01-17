using System;
using System.ServiceProcess.Definitions;

namespace Kingdom.ServiceProcess.Harness
{
    /// <summary>
    /// Program class.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Returns the ServiceRunner.
        /// </summary>
        /// <returns></returns>
        private static IServiceRunner GetServiceRunner()
        {
            //TODO: This could get tucked away in a composition-root somewhere.
            Func<IServiceRunner> interactive = () => new InteractiveServiceRunner();

            Func<IServiceRunner> production = () => new ProductionServiceRunner(
                new IServiceBase[] {new ThreadHarnessService(),});

            return Environment.UserInteractive ? interactive() : production();
        }

        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var serviceRunner = GetServiceRunner();

            if (!serviceRunner.TryParse(args)) return;

            serviceRunner.Run();
        }
    }
}
