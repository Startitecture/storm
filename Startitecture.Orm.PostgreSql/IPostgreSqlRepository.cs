// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPostgreSqlRepository.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Orm.PostgreSql
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Startitecture.Orm.Common;

    /// <summary>
    /// Provides an interface to a PostgreSQL repository.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of model represented by the repository.
    /// </typeparam>
    /// <typeparam name="TEntity">
    /// The type of entity stored in the repository.
    /// </typeparam>
    public interface IPostgreSqlRepository<TModel, TEntity> : IEntityRepository<TModel>
        where TEntity : class, new()
    {
        /// <summary>
        /// Inserts a list of items into the repository.
        /// </summary>
        /// <param name="items">
        /// The items to insert.
        /// </param>
        /// <param name="insertAction">
        /// The insert action to take, or null to take the default insert action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to insert..
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is null.
        /// </exception>
        void Insert<TItem>([NotNull] IEnumerable<TItem> items, Action<JsonInsert<TEntity>> insertAction);

        /// <summary>
        /// Inserts a list of items into the repository.
        /// </summary>
        /// <param name="items">
        /// The items to insert.
        /// </param>
        /// <param name="insertAction">
        /// The insert action to take, or null to take the default insert action.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to insert..
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is null.
        /// </exception>
        /// <returns>
        /// The <see cref="Task"/> that is performing the insert.
        /// </returns>
        Task InsertAsync<TItem>([NotNull] IEnumerable<TItem> items, Action<JsonInsert<TEntity>> insertAction, CancellationToken cancellationToken);

        /// <summary>
        /// Inserts a list of items into the repository.
        /// </summary>
        /// <param name="items">
        /// The items to insert.
        /// </param>
        /// <param name="insertAction">
        /// The insert action to take, or null to take the default insert action.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to insert..
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is null.
        /// </exception>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <typeparamref name="TItem"/> items.
        /// </returns>
        IEnumerable<TItem> InsertForResults<TItem>(
            [NotNull] IEnumerable<TItem> items,
            Action<JsonInsert<TEntity>> insertAction);

        /// <summary>
        /// Inserts a list of items into the repository.
        /// </summary>
        /// <param name="items">
        /// The items to insert.
        /// </param>
        /// <param name="insertAction">
        /// The insert action to take, or null to take the default insert action.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token for this task.
        /// </param>
        /// <typeparam name="TItem">
        /// The type of item to insert..
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is null.
        /// </exception>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <typeparamref name="TItem"/> items.
        /// </returns>
        IAsyncEnumerable<TItem> InsertForResultsAsync<TItem>(
            [NotNull] IEnumerable<TItem> items,
            Action<JsonInsert<TEntity>> insertAction,
            CancellationToken cancellationToken);
    }
}