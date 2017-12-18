namespace SAF.Data.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics;

    /// <summary>
    /// Provides a shared connection for data commands that can be explicitly opened and closed.
    /// </summary>
    /// <remarks>.NET typed datasets open and close a unique database connection with 
    /// each command, without a public interface to change the connection other than the 
    /// class settings. This class acts as an extension to a typed dataset and allows a 
    /// connection to remain open at the discretion of the caller for all commands. It
    /// also provides events for each connection that return SQL print and error 
    /// messages back to the caller.</remarks>
    public class SqlCommandAdapter : CommandAdapterBase<SqlConnection, SqlCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCommandAdapter"/> class with the specified connection 
        /// and command collection.
        /// </summary>
        /// <param name="commandCollection">The command collection to be managed.</param>
        public SqlCommandAdapter(IList<SqlCommand> commandCollection)
            : base(commandCollection)
        {
        }

        /// <summary>
        /// Occurs when a message is returned from the connection.
        /// </summary>
        public event SqlInfoMessageEventHandler MessageReturned;

        /// <summary>
        /// Enables tracing for this command adapter.
        /// </summary>
        /// <param name="enable"><c>true</c> to enable tracing; otherwise <c>false</c>.</param>
        protected override void EnableTrace(bool enable)
        {
            base.EnableTrace(enable);

            if (enable)
            {
                this.MessageReturned += TraceAdapterMessages;
            }
            else
            {
                this.MessageReturned -= TraceAdapterMessages;
            }
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        /// <param name="disposing">A flag indicating whether this object is disposing.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (this.MessageReturned != null)
                {
                    this.MessageReturned -= TraceAdapterMessages;
                }
            }
        }

        /// <summary>
        /// Initializes the specified connection.
        /// </summary>
        /// <param name="name">The name of the adapter.</param>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="connection">The connection to initialize.</param>
        protected override void InitializeConnection(string name, string connectionString, SqlConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            connection.ConnectionString = connectionString;
            connection.FireInfoMessageEventOnUserErrors = true;
            connection.InfoMessage += this.TriggerMessageReturned;
        }

        /// <summary>
        /// Performs any tasks required to unregister the shared connection.
        /// </summary>
        /// <param name="connection">The connection to unregister.</param>
        protected override void UnregisterConnection(SqlConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            base.UnregisterConnection(connection);

            connection.InfoMessage -= this.TriggerMessageReturned;
        }

        /// <summary>
        /// Trigger the MessageReturned event.
        /// </summary>
        /// <param name="e">SqlInfoMessageEventArgs to pass</param>
        protected virtual void OnMessageReturned(SqlInfoMessageEventArgs e)
        {
            SqlInfoMessageEventHandler temp = this.MessageReturned;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// Traces adapter messages returned by the command adapters.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event data associated with the event.</param>
        private static void TraceAdapterMessages(object sender, SqlInfoMessageEventArgs e)
        {
            // TODO: obfuscate passwords.
            Trace.TraceInformation("[{0}] {1}: {2}", sender, e.Source, e.Message);

            foreach (SqlError error in e.Errors)
            {
                Trace.TraceInformation("[{0}] {1}: {2}", sender, e.Source, error);
            }
        }

        /// <summary>
        /// Triggers the MessageReturned event for this class.
        /// </summary>
        /// <param name="sender">The sender of the originating event.</param>
        /// <param name="e">The <see cref="SqlInfoMessageEventArgs"/> associated with the event.</param>
        private void TriggerMessageReturned(object sender, SqlInfoMessageEventArgs e)
        {
            this.OnMessageReturned(e);
        }
    }
}
