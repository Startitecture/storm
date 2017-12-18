// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryObjectFactory.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides methods to create and populate <see cref="DirectoryObject" /> instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.ActiveDirectory.Factories
{
    using System;
    using System.DirectoryServices;

    /// <summary>
    /// Provides methods to create and populate <see cref="DirectoryObject"/> instances.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to create.</typeparam>
    public abstract class DirectoryObjectFactory<TEntity>
        where TEntity : DirectoryObject
    {
        /// <summary>
        /// Returns a new <see cref="DirectoryObject"/> using the specified <see cref="DirectoryEntry"/>.
        /// </summary>
        /// <param name="entry">The <see cref="DirectoryEntry"/> to use for creating the new <see cref="DirectoryObject"/>.</param>
        /// <returns>A new <see cref="DirectoryObject"/> using the specified <see cref="DirectoryEntry"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="entry"/> is null.</exception>
        public TEntity CreateFrom(DirectoryEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            TEntity entity = this.GenerateEntity();
            this.PopulateWith(entity, entry);
            return entity;
        }

        /// <summary>
        /// Populates a <see cref="DirectoryObject"/> using the specified <see cref="DirectoryEntry"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DirectoryObject"/> to populate.</param>
        /// <param name="entry">The <see cref="DirectoryEntry"/> to use for populating the <see cref="DirectoryObject"/>.</param>
        public abstract void PopulateWith(TEntity entity, DirectoryEntry entry);

        /// <summary>
        /// Generates a new entity.
        /// </summary>
        /// <returns>A newly generated entity.</returns>
        protected abstract TEntity GenerateEntity();
    }
}
