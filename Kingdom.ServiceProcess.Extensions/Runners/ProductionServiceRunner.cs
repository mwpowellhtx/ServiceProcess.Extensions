using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace System.ServiceProcess.Definitions
{
    /// <summary>
    /// ProductionServiceRunner interface.
    /// </summary>
    public interface IProductionServiceRunner : IServiceRunner
    {
    }

    /// <summary>
    /// ProductionServiceRunner class.
    /// </summary>
    /// <remarks>This is production scaffolding that supports running
    /// the services themselves in production.</remarks>
    public class ProductionServiceRunner : AdaptableServiceRunner, IProductionServiceRunner
    {
        /// <summary>
        /// Services backing field.
        /// </summary>
        private readonly IEnumerable<IServiceBase> _services;

        /// <summary>
        /// Gets the Services.
        /// </summary>
        private IEnumerable<AdaptableServiceBase> Services
        {
            get { return _services.Cast<AdaptableServiceBase>().ToList(); }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="services"></param>
        /// <remarks>Allows there to be DI connections.</remarks>
        public ProductionServiceRunner(IEnumerable<IServiceBase> services)
        {
            _services = services;
        }

        #region ServiceRunner Members

        /// <summary>
        /// Parses the Args.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override bool TryParse(params string[] args)
        {
            return true;
        }

        /// <summary>
        /// Runs the Service.
        /// </summary>
        public override void Run()
        {
            // ReSharper disable once CoVariantArrayConversion
            ServiceBase.Run(Services.ToArray());
        }

        #endregion
    }
}
