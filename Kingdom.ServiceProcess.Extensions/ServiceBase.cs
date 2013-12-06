using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.ServiceProcess.Definitions
{
    /// <summary>
    /// Fills a gap where ServiceBase is an eligible-base-class but which itself
    /// does not implement an interface. This makes it easier to wire up with a
    /// Dependency Injection composition root. As long as we utilize the service
    /// extensibility framework, this also makes it easier to identify just the
    /// services we want: the ones implementing this interface.
    /// </summary>
    /// <remarks>Basically this was lifted straight from ServiceBase. We can
    /// elaborate on this one further if we need to.</remarks>
    public interface IServiceBase : IComponent
    {
        #region ServiceBase Concerns

        /// <summary>
        /// Indicates whether to report Start, Stop, Pause, and Continue commands
        /// in the event log.
        /// </summary>
        /// <value>true to report information in the event log; otherwise, false.</value>
        [DefaultValue(true)]
        [ServiceProcessDescription("SBAutoLog")]
        bool AutoLog { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the service can handle
        /// notifications of computer power status changes.
        /// </summary>
        /// <value>true if the service handles the computer power status changes
        /// indicated in the System.ServiceProcess.PowerBroadcastStatus class,
        /// otherwise, false.</value>
        /// <exception cref="InvalidOperationException">This property is modified
        /// after the service was started.</exception>
        [DefaultValue(false)]
        bool CanHandlePowerEvent { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the service can handle
        /// session change events received from a Terminal Server session.
        /// </summary>
        /// <value>true if the service can handle Terminal Server session change
        /// events; otherwise, false.</value>
        /// <exception cref="InvalidOperationException">This property is modified
        /// after the service was started.</exception>
        [ComVisible(false)]
        [DefaultValue(false)]
        bool CanHandleSessionChangeEvent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the service can be paused and
        /// resumed.
        /// </summary>
        /// <value>true if the service can be paused; otherwise, false.</value>
        /// <exception cref="InvalidOperationException">The service has already
        /// been started. The System.ServiceProcess.ServiceBase.CanPauseAndContinue
        /// property cannot be changed once the service has started.</exception>
        [DefaultValue(false)]
        bool CanPauseAndContinue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the service should be notified
        /// when the system is shutting down.
        /// </summary>
        /// <value>true if the service should be notified when the system is
        /// shutting down; otherwise, false.</value>
        /// <exception cref="InvalidOperationException">The service has already
        /// been started. The System.ServiceProcess.ServiceBase.CanShutdown property cannot be changed once the service has started.</exception>
        [DefaultValue(false)]
        bool CanShutdown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the service can be stopped
        /// once it has started.
        /// </summary>
        /// <value>true if the service can be stopped and the
        /// System.ServiceProcess.ServiceBase.OnStop() method called;
        /// otherwise, false.</value>
        /// <exception cref="InvalidOperationException">The service has already
        /// been started. The System.ServiceProcess.ServiceBase.CanStop property
        /// cannot be changed once the service has started.</exception>
        [DefaultValue(true)]
        bool CanStop { get; set; }

        /// <summary>
        /// Gets an event log you can use to write notification of service
        /// command calls, such as Start and Stop, to the Application event log.
        /// </summary>
        /// <value>An System.Diagnostics.EventLog instance whose source is
        /// registered to the Application log.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        EventLog EventLog { get; }

        /// <summary>
        /// Gets or sets the exit code for the service.
        /// </summary>
        /// <value>The exit code for the service.</value>
        [ComVisible(false)]
        int ExitCode { get; set; }

        /// <summary>
        /// Gets or sets the short name used to identify the service to the system.
        /// </summary>
        /// <value>The name of the service.</value>
        /// <exception cref="InvalidOperationException">The service has already been
        /// started. The System.ServiceProcess.ServiceBase.ServiceName property cannot
        /// be changed once the service has started.</exception>
        /// <exception cref="ArgumentException">The specified name is a zero-length
        /// string or is longer than System.ServiceProcess.ServiceBase.MaxNameLength,
        /// or the specified name contains forward slash or backslash characters.</exception>
        [ServiceProcessDescription("SBServiceName")]
        [TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        string ServiceName { get; set; }

        /// <summary>
        /// Requests additional time for a pending operation.
        /// </summary>
        /// <param name="milliseconds">The requested time in milliseconds.</param>
        /// <exception cref="InvalidOperationException">The service is not in a
        /// pending state.</exception>
        [ComVisible(false)]
        void RequestAdditionalTime(int milliseconds);

        /// <summary>
        /// Requests additional time for a pending operation.
        /// </summary>
        /// <param name="timeSpan">The requested time as a TimeSpan.</param>
        /// <exception cref="InvalidOperationException">The service is not in a
        /// pending state.</exception>
        [ComVisible(false)]
        void RequestAdditionalTime(TimeSpan timeSpan);

        /// <summary>
        /// Registers the command handler and starts the service.
        /// </summary>
        /// <param name="argCount">The number of arguments in the argument
        /// array.</param>
        /// <param name="argPointer">An System.IntPtr structure that points
        /// to an array of arguments.</param>
        [ComVisible(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void ServiceMainCallback(int argCount, IntPtr argPointer);

        /// <summary>
        /// Stops the executing service.
        /// </summary>
        void Stop();

        #endregion

        #region Extensible Concerns

        /// <summary>
        /// Starting event.
        /// </summary>
        event EventHandler<ServiceStartEventArgs> Starting;

        /// <summary>
        /// Started event.
        /// </summary>
        event EventHandler<ServiceStartEventArgs> Started;

        /// <summary>
        /// Stopping event.
        /// </summary>
        event EventHandler<EventArgs> Stopping;

        /// <summary>
        /// Stopped event.
        /// </summary>
        event EventHandler<EventArgs> Stopped;

        /// <summary>
        /// Pausing event.
        /// </summary>
        event EventHandler<EventArgs> Pausing;

        /// <summary>
        /// Paused event.
        /// </summary>
        event EventHandler<EventArgs> Paused;

        /// <summary>
        /// Continuing event.
        /// </summary>
        event EventHandler<EventArgs> Continuing;

        /// <summary>
        /// Continued event.
        /// </summary>
        event EventHandler<EventArgs> Continued;

        /// <summary>
        /// PowerEventRequested event.
        /// </summary>
        event EventHandler<PowerEventEventArgs> PowerEventRequested;

        /// <summary>
        /// ReceivingCustomCommand event.
        /// </summary>
        event EventHandler<CustomCommandEventArgs> ReceivingCustomCommand;

        /// <summary>
        /// CustomCommandReceived event.
        /// </summary>
        event EventHandler<CustomCommandEventArgs> CustomCommandReceived;

        /// <summary>
        /// SessionChanging event.
        /// </summary>
        event EventHandler<SessionChangeEventArgs> SessionChanging;

        /// <summary>
        /// SessionChanged event.
        /// </summary>
        event EventHandler<SessionChangeEventArgs> SessionChanged;

        #endregion
    }
}
