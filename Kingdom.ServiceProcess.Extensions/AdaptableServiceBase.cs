using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace System.ServiceProcess.Definitions
{
    /// <summary>
    /// Provides an adaptable-base-class for a service that will exist as part of a
    /// service application. System.ServiceProcess.ServiceBase must be derived from
    /// when creating a new service class.
    /// </summary>
    /// <remarks>Because we are adapting in Dependency-Injected concerns, if there
    /// are any known interfaces, it would be appropriate to Constructor-Inject them,
    /// here and/or in Derived-Service-Classes.</remarks>
    public abstract class AdaptableServiceBase : ServiceBase, IServiceBase
    {
        /// <summary>
        /// Workers backing field.
        /// </summary>
        private readonly List<IServiceWorker> _workers;

        /// <summary>
        /// Constructor. Is Dependency-Injectable, they are loadable that way,
        /// so feel free to add Constructor-Injected references.
        /// </summary>
        /// <param name="workers"></param>
        protected internal AdaptableServiceBase(IEnumerable<IServiceWorker> workers)
            : base()
        {
            /* Internal-protected: meaning we may activate a new instance from within
             * the assembly, otherwise, allowing extension across assembly boundaries. */

            //Verify a couple of things in Debug mode prior to starting.
            Debug.Assert(workers != null, "Workers are required!");
            Debug.Assert(workers.Any(), "Workers are required!");

            // ReSharper disable once PossibleMultipleEnumeration
            _workers = workers.ToList();
        }

        #region Extensible Concerns

        /* TODO: If we need/want to inject any adaptable-service-base framework code,
         * this is the place to do so. Otherwise, leverage this base class, implied
         * the interface from which it derives, in order to gain a measure of
         * extensibility into a DI model. */

        /// <summary>
        /// Starting event.
        /// </summary>
        public event EventHandler<ServiceStartEventArgs> Starting;

        /// <summary>
        /// Raises the Starting event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void RaiseStarting(ServiceStartEventArgs e)
        {
            if (Starting == null) return;
            Starting(this, e);
        }

        /// <summary>
        /// Started event.
        /// </summary>
        public event EventHandler<ServiceStartEventArgs> Started;

        /// <summary>
        /// Raises the Started event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void RaiseStarted(ServiceStartEventArgs e)
        {
            if (Started == null) return;
            Started(this, e);
        }

        /// <summary>
        /// Stopping event.
        /// </summary>
        public event EventHandler<EventArgs> Stopping;

        /// <summary>
        /// Raises the Stopping event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void RaiseStopping(EventArgs e)
        {
            if (Stopping == null) return;
            Stopping(this, e);
        }

        /// <summary>
        /// Stopped event.
        /// </summary>
        public event EventHandler<EventArgs> Stopped;

        /// <summary>
        /// Raises the Stopped event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void RaiseStopped(EventArgs e)
        {
            if (Stopped == null) return;
            Stopped(this, e);
        }

        /// <summary>
        /// Pausing event.
        /// </summary>
        public event EventHandler<EventArgs> Pausing;

        /// <summary>
        /// Raises the Pausing event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void RaisePausing(EventArgs e)
        {
            if (Pausing == null) return;
            Pausing(this, e);
        }

        /// <summary>
        /// Paused event.
        /// </summary>
        public event EventHandler<EventArgs> Paused;

        /// <summary>
        /// Raises the Paused event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void RaisePaused(EventArgs e)
        {
            if (Paused == null) return;
            Paused(this, e);
        }

        /// <summary>
        /// Continuing event.
        /// </summary>
        public event EventHandler<EventArgs> Continuing;

        /// <summary>
        /// Raises the Continuing event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void RaiseContinuing(EventArgs e)
        {
            if (Continuing == null) return;
            Continuing(this, e);
        }

        /// <summary>
        /// Continued event.
        /// </summary>
        public event EventHandler<EventArgs> Continued;

        /// <summary>
        /// Raises the Continued event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void RaiseContinued(EventArgs e)
        {
            if (Continued == null) return;
            Continued(this, e);
        }

        /// <summary>
        /// PowerEventHandlers backing field.
        /// </summary>
        private readonly List<EventHandler<PowerEventEventArgs>> _powerEventHandlers
            = new List<EventHandler<PowerEventEventArgs>>();

        /// <summary>
        /// PowerEventRequested event.
        /// </summary>
        public event EventHandler<PowerEventEventArgs> PowerEventRequested
        {
            add { lock (_powerEventHandlers) _powerEventHandlers.Add(value); }
            remove { lock (_powerEventHandlers) _powerEventHandlers.Remove(value); }
        }

        /// <summary>
        /// Raises the PowerEventRequested event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void RaisePowerEventRequested(PowerEventEventArgs e)
        {
            lock (_powerEventHandlers)
            {
                foreach (var handler in _powerEventHandlers)
                {
                    //Return at the earliest sign of trouble.
                    if (e.QueryResult) return;

                    if (handler != null) 
                        handler(this, e);
                }
            }
        }

        /// <summary>
        /// ReceivingCustomCommand event.
        /// </summary>
        public event EventHandler<CustomCommandEventArgs> ReceivingCustomCommand;

        /// <summary>
        /// Raises the ReceivingCustomCommand event.
        /// </summary>
        /// <param name="e"></param>
        public virtual void RaiseReceivingCustomCommand(CustomCommandEventArgs e)
        {
            if (ReceivingCustomCommand == null) return;
            ReceivingCustomCommand(this, e);
        }

        /// <summary>
        /// CustomCommandReceived event.
        /// </summary>
        public event EventHandler<CustomCommandEventArgs> CustomCommandReceived;

        /// <summary>
        /// Raises the CustomCommandReceived event.
        /// </summary>
        /// <param name="e"></param>
        public virtual void RaiseCustomCommandReceived(CustomCommandEventArgs e)
        {
            if (CustomCommandReceived == null) return;
            CustomCommandReceived(this, e);
        }

        /// <summary>
        /// SessionChanging event.
        /// </summary>
        public event EventHandler<SessionChangeEventArgs> SessionChanging;

        /// <summary>
        /// Raises the SessionChanging event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void RaiseSessionChanging(SessionChangeEventArgs e)
        {
            if (SessionChanging == null) return;
            SessionChanging(this, e);
        }

        /// <summary>
        /// SessionChanged event.
        /// </summary>
        public event EventHandler<SessionChangeEventArgs> SessionChanged;

        /// <summary>
        /// Raises the SessionChanged event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void RaiseSessionChanged(SessionChangeEventArgs e)
        {
            if (SessionChanged == null) return;
            SessionChanged(this, e);
        }

        #endregion

        #region ServiceBase Concerns

        /// <summary>
        /// Start event handler.
        /// </summary>
        /// <param name="args"></param>
        protected sealed override void OnStart(string[] args)
        {
            //TODO: TBD: Whether Worker-based operations ought not go in the RaiseXYZ methods.
            var e = new ServiceStartEventArgs(args);
            RaiseStarting(e);
            base.OnStart(args);
            _workers.ForEach(x => x.Start());
            RaiseStarted(e);
        }

        /// <summary>
        /// Stop event handler.
        /// </summary>
        protected sealed override void OnStop()
        {
            var e = EventArgs.Empty;
            RaiseStopping(e);
            //TODO: TBD: Will this work? Need to hook up a callback event?
            _workers.ForEach(x => x.Stop());
            WaitForWorkers(_workers.ToArray());
            base.OnStop();
            RaiseStopped(e);
        }

        /// <summary>
        /// Waits for ServiceWorkers to finish.
        /// </summary>
        /// <param name="workers"></param>
        protected abstract void WaitForWorkers(params IServiceWorker[] workers);

        /// <summary>
        /// Pause event handler.
        /// </summary>
        protected sealed override void OnPause()
        {
            var e = EventArgs.Empty;
            RaisePausing(e);
            base.OnPause();
            _workers.ForEach(x => x.Pause());
            RaisePaused(e);
        }

        /// <summary>
        /// Continue event handler.
        /// </summary>
        protected sealed override void OnContinue()
        {
            var e = EventArgs.Empty;
            RaiseContinuing(e);
            _workers.ForEach(x => x.Continue());
            base.OnContinue();
            RaiseContinued(e);
        }

        /// <summary>
        /// PowerEvent event handler.
        /// </summary>
        /// <param name="powerStatus"></param>
        /// <returns>The needs of your application determine what value to
        /// return. For example, if a QuerySuspend broadcast status is passed,
        /// you could cause your application to reject the query by returning
        /// false.</returns>
        protected sealed override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            //Bypasses the base-class altogether.
            //return base.OnPowerEvent(powerStatus);

            var e = new PowerEventEventArgs(powerStatus);
            RaisePowerEventRequested(e);
            return e.QueryResult;
        }

        /// <summary>
        /// CustomCommand event handler.
        /// </summary>
        /// <param name="command"></param>
        protected sealed override void OnCustomCommand(int command)
        {
            var e = new CustomCommandEventArgs(command);
            RaiseReceivingCustomCommand(e);
            base.OnCustomCommand(e.Command);
            RaiseCustomCommandReceived(e);
        }

        /// <summary>
        /// SessionChange event handler.
        /// </summary>
        /// <param name="changeDescription"></param>
        protected sealed override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            var e = new SessionChangeEventArgs(changeDescription);
            RaiseSessionChanging(e);
            base.OnSessionChange(e.ChangeDescription);
            RaiseSessionChanged(e);
        }

        /// <summary>
        /// Requests additional time for a pending operation.
        /// </summary>
        /// <param name="timeSpan">The requested time as a TimeSpan.</param>
        /// <exception cref="InvalidOperationException">The service is not in a
        /// pending state.</exception>
        [ComVisible(false)]
        public void RequestAdditionalTime(TimeSpan timeSpan)
        {
            base.RequestAdditionalTime((int)timeSpan.TotalMilliseconds);
        }

        #endregion
    }
}
