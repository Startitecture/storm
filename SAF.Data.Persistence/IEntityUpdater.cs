namespace SAF.Data.Persistence
{
    using SAF.ProcessEngine;

    /// <summary>
    /// Provides an interface to entity updater integration controllers.
    /// </summary>
    /// <typeparam name="TSource">The type of data source the updater processes.</typeparam>
    public interface IEntityUpdater<TSource>
    {
        /// <summary>
        /// Gets the process that the entity updater is using.
        /// </summary>
        IProcessEngine Process { get; }

        /// <summary>
        /// Updates the Federal Exclusion Compliance database with the specified data source.
        /// </summary>
        /// <param name="dataSource">The data source to use for the update.</param>
        void Update(TSource dataSource);
    }
}
