namespace SAF.Data.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Data.OleDb;
    using System.Diagnostics;

    /// <summary>
    /// Provides a shared connection for data commands that can be explicitly opened and closed.
    /// </summary>
    /// <remarks>.NET typed datasets open and close a unique database connection with 
    /// each command, without a public interface to change the connection other than the 
    /// class settings. This class acts as an extension to a typed dataset and allows a 
    /// connection to remain open at the discretion of the caller for all commands. It
    /// also provides events for each connection that return OLEDB print and error 
    /// messages back to the caller.</remarks>
    public class OleDbCommandAdapter : CommandAdapterBase<OleDbConnection, OleDbCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OleDbCommandAdapter"/> class with the specified 
        /// OLEDB connection and command collection.
        /// </summary>
        /// <param name="commandCollection">The <see cref="OleDbCommand"/> collection to manage.</param>
        public OleDbCommandAdapter(IEnumerable<OleDbCommand> commandCollection)
            : base(commandCollection)
        {
        }

        /// <summary>
        /// Occurs when a message is returned from the connection.
        /// </summary>
        public event OleDbInfoMessageEventHandler MessageReturned;

        /// <summary>
        /// Disposes this object.
        /// </summary>
        /// <param name="disposing">A flag indicating whether this object is disposing.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (this.MessageReturned == null)
                {
                    this.MessageReturned -= TraceAdapterMessages;
                }
            }
        }

        /// <summary>
        /// Initializes the specified connection for this command adapter.
        /// </summary>
        /// <param name="name">The name of the adapter.</param>
        /// <param name="connectionString">The connection string to apply to the connection.</param>
        /// <param name="connection">The connection to initialize.</param>
        protected override void InitializeConnection(string name, string connectionString, OleDbConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            connection.ConnectionString = connectionString;
            connection.InfoMessage += this.TriggerMessageReturned;
        }

        /// <summary>
        /// Unregisters the specified connection from this command adapter.
        /// </summary>
        /// <param name="connection">The connection to unregister.</param>
        protected override void UnregisterConnection(OleDbConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            base.UnregisterConnection(connection);

            connection.InfoMessage -= this.TriggerMessageReturned;
        }

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
        /// Trigger the MessageReturned event.
        /// </summary>
        /// <param name="e">The <see cref="OleDbInfoMessageEventArgs"/> associated with the event.</param>
        protected virtual void OnMessageReturned(OleDbInfoMessageEventArgs e)
        {
            OleDbInfoMessageEventHandler temp = this.MessageReturned;

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
        private static void TraceAdapterMessages(object sender, OleDbInfoMessageEventArgs e)
        {
            // TODO: obfuscate passwords.
            Trace.TraceInformation("[{0}] {1}: {2}", sender, e.Source, e.Message);

            foreach (OleDbError error in e.Errors)
            {
                Trace.TraceInformation("[{0}] {1}: {2}", sender, e.Source, error);
            }
        }

        /// <summary>
        /// Triggers the MessageReturned event for this <see cref="OleDbCommandAdapter"/>.
        /// </summary>
        /// <param name="sender">The original sender of the <see cref="OleDbInfoMessageEventArgs"/> event.</param>
        /// <param name="e">The <see cref="OleDbInfoMessageEventArgs"/> associated with the event.</param>
        private void TriggerMessageReturned(object sender, OleDbInfoMessageEventArgs e)
        {
            this.OnMessageReturned(e);
        }
    }
}
