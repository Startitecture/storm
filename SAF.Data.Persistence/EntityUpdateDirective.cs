namespace SAF.Data.Persistence
{
    using System;

    /// <summary>
    /// Contains instructions for updating an entity.
    /// </summary>
    /// <typeparam name="T">The type of entity used to update the target dataset.</typeparam>
    public class EntityUpdateDirective<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityUpdateDirective&lt;T&gt;"/> class with the
        /// specified persistence adapter and entity.
        /// </summary>
        /// <param name="adapter">The adapter that will persist the entity.</param>
        /// <param name="entity">The entity to update.</param>
        public EntityUpdateDirective(IPersistenceAdapter<T> adapter, T entity)
        {
            if (adapter == null)
            {
                throw new ArgumentNullException("adapter");
            }

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.Adapter = adapter;
            this.Entity = entity;
        }

        /// <summary>
        /// Gets the adapter that will persist the entity update.
        /// </summary>
        public IPersistenceAdapter<T> Adapter { get; private set; }

        /// <summary>
        /// Gets the entity to update.
        /// </summary>
        public T Entity { get; private set; }
    }
}
