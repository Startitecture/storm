namespace SAF.Data.Persistence
{
    using System;
    using System.Data;

    /// <summary>
    /// Provides an interface to a shared connection for data commands that can be 
    /// explicitly opened and closed.
    /// </summary>
    /// <remarks>.NET typed datasets open and close a unique database connection with 
    /// each command, without a public interface to change the connection other than the 
    /// class settings. This class acts as an extension to a typed dataset and allows a 
    /// connection to remain open at the discretion of the caller for all commands.</remarks>
    public interface ICommandAdapter : IDisposable
    {
        /// <summary>
        /// Occurs when the shared connection state changes.
        /// </summary>
        event StateChangeEventHandler ConnectionStateChanged;

        /// <summary>
        /// Gets the name of this shared connection.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the connection string for this shared connection.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Gets the state of the connection.
        /// </summary>
        ConnectionState State { get; }

        /// <summary>
        /// Gets or sets a value indicating whether adapter messages will be traced or not.
        /// </summary>
        bool TraceMessages { get; set; }

        /// <summary>
        /// Creates a transaction adapter with the specified name.
        /// </summary>
        /// <param name="name">The name of the adapter.</param>
        /// <param name="connectionString">The connection string for the adapter to use.</param>
        void Initialize(string name, string connectionString);

        /// <summary>
        /// Opens the transaction connection.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot open a connection 
        /// without specifying a data source or server. The connection is already 
        /// open.</exception>
        /// <exception cref="System.Data.SqlClient.SqlException">A connection-level 
        /// error occurred while opening the connection. If the 
        /// System.Data.SqlClient.SqlException.Number property contains the value 18487 
        /// or 18488, this indicates that the specified password has expired or must be 
        /// reset. See the 
        /// System.Data.SqlClient.SqlConnection.ChangePassword(System.String,System.String) 
        /// method for more information.</exception>
        void Open();

        /// <summary>
        /// Gets the return value of the specified stored procedure.
        /// </summary>
        /// <param name="storedProcedure">The name (including namespace) of the stored
        /// procedure to get the return value for</param>
        /// <returns>The return value of the specified stored procedure, if it exists, or null.</returns>
        object GetReturnValue(string storedProcedure);

        /// <summary>
        /// Sets the command timeout for a specific command.
        /// </summary>
        /// <param name="commandText">The command text that matches the command</param>
        /// <param name="timeout">The number of seconds to set as the timeout</param>
        void SetCommandTimeout(string commandText, int timeout);

        /// <summary>
        /// Closes the transaction connection.
        /// </summary>
        void Close();
    }
}
