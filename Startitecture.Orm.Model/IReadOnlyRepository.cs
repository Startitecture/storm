// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReadOnlyRepository.cs" company="Startitecture">
//   Copyright 2017 Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that allow read-only access to repository data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Model
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides an interface for classes that allow read-only access to repository data.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of model that the repository reads.
    /// </typeparam>
    public interface IReadOnlyRepository<out TModel>
    {
        /// <summary>
        /// Determines whether an item exists in the repository.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to search for.
        /// </typeparam>
        /// <param name="candidate">
        /// The candidate item to search for.
        /// </param>
        /// <returns>
        /// <c>true</c> if the item exists in the repository; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="candidate"/> is null.
        /// </exception>
        bool Contains<TItem>(TItem candidate);

        /// <summary>
        /// Determines whether an item exists in the repository.
        /// </summary>
        /// <param name="selection">
        /// The selection to test.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item with the properties to test.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if a matching item exists in the repository; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        bool Contains<TItem>(EntitySelection<TItem> selection);

        /// <summary>
        /// Gets an item by its identifier or unique key.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to search for.
        /// </typeparam>
        /// <param name="candidate">
        /// The candidate item to search for.
        /// </param>
        /// <returns>
        /// The first <typeparamref name="TModel"/> in the repository matching the candidate item's identifier or unique key, or a 
        /// default value of the <typeparamref name="TModel"/> type if no entity could be found using the candidate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="candidate"/> is null.
        /// </exception>
        TModel FirstOrDefault<TItem>(TItem candidate);

        /// <summary>
        /// Gets the first matching item of the selection. To ensure repeatable results, use unique column criteria.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to search for.
        /// </typeparam>
        /// <param name="selection">
        /// The item selection.
        /// </param>
        /// <returns>
        /// The first <typeparamref name="TModel"/> in the repository matching the candidate item's identifier or unique key, or a 
        /// default value of the <typeparamref name="TModel"/> type if no entity could be found using the candidate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        TModel FirstOrDefault<TItem>(EntitySelection<TItem> selection);

        /// <summary>
        /// Selects all the domain models of the type <typeparamref name="TModel"/> in the repository.
        /// </summary>
        /// <returns>
        /// A collection of domain models that match the criteria.
        /// </returns>
        IEnumerable<TModel> SelectAll();

        /// <summary>
        /// Selects a list of domain models using a selection for the specified data item type.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to filter repository results.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of data item that represents the <typeparamref name="TModel"/>.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IEnumerable{Model}"/> of the matching items.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
#pragma warning disable CA1716 // Identifiers should not match keywords
        IEnumerable<TModel> Select<TItem>(EntitySelection<TItem> selection);
#pragma warning restore CA1716 // Identifiers should not match keywords
    }
}