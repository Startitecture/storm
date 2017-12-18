namespace SAF.Data.Persistence
{
    /// <summary>
    /// Provides an interface to a dataset command adapter.
    /// </summary>
    public interface IDatabaseAdapter
    {
        /// <summary>
        /// Gets the command adapter for the dataset.
        /// </summary>
        ICommandAdapter CommandAdapter { get; }

        /// <summary>
        /// Initializes the database adapter with a name and connection string.
        /// </summary>
        /// <param name="name">The name of the adapter.</param>
        /// <param name="connectionString">The connection string to the target database.</param>
        void Initialize(string name, string connectionString);
    }
}
