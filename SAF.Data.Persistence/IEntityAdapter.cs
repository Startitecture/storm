namespace SAF.Data.Persistence
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface to an entity adapter.
    /// </summary>
    /// <typeparam name="T">The type of entity that the adapter can fill.</typeparam>
    public interface IEntityAdapter<T> : IDatabaseAdapter
    {
        /////// <summary>
        /////// Occurs when the select query is executed.
        /////// </summary>
        ////event EventHandler QueryStarted;

        /// <summary>
        /// Selects all the items from the entity store.
        /// </summary>
        /// <returns>An enumerable of all the items in the entity store.</returns>
        IEnumerable<T> SelectAllItems();
    }
}
