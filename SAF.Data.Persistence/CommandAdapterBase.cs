namespace SAF.Data.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Linq;

    using SAF.Core;

    /// <summary>
    /// Base class for shared connections.
    /// </summary>
    /// <typeparam name="TConn">The type of <see cref="DbConnection"/> to share.</typeparam>
    /// <typeparam name="TCmd">The type of <see cref="DbCommand"/>s associated with the connection.</typeparam>
    public abstract class CommandAdapterBase<TConn, TCmd> : ICommandAdapter
        where TConn : DbConnection, new()
        where TCmd : DbCommand
    {
        #region Constants

        #endregion

        #region Private / Protected Variables

        /// <summary>
        /// A collection of commands associated with the shared connection.
        /// </summary>
        private readonly IList<TCmd> commandCollection;

        /// <summary>
        /// The connection shared by all the commands.
        /// </summary>
        private readonly TConn sharedConnection;

        /// <summary>
        /// Indicates whether adapter tracing is enabled.
        /// </summary>
        private bool traceEnabled;

        /// <summary>
        /// Indicates whether the command adapter has been initialized.
        /// </summary>
        private bool initialized;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAdapterBase{TConn,TCmd}"/> class.
        /// </summary>
        /// <param name="commandCollection">The command collection to manage.</param>
        protected CommandAdapterBase(IEnumerable<TCmd> commandCollection)
        {
            if (commandCollection == null)
            {
                throw new ArgumentNullException("commandCollection");
            }

            this.commandCollection = new List<TCmd>(commandCollection);
            this.sharedConnection = new TConn();
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the shared connection state changes.
        /// </summary>
        public event StateChangeEventHandler ConnectionStateChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of this connection.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the connection string for this shared connection.
        /// </summary>
        public string ConnectionString
        {
            get 
            { 
                return this.sharedConnection.ConnectionString; 
            }

            private set 
            { 
                this.sharedConnection.ConnectionString = value;

                Trace.TraceInformation(
                    "[{0}] Setting connection to '[{1}].[{2}]'",
                    this, 
                    this.sharedConnection.DataSource, 
                    this.sharedConnection.Database);
            }
        }

        /// <summary>
        /// Gets the state of the connection.
        /// </summary>
        public ConnectionState State
        {
            get { return this.sharedConnection.State; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether adapter messages will be traced.
        /// </summary>
        public bool TraceMessages
        { 
            get
            {
                return this.traceEnabled; 
            }

            set
            {
                this.EnableTrace(value);
                this.traceEnabled = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a transaction adapter with the specified name.
        /// </summary>
        /// <param name="name">The name of the adapter.</param>
        /// <param name="connectionString">The connection string for the adapter to use.</param>
        public void Initialize(string name, string connectionString)
        {
            this.initialized = false;
            this.sharedConnection.StateChange -= this.TriggerConnectionStateChange;
            this.UnregisterConnection(this.sharedConnection);

            this.Name = name;
            this.ConnectionString = connectionString;
            this.InitializeConnection(name, connectionString, this.sharedConnection);
            this.sharedConnection.StateChange += this.TriggerConnectionStateChange;
            this.SetCommandConnections();
            this.initialized = true;
        }

        /// <summary>
        /// Opens the transaction connection if it is not already open.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Cannot open a connection without specifying a data source or server. The connection is already open.
        /// </exception>
        /// <exception cref="System.Data.SqlClient.SqlException">
        /// A connection-level error occurred while opening the connection. If the System.Data.SqlClient.SqlException.Number property
        /// contains the value 18487 or 18488, this indicates that the specified password has expired or must be reset. See the 
        /// System.Data.SqlClient.SqlConnection.ChangePassword(System.String,System.String) method for more information.
        /// </exception>
        [DebuggerHidden]
        public void Open()
        {
            if (!this.initialized)
            {
                throw new InvalidOperationException("The command adapter has not been initialized.");
            }

            lock (this.sharedConnection)
            {
                if (ConnectionState.Open != (this.sharedConnection.State & ConnectionState.Open))
                {
                    this.sharedConnection.Open();
                }
            }
        }

        /// <summary>
        /// Sets the command timeout for a specific command.
        /// </summary>
        /// <param name="commandText">The command text that matches the command</param>
        /// <param name="timeout">The number of seconds to set as the timeout</param>
        [DebuggerHidden]
        public void SetCommandTimeout(string commandText, int timeout)
        {
            if (String.IsNullOrEmpty(commandText))
            {
                throw new ArgumentNullException("commandText");
            }

            foreach (TCmd command in this.commandCollection)
            {
                if (command.CommandText.ToLower() == commandText.ToLower())
                {
                    command.CommandTimeout = timeout;
                }
            }
        }

        /// <summary>
        /// Gets the return value of the specified stored procedure.
        /// </summary>
        /// <param name="storedProcedure">The name (including namespace) of the stored
        /// procedure to get the return value for.</param>
        /// <returns>The return value of the specified stored procedure, if it exists, or null.</returns>
        public object GetReturnValue(string storedProcedure)
        {
            if (String.IsNullOrEmpty(storedProcedure))
            {
                throw new ArgumentNullException("storedProcedure");
            }

            TCmd command =
                this.commandCollection.FirstOrDefault(
                    x => x.CommandType == CommandType.StoredProcedure
                        && 0 == String.Compare(x.CommandText, storedProcedure, StringComparison.OrdinalIgnoreCase));

            if (command == null)
            {
                throw new ArgumentException(
                    String.Format("The procedure '{0}' does not exist in this adapter.", storedProcedure));
            }

            return (from DbParameter param in command.Parameters
                    where param.Direction == ParameterDirection.ReturnValue && param.Value != null
                    select param.Value).FirstOrDefault();
        }

        /// <summary>
        /// Closes the transaction connection.
        /// </summary>
        [DebuggerHidden]
        public void Close()
        {
            this.sharedConnection.Close();
        }

        /// <summary>
        /// Returns a string representation of this shared connection.
        /// </summary>
        /// <returns>The name of this shared connection.</returns>
        [DebuggerHidden]
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Disposes of this object.
        /// </summary>
        [DebuggerHidden]
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Enables tracing for this command adapter.
        /// </summary>
        /// <param name="enable"><c>true</c> to enable tracing; otherwise <c>false</c>.</param>
        protected virtual void EnableTrace(bool enable)
        {
            if (enable)
            {
                this.ConnectionStateChanged += TraceConnectionStateChange;
            }
            else
            {
                this.ConnectionStateChanged -= TraceConnectionStateChange;
            }
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        /// <param name="disposing">A flag indicating whether the object is disposing.</param>
        [DebuggerHidden]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clean up all IDisposable objects.
                this.Close();

                if (this.sharedConnection != null)
                {
                    this.sharedConnection.StateChange -= this.TriggerConnectionStateChange;
                    this.UnregisterConnection(this.sharedConnection);
                    this.sharedConnection.Dispose();
                }

                if (this.ConnectionStateChanged != null)
                {
                    this.ConnectionStateChanged -= TraceConnectionStateChange;
                }
            }
        }

        /// <summary>
        /// Initializes the connection for this command adapter.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        /// <param name="connectionString">The connection string to apply.</param>
        /// <param name="connection">The connection to initialize.</param>
        protected abstract void InitializeConnection(string name, string connectionString, TConn connection);

        /// <summary>
        /// Performs any tasks required to unregister the shared connection.
        /// </summary>
        /// <param name="connection">The connection to unregister.</param>
        protected virtual void UnregisterConnection(TConn connection)
        {
        }

        /// <summary>
        /// Traces information from a connection state change.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event data associated with the event.</param>
        private static void TraceConnectionStateChange(object sender, StateChangeEventArgs e)
        {
            Trace.TraceInformation(
                "[{0}] connection state changed from {1} to {2}.", sender, e.OriginalState, e.CurrentState);

            if (e.CurrentState == ConnectionState.Open)
            {
                CommandAdapterBase<TConn, TCmd> adapter = sender as CommandAdapterBase<TConn, TCmd>;
                Trace.TraceInformation(
                    "Connected to [{0}].[{1}] ({2}).",
                    adapter.sharedConnection.DataSource,
                    adapter.sharedConnection.Database,
                    adapter.sharedConnection.ServerVersion);
            }
        }

        /// <summary>
        /// Trigger the ConnectionStateChanged event.
        /// </summary>
        /// <param name="e">StateChangeEventArgs to pass</param>
        [DebuggerHidden]
        private void OnConnectionStateChanged(StateChangeEventArgs e)
        {
            StateChangeEventHandler temp = this.ConnectionStateChanged;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// Sets the connection string for the specified command.
        /// </summary>
        [DebuggerHidden]
        private void SetCommandConnections()
        {
            foreach (TCmd command in this.commandCollection)
            {
                command.Connection = this.sharedConnection;
            }
        }

        /// <summary>
        /// Triggers the ConnectionStateChanged event.
        /// </summary>
        /// <param name="sender">The sender of the encapsulated <see cref="StateChangeEventArgs"/> event.</param>
        /// <param name="e">The <see cref="StateChangeEventArgs"/> associated with the event.</param>
        [DebuggerHidden]
        private void TriggerConnectionStateChange(object sender, StateChangeEventArgs e)
        {
            this.OnConnectionStateChanged(e);
        }
    }
}
