namespace SAF.Data.Persistence
{
    /// <summary>
    /// Provides an interface to an adapter that can persist an entity.
    /// </summary>
    /// <typeparam name="T">The type of entity that the adapter can persist.</typeparam>
    public interface IPersistenceAdapter<T>
    {
        /// <summary>
        /// Persists the provided entity to the database.
        /// </summary>
        /// <param name="entity">The entity to persist.</param>
        /// <returns>Return value of the persistence statement.</returns>
        EntityAction SaveEntity(T entity);
    }
}
