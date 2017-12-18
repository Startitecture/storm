namespace SAF.Data.Persistence
{
    using SAF.Data;

    /// <summary>
    /// Provides an interface to an entity data source.
    /// </summary>
    public interface IEntityDataSource : IDataSource
    {
        /// <summary>
        /// Gets the connection string to the data source.
        /// </summary>
        string ConnectionString { get; }
    }
}
