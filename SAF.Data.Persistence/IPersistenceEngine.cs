namespace SAF.Data.Persistence
{
    using SAF.ProcessEngine;

    /// <summary>
    /// Provides an interface to a persistence engine.
    /// </summary>
    public interface IPersistenceEngine : ITaskEngine
    {
        /// <summary>
        /// Gets the number of added items the engine has processed.
        /// </summary>
        long AddedItems { get; }

        /// <summary>
        /// Gets the number of modified items the engine has processed.
        /// </summary>
        long ModifiedItems { get; }

        /// <summary>
        /// Gets the number of unchanged items the engine has processed.
        /// </summary>
        long UnchangedItems { get; }

        /// <summary>
        /// Gets the number of removed items the engine has processed.
        /// </summary>
        long RemovedItems { get; }

        /// <summary>
        /// Gets the number of rolled back items the engine has processed.
        /// </summary>
        long RolledBackItems { get; }
    }
}
