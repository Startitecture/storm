// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReadOnlyRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that allow read-only access to repository data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Collections.Generic;

    using Startitecture.Orm.Query;

    /// <summary>
    /// Provides an interface for classes that allow read-only access to repository data.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity to allow access to.
    /// </typeparam>
    public interface IReadOnlyRepository<out TEntity>
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
        /// Selects all items in the repository.
        /// </summary>
        /// <returns>
        /// A collection of items that match the criteria.
        /// </returns>
        IEnumerable<TEntity> SelectAll();

        /// <summary>
        /// Selects a list of entities using a selection for the specified data item type.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to filter repository results.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of data item that represents the <typeparamref name="TEntity"/>.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IEnumerable{TEnity}"/> of the matching items.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
#pragma warning disable CA1716 // Identifiers should not match keywords
        IEnumerable<TEntity> Select<TItem>(ItemSelection<TItem> selection);
#pragma warning restore CA1716 // Identifiers should not match keywords
    }
}