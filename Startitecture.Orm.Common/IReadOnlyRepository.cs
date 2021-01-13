// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReadOnlyRepository.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// <summary>
//   Provides an interface for classes that allow read-only access to repository data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.Common
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Startitecture.Orm.Model;

    /// <summary>
    /// Provides an interface for classes that allow read-only access to repository data.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of model that the repository reads.
    /// </typeparam>
    public interface IReadOnlyRepository<TModel>
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
        Task<bool> ContainsAsync<TItem>(TItem candidate);

        /// <summary>
        /// Determines whether an item exists in the repository.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to query the repository.
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
        bool Contains<TItem>(EntitySet<TItem> selection);

        /// <summary>
        /// Determines whether an item exists in the repository.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to query the repository.
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
        Task<bool> ContainsAsync<TItem>(EntitySet<TItem> selection);

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
        Task<TModel> FirstOrDefaultAsync<TItem>(TItem candidate);

        /// <summary>
        /// Gets the first matching item of the selection. To ensure repeatable results, use unique column criteria.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to search for.
        /// </typeparam>
        /// <param name="selection">
        /// The selection to use to query the repository.
        /// </param>
        /// <returns>
        /// The first <typeparamref name="TModel"/> in the repository matching the candidate item's identifier or unique key, or a
        /// default value of the <typeparamref name="TModel"/> type if no entity could be found using the candidate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        TModel FirstOrDefault<TItem>(EntitySet<TItem> selection);

        /// <summary>
        /// Gets the first matching item of the selection. To ensure repeatable results, use unique column criteria.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item to search for.
        /// </typeparam>
        /// <param name="selection">
        /// The selection to use to query the repository.
        /// </param>
        /// <returns>
        /// The first <typeparamref name="TModel"/> in the repository matching the candidate item's identifier or unique key, or a
        /// default value of the <typeparamref name="TModel"/> type if no entity could be found using the candidate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        Task<TModel> FirstOrDefaultAsync<TItem>(EntitySet<TItem> selection);

        /// <summary>
        /// Gets the first matching item of the selection. To ensure repeatable results, use unique column criteria.
        /// </summary>
        /// <param name="defineSet">
        /// Define the set to use to query the repository.
        /// </param>
        /// <returns>
        /// The first <typeparamref name="TModel"/> in the repository matching the candidate item's identifier or unique key, or a
        /// default value of the <typeparamref name="TModel"/> type if no entity could be found using the candidate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defineSet"/> is null.
        /// </exception>
        TModel FirstOrDefault(Action<EntitySet<TModel>> defineSet);

        /// <summary>
        /// Gets the first matching item of the selection. To ensure repeatable results, use unique column criteria.
        /// </summary>
        /// <param name="defineSet">
        /// Define the set to use to query the repository.
        /// </param>
        /// <returns>
        /// The first <typeparamref name="TModel"/> in the repository matching the candidate item's identifier or unique key, or a
        /// default value of the <typeparamref name="TModel"/> type if no entity could be found using the candidate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defineSet"/> is null.
        /// </exception>
        Task<TModel> FirstOrDefaultAsync(Action<EntitySet<TModel>> defineSet);

        /// <summary>
        /// Gets the first matching result of the <paramref name="selection"/> as a dynamic object.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to query the repository.
        /// </param>
        /// <returns>
        /// The first result of the <paramref name="selection"/> as a dynamic object, or null if no results were found.
        /// </returns>
        dynamic DynamicFirstOrDefault(ISelection selection);

        /// <summary>
        /// Gets the first matching result of the <paramref name="selection"/> as a dynamic object.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to query the repository.
        /// </param>
        /// <returns>
        /// The first result of the <paramref name="selection"/> as a dynamic object, or null if no results were found.
        /// </returns>
        Task<dynamic> DynamicFirstOrDefaultAsync(ISelection selection);

        /// <summary>
        /// Gets a scalar result from the specified query.
        /// </summary>
        /// <param name="selection">
        /// A selection for the specified scalar to return.
        /// </param>
        /// <typeparam name="T">
        /// The type of scalar to return.
        /// </typeparam>
        /// <returns>
        /// The scalar value as a <typeparamref name="T"/>.
        /// </returns>
        T GetScalar<T>(ISelection selection);

        /// <summary>
        /// Gets a scalar result from the specified query.
        /// </summary>
        /// <param name="selection">
        /// A selection for the specified scalar to return.
        /// </param>
        /// <typeparam name="T">
        /// The type of scalar to return.
        /// </typeparam>
        /// <returns>
        /// The scalar value as a <typeparamref name="T"/>.
        /// </returns>
        Task<T> GetScalarAsync<T>(ISelection selection);

        /// <summary>
        /// Selects all the domain models of the type <typeparamref name="TModel"/> in the repository.
        /// </summary>
        /// <returns>
        /// A collection of domain models that match the criteria.
        /// </returns>
        IEnumerable<TModel> SelectAll();

        /// <summary>
        /// Selects all the domain models of the type <typeparamref name="TModel"/> in the repository.
        /// </summary>
        /// <returns>
        /// A collection of domain models that match the criteria.
        /// </returns>
        Task<IEnumerable<TModel>> SelectAllAsync();

        /// <summary>
        /// Returns a collection of domain models matching the <paramref name="selection"/> for the specified data item type.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to query the repository.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of data item that represents the <typeparamref name="TModel"/>.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of the matching items.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        IEnumerable<TModel> SelectEntities<TItem>(EntitySet<TItem> selection);

        /// <summary>
        /// Returns a collection of domain models matching the <paramref name="selection"/> for the specified data item type.
        /// </summary>
        /// <param name="selection">
        /// The selection to use to query the repository.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of data item that represents the <typeparamref name="TModel"/>.
        /// </typeparam>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of the matching items.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="selection"/> is null.
        /// </exception>
        Task<IEnumerable<TModel>> SelectEntitiesAsync<TItem>(EntitySet<TItem> selection);

        /// <summary>
        /// Returns a collection of dynamic objects matching the <paramref name="selection"/>.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item the selection is based on.
        /// </typeparam>
        /// <param name="selection">
        /// The selection to use to query the repository.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of dynamic objects matching the <paramref name="selection"/>.
        /// </returns>
        IEnumerable<dynamic> DynamicSelect<TItem>(EntitySelection<TItem> selection);

        /// <summary>
        /// Returns a collection of dynamic objects matching the <paramref name="selection"/>.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of item the selection is based on.
        /// </typeparam>
        /// <param name="selection">
        /// The selection to use to query the repository.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of dynamic objects matching the <paramref name="selection"/>.
        /// </returns>
        Task<IEnumerable<dynamic>> DynamicSelectAsync<TItem>(EntitySelection<TItem> selection);
    }
}