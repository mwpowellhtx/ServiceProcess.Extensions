using System;
using System.ServiceProcess.Definitions;
using System.ServiceProcess.Runners;

namespace Kingdom.ServiceProcess.Extensions
{
    /// <summary>
    /// Program class.
    /// </summary>
    /// <remarks>Thus far the test harness is here to prove it out in
    /// .NET 4.0. We can consider a .NET 3.5 if we need to.</remarks>
    class Program
    {
        /// <summary>
        /// Returns the ServiceRunner.
        /// </summary>
        /// <returns></returns>
        private static IServiceRunner GetServiceRunner()
        {
            Func<IServiceRunner> interactive = () => new InteractiveServiceRunner();

            Func<IServiceRunner> production = () => new ProductionServiceRunner(
                new IServiceBase[] {new HarnessService(),});

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
