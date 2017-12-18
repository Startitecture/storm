// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReadOnlyRepository.cs" company="TractManager, Inc.">
//   Copyright 2013 TractManager, Inc. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that allow read-only access to repository data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SAF.Data
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    /// <summary>
    /// Provides an interface for classes that allow read-only access to repository data.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity to allow access to.
    /// </typeparam>
    public interface IReadOnlyRepository<TEntity>
    {
        /// <summary>
        /// Determines whether an item exists in the repository.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to search for.
        /// </typeparam>
        /// <param name="candidate">
        /// The candidate to search for.
        /// </param>
        /// <returns>
        /// <c>true</c> if the item exists in the repository; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="candidate"/> is null.
        /// </exception>
        /// <remarks>
        /// This method always queries the underlying provider directly rather than using a cache.
        /// </remarks>
        bool Contains<TItem>(TItem candidate);

        /// <summary>
        /// Gets an item by its identifier or unique key.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to search for.
        /// </typeparam>
        /// <param name="candidate">
        /// A candidate item representing the item to search for.
        /// </param>
        /// <returns>
        /// The first <typeparamref name="TEntity"/> in the repository matching the candidate item's identifier or unique key, or a 
        /// default value of the <typeparamref name="TEntity"/> type if no entity could be found using the candidate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="candidate"/> is null.
        /// </exception>
        TEntity FirstOrDefault<TItem>(TItem candidate);

        /// <summary>
        /// Loads an entity with its children.
        /// </summary>
        /// <typeparam name="TKey">
        /// The type of the key that uniquely identifies the entity.
        /// </typeparam>
        /// <param name="key">
        /// The key that uniquely identifies the entity.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        /// <returns>
        /// The entity with the specified key, with all child elements loaded, or null if the entity does not exist.
        /// </returns>
        TEntity FirstOrDefaultWithChildren<TKey>(TKey key);

        /// <summary>
        /// Gets an item by its identifier or unique key.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to search for.
        /// </typeparam>
        /// <param name="candidate">
        /// A candidate item representing the item to search for.
        /// </param>
        /// <returns>
        /// The first <typeparamref name="TItem"/> in the repository matching the candidate item's identifier or unique key, or a 
        /// default value of the <typeparamref name="TItem"/> type if no entity could be found using the candidate.
        /// </returns>
        TItem FirstOrDefaultAs<TItem>(TItem candidate);

        /// <summary>
        /// Selects all items in the repository.
        /// </summary>
        /// <returns>
        /// A collection of items that match the criteria.
        /// </returns>
        IEnumerable<TEntity> SelectAll();

        /// <summary>
        /// Selects a list of items from the repository.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item that contains the properties to select.
        /// </typeparam>
        /// <param name="selection">
        /// The selection criteria.
        /// </param>
        /// <returns>
        /// A collection of items that match the criteria.
        /// </returns>
        IEnumerable<TEntity> SelectEntities<TItem>(IExampleQuery<TItem> selection);

        /// <summary>
        /// Selects a list of items from the repository.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item that contains the properties to select.
        /// </typeparam>
        /// <param name="selection">
        /// The selection criteria.
        /// </param>
        /// <returns>
        /// A collection of items that match the criteria.
        /// </returns>
        Page<TEntity> SelectEntityPage<TItem>(IExampleQuery<TItem> selection);

        /// <summary>
        /// Selects a list of items from the repository. The items are converted directly to the <typeparamref name="TItem"/> type
        /// from the underlying data item type.
        /// </summary>
        /// <param name="selection">
        /// The selection criteria.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that contains the properties to select.
        /// </typeparam>
        /// <returns>
        /// A collection of items that match the criteria.
        /// </returns>
        IEnumerable<TItem> SelectAs<TItem>(IExampleQuery<TItem> selection);

        /// <summary>
        /// Selects a list of items from the repository. The items are converted directly to the <typeparamref name="TItem"/> type
        /// from the underlying data item type.
        /// </summary>
        /// <param name="selection">
        /// The selection criteria.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item that contains the properties to select.
        /// </typeparam>
        /// <returns>
        /// A collection of items that match the criteria.
        /// </returns>
        Page<TItem> SelectPageAs<TItem>(IExampleQuery<TItem> selection);

        /// <summary>
        /// Loads the children of the specified entity.
        /// </summary>
        /// <param name="entity">
        /// The entity to load the children for.
        /// </param>
        void LoadChildren(TEntity entity);
    }
}