using System.Collections.Generic;

namespace System.ServiceProcess.Definitions
{
    /// <summary>
    /// ServiceStartEventArgs EventArgs class.
    /// </summary>
    public class ServiceStartEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the Args.
        /// </summary>
        public IEnumerable<string> Args { get; private set; } 

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="args"></param>
        internal ServiceStartEventArgs(params string[] args)
            : base()
        {
            Args = args;
        }
    }

    /// <summary>
    /// PowerEventEventArgs EventArgs class.
    /// </summary>
    public class PowerEventEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the PowerStatus.
        /// </summary>
        public PowerBroadcastStatus PowerStatus { get; private set; }

        /// <summary>
        /// Gets or sets the QueryResult.
        /// </summary>
        /// <value>false by default, meaning query-rejected.</value>
        public bool QueryResult { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="powerStatus"></param>
        internal PowerEventEventArgs(PowerBroadcastStatus powerStatus)
            : base()
        {
            PowerStatus = powerStatus;
            QueryResult = false;
        }
    }

    /// <summary>
    /// CustomCommandEventArgs EventArgs class.
    /// </summary>
    public class CustomCommandEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the Command.
        /// </summary>
        public int Command { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="command"></param>
        internal CustomCommandEventArgs(int command)
            : base()
        {
            Command = command;
        }
    }

    /// <summary>
    /// SessionChangeEventArgs EventArgs class.
    /// </summary>
    public class SessionChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the ChangeDescription.
        /// </summary>
        public SessionChangeDescription ChangeDescription { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="changeDescription"></param>
        internal SessionChangeEventArgs(SessionChangeDescription changeDescription)
            : base()
        {
            ChangeDescription = changeDescription;
        }
    }
}
